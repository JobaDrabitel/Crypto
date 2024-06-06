namespace FP.Core.Api.ApiDto;

public class PackDto
{
    public string InvestmentCode { get; set; } = null!;

    public MinPackDto[]? PackDtos { get; set; } = null;
}