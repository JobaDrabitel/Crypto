using FP.Core.Database.Models;

namespace FP.Core.Api.ApiDto;

public class PoolResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsClosed { get; set; } = true;
    public decimal TotalIncome { get; set; } = 0;
    public decimal ActiveSum { get; set; } = 0;
    public int MaxPacksCount { get; set; } = 0;
    public string Avatar { get; set; } = string.Empty;
    public decimal RandomTotalIncome { get; set; }
    public decimal RandomActiveSum { get; set; }

    public PoolResponseDto(Pool pool)
    {
        Id = pool.Id;
        Name = pool.Name;
        Description = pool.Description;
        IsClosed = pool.IsClosed;
        TotalIncome = pool.TotalIncome;
        ActiveSum = pool.ActiveSum;
        MaxPacksCount = pool.MaxPacksCount;
        Avatar = pool.Avatar;

        RandomTotalIncome = TotalIncome + TotalIncome * Random.Shared.Next(-10, 10) / 100;
        RandomActiveSum = ActiveSum + ActiveSum * Random.Shared.Next(-10, 10) / 100;
    }
}