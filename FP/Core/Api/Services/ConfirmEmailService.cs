using FP.Core.Api.ApiDto;
using FP.Core.Database.Handlers;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace FP.Core.Api.Services;

public class ConfirmEmailService
{
    public static string SmtpAddress = "";
    public static string SmtpEmail = "";
    public static string SmtpPassword = "";
    public static int SmtpPort = 0;

    private readonly VerificationCodeDatabaseHandler _dbHandler;

    public ConfirmEmailService(VerificationCodeDatabaseHandler dbHandler)
    {
        _dbHandler = dbHandler;

    }
    
    public async Task<bool> SendConfirmEmail(ConfirmEmailDto emailDto, string email)
    {
        var isSuccess = true;

        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("info", SmtpEmail));
        emailMessage.To.Add(new MailboxAddress("", email));
        emailMessage.Subject = emailDto.Subject;

        var code = await GetVerificationCode();
        if (code == 0)
            return false;
        
        emailMessage.Body = new TextPart(TextFormat.Html)
        {
            Text = emailDto.TextBeforeCode + code + emailDto.TextAfterCode
        };

        try
        {
            using var client = new SmtpClient();
            
            await client.ConnectAsync(SmtpAddress, SmtpPort, false);
            await client.AuthenticateAsync(SmtpEmail, SmtpPassword);
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            isSuccess = false;
        }
        
        return isSuccess;
    }
    public async Task<bool> SendConfirmEmail(CreateTransactionDto emailDto, string email)
    {
        var isSuccess = true;

        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("info", SmtpEmail));
        emailMessage.To.Add(new MailboxAddress("", email));
        emailMessage.Subject = emailDto.Subject;

        var code = await GetVerificationCode();
        if (code == 0)
            return false;
        
        emailMessage.Body = new TextPart(TextFormat.Html)
        {
            Text = emailDto.TextBeforeCode + code + emailDto.TextAfterCode
        };

        try
        {
            using var client = new SmtpClient();
            
            await client.ConnectAsync(SmtpAddress, SmtpPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(SmtpEmail, SmtpPassword);
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            isSuccess = false;
        }
        
        return isSuccess;
    } 
    public async Task<bool> SendRecoverEmail(string url, string email)
    {
        var isSuccess = true;

        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("info", SmtpEmail));
        emailMessage.To.Add(new MailboxAddress("", email));

        var code = await GetVerificationCode();
        if (code == 0)
            return false;
        
        emailMessage.Body = new TextPart(TextFormat.Html)
        {
            Text = url
        };

        try
        {
            using var client = new SmtpClient();
            
            await client.ConnectAsync(SmtpAddress, SmtpPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(SmtpEmail, SmtpPassword);
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            isSuccess = false;
        }
        
        return isSuccess;
    }

    public async Task<bool> ConfirmEmail(int code) => (await _dbHandler.FindCode(code.ToString())).Status;

    public async Task<int> GetVerificationCode()
    {
        var random = new Random();
        var code = random.Next(100000, 999999);

        var timer = new Timer(TimeExpired, code, TimeSpan.FromMinutes(2), TimeSpan.FromMilliseconds(-1));
        var response = await _dbHandler.Create(code.ToString());
        return response.Status? code : 0;
    }

    private async void TimeExpired(object? state)
    {
        if(state == null)
            return;
        
        var code = Convert.ToInt32(state);
        var strCode = code.ToString();

        _ = await _dbHandler.FindCode(strCode);
    }
}