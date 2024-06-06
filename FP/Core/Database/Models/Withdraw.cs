using FP.Core.Api.Providers;
using FP.Core.Api.Services;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FP.Core.Database.Models
{
	public class Withdraw
	{
		public int Id { get; set; }
		public decimal Sum { get; set; }
		public DateTime CreationTime { get; set; }
		public DateTime? RealizationTime { get; set; }
		public string Status { get; set; } = WithdrawStatusEnum.Waiting.ToString();
		public string WalletAddress { get; set; }
		public string Type { get; set; } = WalletType.Trc20.ToString();
		public int UserId { get; set; }
		[ForeignKey("UserId")]
		public User User { get; set; } = null!;
	}
}
