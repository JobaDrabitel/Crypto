using System.ComponentModel.DataAnnotations.Schema;

namespace FP.Core.Database.Models;

public class DefiTransactions
{
    public int Id { get; set; }
    public decimal AdditionPercent { get; set; } = 0;
    public decimal Sum { get; set; } = 0.0m;
    public bool IsClosed { get; set; } = false;
    public int DaysWithoutWithdraws { get; set; } = 0;
    public int InvestId { get; set; }
    [ForeignKey("InvestId")]
    public Investment Investment { get; set; }
}