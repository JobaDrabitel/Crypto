using FP.Core.Api.ApiDto;
using FP.Core.Api.Responses;
using FP.Core.Database.Models;

namespace FP.Core.Api.Providers.Interfaces;

public interface ICryptoApiProvider
{
	public Task<CryptoCreatedWallet?> CreateWallet();
	public WalletType Type { get; }

    public Task<decimal?> GetWalletBalance(string walletAddress);
    public Task<bool> TransferToken(Wallet fromWallet, Wallet toWallet, decimal amount);
	public Task<bool> TransferTokenNoFee(Wallet fromWallet, string toWalletaddress, decimal amount);
	public Task<bool> TransferGasToWallet(Wallet fromWallet, string toWalletAddress, decimal amount);
}