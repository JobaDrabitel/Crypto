namespace FP.Core.Api.ApiDto;

public class TestUserCreateDto
{
    public string ReferrerCode { get; set; } = null!;
    public decimal Sum { get; set; } = 0.0m;
    public int DaysCount { get; set; }
}