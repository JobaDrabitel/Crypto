using System.Net.Mail;

namespace FP.Core.Api.Providers.Providers;

public class ConfirmEmailAddressProvider
{
	public async Task<string> Confirm(string email, string subject, string body)
	{
		var status = "Ok";

		var mail = new MailMessage
		{
			From = new MailAddress(""),
			Subject = subject,
			Body = body
		};
		mail.To.Add(new MailAddress(email));

		return status;
	}
}
