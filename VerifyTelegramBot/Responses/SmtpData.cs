namespace FP.Core.Api.Responses;

public class SmtpData
{
    public string SmtpAddress { get; set; } = "";
    public string SmtpEmail { get; set; } = "";
    public string SmtpPassword { get; set; } = "";
    public int SmtpPort { get; set; } = 0;
}