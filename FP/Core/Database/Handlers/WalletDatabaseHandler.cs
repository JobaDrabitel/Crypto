using FP.Core.Api.Providers;
using FP.Core.Api.Providers.Interfaces;
using FP.Core.Database.Models;
using Microsoft.EntityFrameworkCore;
using TronNet;

namespace FP.Core.Database.Handlers;

public class WalletDatabaseHandler
{
    private readonly ILogger<WalletDatabaseHandler> _logger;
    private readonly FpDbContext _dbContext;
    private readonly ICryptoFactory _networkFactory;

    public WalletDatabaseHandler(FpDbContext dbContext, ILogger<WalletDatabaseHandler> logger, ICryptoFactory networkFactory)
    {
        _dbContext = dbContext;
        _logger = logger;
        _networkFactory = networkFactory;
    }

	public async Task<Wallet> CreateTrc20Wallet(int userId)
	{
		_logger.LogInformation("Start to add wallet in database");

		Wallet wallet = new();
		var key = TronECKey.GenerateKey(TronNetwork.MainNet);
		var address = key.GetPublicAddress();

		if (address != null)
		{
			wallet.WalletAddress = address;
			wallet.WalletSecretKey = key.GetPrivateKey();
            wallet.UserId = userId;
            wallet.WalletType = WalletType.Trc20.ToString();
        }
        try
        {
            var result = await _dbContext.Wallets.AnyAsync(u => u.WalletAddress == wallet.WalletAddress);
            if (!result)
            {
                await _dbContext.Wallets.AddAsync(wallet);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Wallet created");
            }
            else
            {
                _logger.LogInformation($"Cannot create user with email {wallet.WalletAddress}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "Cannot create wallet");
        }

        return wallet;
    }

    public async Task<Wallet?> CreateEthWallet(WalletType type, int userId)
    {
        var network = _networkFactory.GetNetwork(type);
        var cryptoWallet = await network.CreateWallet();
        if (cryptoWallet is null) return null;
        
        var wallet = new Wallet
        {
            WalletAddress = cryptoWallet.Address,
            WalletSecretKey = cryptoWallet.PrivateKey,
            WalletType = type.ToString(),
            UserId = userId
        };
        
        try
        {
            var result = await _dbContext.Wallets.AnyAsync(u => u.WalletAddress == wallet.WalletAddress);
            if (!result)
            {
                await _dbContext.Wallets.AddAsync(wallet);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Wallet created");
            }
            else
            {
                _logger.LogInformation($"Cannot create user with wallet {wallet.WalletAddress}");
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "Cannot create wallet");
            return null;
        }

        return wallet;
    }
    
    public async Task<Wallet?> GetWallet(string walletAddress)
    {
        _logger.LogInformation("Start to finding wallet in database");

        try
        {
            return await _dbContext.Wallets.FirstOrDefaultAsync(u => u.WalletAddress == walletAddress);
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "Cannot find wallet");        
            return null;
        }
    }
}