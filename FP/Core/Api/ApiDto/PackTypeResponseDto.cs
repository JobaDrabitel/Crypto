using FP.Core.Database.Models;

namespace FP.Core.Api.ApiDto;

public class PackTypeResponseDto
{
    public int Id { get; set; }
    public decimal Yield { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MaxDuration { get; set; }
    public int MinDuration { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal ActiveSum { get; set; }
    public string? Avatar { get; set; }
    public decimal RandomTotalIncome { get; set; }
    public decimal RandomActiveSum { get; set; }

    public PackTypeResponseDto(PackType packType)
    {
        Id = packType.Id;
        Yield = packType.Yield;
        Name = packType.Name;
        MaxDuration = packType.MaxDuration;
        MinDuration = packType.MinDuration;
        TotalIncome = packType.TotalIncome;
        ActiveSum = packType.ActiveSum;
        Avatar = packType.Avatar;
        
        RandomTotalIncome = TotalIncome + TotalIncome * Random.Shared.Next(-10, 10) / 100;
        RandomActiveSum = ActiveSum + ActiveSum * Random.Shared.Next(-10, 10) / 100;
    }
}