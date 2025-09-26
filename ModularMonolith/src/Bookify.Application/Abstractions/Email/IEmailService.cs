namespace Bookify.Application.Abstractions.Email;

public interface IEmailService
{
	Task SendAsync(Domain.Shared.Email recipient, string subject, string body);
}
