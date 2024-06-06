namespace FP.Core.Api.ApiDto;

public class ReactivatePackDto
{
    public int PackTypeId { get; set; }
    public bool IsMaxEndDate { get; set; }
    public string InvestmentCode { get; set; } = null!;
}