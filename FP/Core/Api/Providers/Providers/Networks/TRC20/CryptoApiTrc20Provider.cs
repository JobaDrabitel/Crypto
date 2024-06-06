using FP.Core.Api.ApiDto;
using FP.Core.Api.Providers.Interfaces;
using FP.Core.Api.Responses;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Text.Json;
using FP.Core.Database.Handlers;
using FP.Core.Database.Models;

namespace FP.Core.Api.Providers.Providers.Networks.TRC20
{
	public class CryptoApiTrc20Provider : ICryptoApiProvider
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

		public CryptoApiTrc20Provider(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}
		
		public async Task<CryptoCreatedWallet?> CreateWallet()
		{
			var httpClient = _httpClientFactory.CreateClient("Crypto");
			using var response = await httpClient.PostAsync("create_trc20_wallet", null);
			if (response.StatusCode != HttpStatusCode.OK) return null;
			return await response.Content.ReadFromJsonAsync<CryptoCreatedWallet>();
		}

		public WalletType Type => WalletType.Trc20;

		public async Task<decimal?> GetWalletBalance(string walletAddress)
		{
			var httpClient = _httpClientFactory.CreateClient("Crypto");
			using var response = await httpClient.GetAsync($"trc20_balance/{walletAddress}");
			if (response.StatusCode != HttpStatusCode.OK) return null;
			var updated = await response.Content.ReadFromJsonAsync<CryptoWalletBalance>();
			return updated?.Balance;
		}
		
		public async Task<bool> TransferToken(Wallet fromWallet, Wallet toWallet, decimal amount)
		{
			try
			{
				var httpClient = _httpClientFactory.CreateClient("Crypto");

				var transferRequest = new
				{
					from = new { address = fromWallet.WalletAddress, privateKey = fromWallet.WalletSecretKey },
					to = new { address = toWallet.WalletAddress, privateKey = toWallet.WalletSecretKey },
					amount
				};

				using var content = new StringContent(JsonSerializer.Serialize(transferRequest, Options));
				using var response = await httpClient.PostAsync("transfer_trc20", content);
				return await response.Content.ReadFromJsonAsync<bool>();
			}
			catch (Exception ex)
			{
				return false;
			}
		}
		
		public async Task<bool> TransferTokenNoFee(Wallet fromWallet, string toWalletaddress, decimal amount)
		{
			try
			{
				var httpClient = _httpClientFactory.CreateClient("Crypto");

				var transferRequest = new
				{
					from = new { address = fromWallet.WalletAddress, privateKey = fromWallet.WalletSecretKey },
					to = new {address = toWalletaddress},
					amount
				};
				using var content = new StringContent(JsonSerializer.Serialize(transferRequest, Options));
				using var response = await httpClient.PostAsync("transfer_without_fee_trc20", content);
				return response.IsSuccessStatusCode;
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
				var httpClient = _httpClientFactory.CreateClient("Crypto");

				var transferRequest = new
				{
					from = new { address = fromWallet.WalletAddress, privateKey = fromWallet.WalletSecretKey },
					to = new {address = toWalletAddress},
					amount
				};
				using var content = new StringContent(JsonSerializer.Serialize(transferRequest, Options));
				using var response = await httpClient.PostAsync("transfer_commission_trc20", content);
				return response.IsSuccessStatusCode;
			}
			catch (Exception ex)
			{
				return false;
			}
		}
	}
}
