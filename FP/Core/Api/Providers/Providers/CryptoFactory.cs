using FP.Core.Api.Providers.Interfaces;

namespace FP.Core.Api.Providers.Providers;

public class CryptoFactory : ICryptoFactory
{
    private readonly IEnumerable<ICryptoApiProvider> _networks;
    public IEnumerable<ICryptoApiProvider> Networks { get => _networks; }
    
    public CryptoFactory(IEnumerable<ICryptoApiProvider> networks)
    {
        _networks = networks;
    }
    
    public ICryptoApiProvider GetNetwork(WalletType type)
    {
        return _networks.First(n => n.Type == type);
    }
}