using Bookify.Application.Abstractions.Email;

namespace Bookify.Infrastructure.Email;

internal sealed class EmailService : IEmailService
{
	public Task SendAsync(Domain.Shared.Email recipient, string subject, string body)
	{
		return Task.CompletedTask;
	}
}
