using Bookify.Application.Abstractions.Email;

namespace Bookify.Infrastructure.Email;

internal sealed class EmailService : IEmailService
{
	public Task SendAsync(Domain.Users.Email recipient, string subject, string body)
	{
		// We return task completed task to simulate that this method completed successfully. 
		return Task.CompletedTask;
	}
}