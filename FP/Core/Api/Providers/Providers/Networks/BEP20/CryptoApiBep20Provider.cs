using System.Net;
using System.Text.Json;
using FP.Core.Api.Providers.Interfaces;
using FP.Core.Api.Responses;
using FP.Core.Database.Models;

namespace FP.Core.Api.Providers.Providers.Networks.BEP20;

public class CryptoApiBep20Provider : ICryptoApiProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);
    
    public CryptoApiBep20Provider(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<CryptoCreatedWallet?> CreateWallet()
    {
        var httpClient = _httpClientFactory.CreateClient("Eth");
        using var response = await httpClient.GetAsync("bep20/create_wallet");
        if (response.StatusCode != HttpStatusCode.OK) return null;
        var a = await response.Content.ReadAsStringAsync();
        return await response.Content.ReadFromJsonAsync<CryptoCreatedWallet>();
    }

    public WalletType Type => WalletType.Bep20;

    public async Task<decimal?> GetWalletBalance(string walletAddress)
    {
		var httpClient = _httpClientFactory.CreateClient("Eth");
		using var response = await httpClient.GetAsync($"bep20/get_balance_usdt/{walletAddress}");
		if (response.StatusCode != HttpStatusCode.OK) return null;
		var updated = await response.Content.ReadFromJsonAsync<CryptoWalletBalance>();
		return updated?.Balance;
	}

    public async Task<bool> TransferToken(Wallet fromWallet, Wallet toWallet, decimal amount)
    {
		if (await TransferGasToWallet(toWallet, fromWallet.WalletAddress, 0.001m))
			if (await TransferTokenNoFee(fromWallet, toWallet.WalletAddress, amount))
				return true;
		return false;
	}

    public async Task<bool> TransferTokenNoFee(Wallet fromWallet, string toWalletaddress, decimal amount)
    {
		try
		{
			var httpClient = _httpClientFactory.CreateClient("Eth");

			var transferRequest = new
			{
				from_address = fromWallet.WalletAddress,
				from_private_key = fromWallet.WalletSecretKey,
				to_address = toWalletaddress,
				amount
			};

			using var content = new StringContent(JsonSerializer.Serialize(transferRequest, Options));
			using var response = await httpClient.PostAsJsonAsync("bep20/transfer_usdt", transferRequest);
			return response.StatusCode == HttpStatusCode.OK;
		}
		catch (Exception ex)
		{
			return false;
		}
	}

    public async Task<bool> TransferGasToWallet(Wallet fromWallet, string toWalletAddress, decimal amount)
    {
		try
		{
			var httpClient = _httpClientFactory.CreateClient("Eth");

			var transferRequest = new
			{
				from_address = fromWallet.WalletAddress,
				from_private_key = fromWallet.WalletSecretKey,
				to_address = toWalletAddress,
				amount
			};

			using var response = await httpClient.PostAsJsonAsync("bep20/transfer_gas", transferRequest);
			return response.StatusCode == HttpStatusCode.OK;
		}
		catch (Exception ex)
		{
			return false;
		}
	}
}