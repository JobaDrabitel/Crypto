namespace FP.Core.Api.ApiDto;

public class UserUpdateDto
{
    public string? Name { get; set; } = string.Empty;
    public string? Surname { get; set; } = string.Empty;
	public string? Nickname { get; set; } = string.Empty;
    public string? Country { get; set; } = string.Empty;
    public string? City { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Telegram { get; set; }
    public string? Email { get; set; }
    public string? Avatar { get; set; } = string.Empty;

    public bool ShowPhone { get; set; } = false;
    public bool ShowTg { get; set; } = false;
    public bool ShowEmail { get; set; } = false;
}