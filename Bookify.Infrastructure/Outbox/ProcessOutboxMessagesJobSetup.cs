using Microsoft.Extensions.Options;
using Quartz;

namespace Bookify.Infrastructure.Outbox;

internal sealed class ProcessOutboxMessagesJobSetup : IConfigureOptions<QuartzOptions>
{
	private readonly OutboxOptions _outboxOptions;

	public ProcessOutboxMessagesJobSetup ( IOptions<OutboxOptions> outboxOptions )
	{
		_outboxOptions = outboxOptions.Value;
	}

	public void Configure ( QuartzOptions options )
	{
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
