using TronNet.Protocol;

namespace FP.Core.Api.Providers.Interfaces;

public interface ICryptoFactory
{
    ICryptoApiProvider GetNetwork(WalletType type);
	public IEnumerable<ICryptoApiProvider> Networks { get; }
}