namespace FP.Core.Api.ApiDto;

public class ConfirmEmailDto
{
    public string Subject { get; set; } = "";
    public string TextBeforeCode { get; set; } = "";
    public string TextAfterCode { get; set; } = "";
}