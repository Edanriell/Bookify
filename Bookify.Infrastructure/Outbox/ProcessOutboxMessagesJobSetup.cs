using Microsoft.Extensions.Options;
using Quartz;

namespace Bookify.Infrastructure.Outbox;

// Transactional Outbox Pattern
public class ProcessOutboxMessagesJobSetup : IConfigureOptions<QuartzOptions>
{
	// Injecting outboxOptions instance so we can access the interval in seconds
	// which we are going to need to configure how often we run the process outbox messages job.
	private readonly OutboxOptions _outboxOptions;

	public ProcessOutboxMessagesJobSetup ( IOptions<OutboxOptions> outboxOptions )
	{
		_outboxOptions = outboxOptions.Value;
	}

	public void Configure ( QuartzOptions options )
	{
		// jobName is used as key for configuring our background job.
		const string jobName = nameof(ProcessOutboxMessagesJob);

		options.AddJob<ProcessOutboxMessagesJob> (
					configure : configure => configure.WithIdentity (
							name : jobName
						)
				).
			AddTrigger (
					configure : configure =>
						configure.ForJob (
									jobName : jobName
								).
							// Background job will run with a simple schedule, and this
							// schedule is going to have an interval in seconds which will be 
							// determined by our outbox options instance and the interval in seconds 
							// property, and we want to repeat this job forever as long as our application is running. 
							WithSimpleSchedule (
									action : schedule =>
										schedule.WithIntervalInSeconds (
													seconds : _outboxOptions.IntervalInSeconds
												).
											RepeatForever()
								)
				);
	}
}
