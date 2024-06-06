namespace FP.Core.Database.Models;

public class Pool
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsClosed { get; set; } = true;
    public decimal TotalIncome { get; set; } = 0;
    public decimal ActiveSum { get; set; } = 0;
    public int MaxPacksCount { get; set; } = 0;
    public string Avatar { get; set; } = string.Empty;
}