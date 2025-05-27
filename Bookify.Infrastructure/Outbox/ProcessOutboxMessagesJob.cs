using System.Data;
using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Data;
using Bookify.Domain.Abstractions;
using Dapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quartz;

namespace Bookify.Infrastructure.Outbox;

// Transactional Outbox Pattern
// This attribute is going to make sure that there is
// only one outbox message job instance running at any given time.
// This is going to solve any concurrency problems.
[ DisallowConcurrentExecution ]
internal sealed class ProcessOutboxMessagesJob : IJob
{
	private static readonly JsonSerializerSettings JsonSerializerSettings = new()
																			{
																				TypeNameHandling = TypeNameHandling.All
																			};

	// Is used to record when a given outbox message was processed
	private readonly IDateTimeProvider _dateTimeProvider;

	// Is used to log some contextual information
	private readonly ILogger<ProcessOutboxMessagesJob> _logger;

	// Is used to get the batch size
	private readonly OutboxOptions _outboxOptions;

	// Is used from mediator to publish the individual domain events
	private readonly IPublisher _publisher;

	// We are using sql connection factory because we are going to use Dapr to query the
	// outbox messages table and update the processed messages.
	private readonly ISqlConnectionFactory _sqlConnectionFactory;

	public ProcessOutboxMessagesJob ( ISqlConnectionFactory sqlConnectionFactory,
									  IPublisher publisher,
									  IDateTimeProvider dateTimeProvider,
									  IOptions<OutboxOptions> outboxOptions,
									  ILogger<ProcessOutboxMessagesJob> logger )
	{
		_sqlConnectionFactory = sqlConnectionFactory;
		_publisher = publisher;
		_dateTimeProvider = dateTimeProvider;
		_logger = logger;
		_outboxOptions = outboxOptions.Value;
	}

	public async Task Execute ( IJobExecutionContext context )
	{
		_logger.LogInformation (
				message : "Beginning to process outbox messages"
			);

		// Using SQLConnectionFactory to create a new database connection
		using var connection = _sqlConnectionFactory.CreateConnection();
		// And we will use this connection instance to open a database transaction
		// because we weant to process all of our outbox messages inside of a single 
		// transaction and make sure that they are all processed together. 
		using var transaction = connection.BeginTransaction();

		// We are querying the outbox messages table and returning back a collection of 
		// outbox messages.
		var outboxMessages = await GetOutboxMessagesAsync (
									 connection : connection,
									 transaction : transaction
								 );

		// Here we are going to loop over each outbox message instance. 
		foreach ( var outboxMessage in outboxMessages )
		{
			Exception? exception = null;

			try
			{
				// We are trying to deserialize it into an IDomainEvent instance by passing in
				// the outbox message content and the JSON serializer settings 
				var domainEvent = JsonConvert.DeserializeObject<IDomainEvent> (
						value : outboxMessage.Content,
						settings : JsonSerializerSettings
					)!;

				// and we are going to try
				// to publish this domain event using mediator.
				await _publisher.Publish (
						notification : domainEvent,
						cancellationToken : context.CancellationToken
					);
			}
			catch ( Exception caughtException )
			{
				// If we encounter an exception, we are going to log the exception together with
				// the message ID so that we know which message triggered the exception and we are
				// going to capture this exception in the exception variable and send it to the 
				// OutboxMessageAsync method. 
				_logger.LogError (
						exception : caughtException,
						message : "Exception while processing outbox message {MessageId}",
						outboxMessage.Id
					);

				exception = caughtException;
			}

			// This method is going to update our outbox message after we are done processing it.
			await UpdateOutboxMessageAsync (
					connection : connection,
					transaction : transaction,
					outboxMessage : outboxMessage,
					exception : exception
				);
		}
		// When we are done processing all of the outbox messages and our forEach loop
		// completes, we are going to commit the database transaction and add 
		// another information log that we have finished processing our outbox messages.

		transaction.Commit();

		_logger.LogInformation (
				message : "Completed processing outbox messages"
			);
	}

	private async Task<IReadOnlyList<OutboxMessageResponse>> GetOutboxMessagesAsync ( IDbConnection connection,
		IDbTransaction transaction )
	{
		// We are using select for update query which is going to log any rows that are
		// queried as part of a database transaction until we commit that transaction.
		// This means that if there are competing transactions or multiple instances of
		// our background job running, these rows will be locked and the concurrent instances
		// of the ProccessOutboxMessages jobs won’t be able to read those rows, which is really 
		// useful because we only want to process our outbox messages once, which is why we have
		// a filter here where ProcessedOnUTC column is null and we are just projecting back the
		// ID and the content which matches the properties on the outbox message response.
		// ForUpdate clause can be practical when we need to scale out our background job to
		// be able to process more outbox messages which could be a requirement in high throughput systems. 
		var sql = $"""                
				   SELECT id, content
				   FROM outbox_messages
				   WHERE processed_on_utc IS NULL
				   ORDER BY occurred_on_utc
				   LIMIT {
					   _outboxOptions.BatchSize
				   }
				   FOR UPDATE
				   """;

		var outboxMessages = await connection.QueryAsync<OutboxMessageResponse> (
									 sql : sql,
									 transaction : transaction
								 );

		return outboxMessages.ToList();
	}

	private async Task UpdateOutboxMessageAsync ( IDbConnection connection,
												  IDbTransaction transaction,
												  OutboxMessageResponse outboxMessage,
												  Exception? exception )
	{
		// This is just going to set the processed on column
		// to the current time, and the error column is going to be either a null 
		// string, or it is going to contain the exception that we caught while 
		// publishing the domain event.
		const string sql = @"
            UPDATE outbox_messages
            SET processed_on_utc = @ProcessedOnUtc,
                error = @Error
            WHERE id = @Id";

		await connection.ExecuteAsync (
				sql : sql,
				param : new
						{
							outboxMessage.Id,
							ProcessedOnUtc = _dateTimeProvider.UtcNow,
							Error = exception?.ToString()
						},
				transaction : transaction
			);
	}

	// We are using simple record to represent our outbox message response.
	// It contains the message ID and the content, which is a JSON string
	// which will be going to used to deserialize back into a domain event instance. 
	internal sealed record OutboxMessageResponse ( Guid Id, string Content );
}
