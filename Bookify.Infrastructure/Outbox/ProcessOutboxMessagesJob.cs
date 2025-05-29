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

[ DisallowConcurrentExecution ]
internal sealed class ProcessOutboxMessagesJob : IJob
{
	private static readonly JsonSerializerSettings JsonSerializerSettings = new()
																			{
																				TypeNameHandling = TypeNameHandling.All
																			};

	private readonly IDateTimeProvider _dateTimeProvider;
	private readonly ILogger<ProcessOutboxMessagesJob> _logger;
	private readonly OutboxOptions _outboxOptions;
	private readonly IPublisher _publisher;

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

		using var connection = _sqlConnectionFactory.CreateConnection();
		using var transaction = connection.BeginTransaction();

		var outboxMessages = await GetOutboxMessagesAsync (
									 connection : connection,
									 transaction : transaction
								 );

		foreach ( var outboxMessage in outboxMessages )
		{
			Exception? exception = null;

			try
			{
				var domainEvent = JsonConvert.DeserializeObject<IDomainEvent> (
						value : outboxMessage.Content,
						settings : JsonSerializerSettings
					)!;

				await _publisher.Publish (
						notification : domainEvent,
						cancellationToken : context.CancellationToken
					);
			}
			catch ( Exception caughtException )
			{
				_logger.LogError (
						exception : caughtException,
						message : "Exception while processing outbox message {MessageId}",
						outboxMessage.Id
					);

				exception = caughtException;
			}

			await UpdateOutboxMessageAsync (
					connection : connection,
					transaction : transaction,
					outboxMessage : outboxMessage,
					exception : exception
				);
		}

		transaction.Commit();

		_logger.LogInformation (
				message : "Completed processing outbox messages"
			);
	}

	private async Task<IReadOnlyList<OutboxMessageResponse>> GetOutboxMessagesAsync ( IDbConnection connection,
		IDbTransaction transaction )
	{
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

	internal sealed record OutboxMessageResponse ( Guid Id, string Content );
}
