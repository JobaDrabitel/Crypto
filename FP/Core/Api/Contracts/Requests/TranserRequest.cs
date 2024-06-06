using FP.Core.Api.ApiDto;

namespace FP.Core.Api.Contracts.Requests
{
	public class TranserRequest
	{
		public record TransferRequest(WalletDto From, WalletDto To, decimal Amount);
	}
}
