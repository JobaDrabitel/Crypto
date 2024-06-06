using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FP.Core.Database.Models;

public class Wallet
{
	public int Id { get; set; }
	public string WalletAddress { get; set; } = "";
	public string WalletType { get; set; } = "";
	public int UserId { get; set; }
	[ForeignKey("UserId")]
	[JsonIgnore]
	public User User { get; set; } = null!;
	[JsonIgnore] 
	public string WalletSecretKey { get; set; } = "";
}
