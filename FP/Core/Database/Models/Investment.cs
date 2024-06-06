using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FP.Core.Database.Models;
public class Investment
{
	public int Id { get; set; }
	public decimal TotalSum { get; set; }
	public decimal MaxSum { get; set; }
	public decimal TotalYield { get; set; }
	public bool IsEnded { get; set; } = false;
	public bool IsClosed { get; set; } = false;
	public DateTime StartDate { get; set; } = DateTime.UtcNow;
	public DateTime LastAccrualDate { get; set; } = DateTime.UtcNow;
	public DateTime EndDate { get; set; }
	public int PacksCount { get; set; } = 0;
	public int MaxPacksCount { get; set; } = 0;
	public decimal TotalAccrual { get; set; }
	public string Code { get; set; } = string.Empty;

	public int UserId { get; set; }
	[ForeignKey("UserId")]
	public User User { get; set; } = null!;
	
	public int PoolId { get; set; }
	[ForeignKey("PoolId")]
	public Pool Pool { get; set; } = null!;
	public bool ReferralPay { get; set; } = true;
	
	[JsonIgnore] public ICollection<Pack> Packs { get; set; } = new List<Pack>();
	[JsonIgnore] public ICollection<DefiTransactions> DefiTransactions { get; set; } = new List<DefiTransactions>();
}


