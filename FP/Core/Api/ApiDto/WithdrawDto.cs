using FP.Core.Api.Providers;

namespace FP.Core.Api.ApiDto
{
	public class WithdrawDto
	{
		public decimal Sum { get; set; }
		public string WalletAddress { get; set; } = string.Empty;
		public string Type { get; set; } = WalletType.Trc20.ToString();
	}
}
