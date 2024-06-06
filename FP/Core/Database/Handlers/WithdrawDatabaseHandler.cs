using FP.Core.Api.ApiDto;
using FP.Core.Api.Providers.Interfaces;
using FP.Core.Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using FP.Core.Api.Responses;
using TronNet.Protocol;
using FP.Core.Api.Services;
using FP.Core.Api.Providers;

namespace FP.Core.Database.Handlers
{
    public class WithdrawDatabaseHandler
	{
		private readonly ILogger<WalletDatabaseHandler> _logger;
		private readonly FpDbContext _dbContext;
		private readonly IServiceProvider _serviceProvider;
		private readonly ICryptoFactory _cryptoFactory;
		private readonly IConfiguration _config;
		private readonly OperationDatabaseHandler _operationDatabaseHandler;
		private readonly NotificationDatabaseHandler _notificationDatabaseHandler;

		public WithdrawDatabaseHandler(FpDbContext dbContext, IServiceProvider service, ILogger<WalletDatabaseHandler> logger,
			ICryptoFactory cryptoFactory, IConfiguration config, OperationDatabaseHandler operationDatabaseHandler, NotificationDatabaseHandler notificationDatabaseHandler)
		{
			_dbContext = dbContext;
			_serviceProvider = service;
			_logger = logger;
			_cryptoFactory = cryptoFactory;
			_config = config;
			_operationDatabaseHandler = operationDatabaseHandler;
			_notificationDatabaseHandler = notificationDatabaseHandler;	
		}

		public async Task<Withdraw?> CreateWithdraw(WithdrawDto withdrawDto, int userId)
		{
			_logger.LogInformation("Start to add withdraw in database");
			try
			{
				var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
				if (user == null)
					return null;
				var withdraw = new Withdraw
				{
					UserId = userId,
					Sum = withdrawDto.Sum,
					CreationTime = DateTime.UtcNow,
					WalletAddress = withdrawDto.WalletAddress,
					RealizationTime = null,
					Status = WithdrawStatusEnum.Waiting.ToString(),
					Type = withdrawDto.Type
				};
				await _dbContext.Withdraws.AddAsync(withdraw);
				await _dbContext.SaveChangesAsync();
				_logger.LogInformation("Wallet created");

				return withdraw;
			}
			catch (Exception ex)
			{
				_logger.LogInformation(ex, "Cannot create user");
			}

			return null;
		}

		public async Task<ReturnResponse> RejectById(int id)
		{
			try
			{
				var result = await _dbContext.Withdraws
					.Include(w => w.User)
					.FirstOrDefaultAsync(w => w.Id == id);

				if (result == null)
					return new InvalidData("WithdrawId");
				
				_dbContext.Withdraws.Remove(result);
				_dbContext.SaveChangesAsync();
				return new OkResponse<Withdraw>(result);
			}
			catch (Exception e)
			{
				return new InternalErrorResponse();
			}
		}
		
		public async Task<ReturnResponse> RealizeWithdraw(int id)
		{
			_logger.LogInformation("Start to find withdraw with {id}", id);

			try
			{
				var result = await _dbContext.Withdraws.Include(w => w.User.Wallets.First()).FirstOrDefaultAsync(w => id == w.Id);
				if (result != null)
				{
					var network = _cryptoFactory.GetNetwork((WalletType)Enum.Parse(typeof(WalletType), result.Type));
					var wallet = result.User.Wallets.First();
					if (wallet == null)  return new InvalidData("Withdraw wallet");
					var master = await _dbContext.Users
						.OrderBy(m => m.Id)
						.Include(u => u.Wallets.First())
						.FirstOrDefaultAsync();
					
					_logger.LogInformation("Transfer to user wallet from database");
					var res = await network.TransferGasToWallet(master.Wallets.First(), wallet.WalletAddress, 20);
                    
					if (!res)
						return new InvalidData("Bad request");
                    
					_logger.LogInformation("Transfer to user wallet");
					res = await network.TransferTokenNoFee(master.Wallets.First(), wallet.WalletAddress, result.Sum);
					if (!res)
						return new InvalidData("Bad Request");
					res = await network.TransferTokenNoFee(wallet, result.WalletAddress, result.Sum);
					if (!res)
						return new InvalidData("Bad request");
					result.RealizationTime = DateTime.UtcNow;
					result.Status = WithdrawStatusEnum.Realized.ToString();
					_dbContext.Update(result);
					await _dbContext.SaveChangesAsync();
					var response = await _operationDatabaseHandler.CreateOperation(result.UserId, result.Sum, "Withdraw", (int)OperationTypeEnum.Withdraw);
					var notify = new WithdrawNotify(result.UserId, result.Sum, result.User.BalanceIncome);
					await _notificationDatabaseHandler.CreateNotification(notify);
				}
				else
				{
					return new InvalidData("WithdrawId");
				}
				
				return new OkResponse<Withdraw>(result);
			}
			catch (Exception ex)
			{
				return new InternalErrorResponse();
			}

		}
		public async Task<List<Withdraw>> GetAllWithdraws()
		{
			_logger.LogInformation("Start to find all withdraws");
			return _dbContext.Withdraws.ToList();
		}
		public async Task<List<Withdraw>> GetUnrealizedWithdraws()
		{
			_logger.LogInformation("Start to find unrealized withdraws");
			return _dbContext.Withdraws.Where(w=> w.Status==WithdrawStatusEnum.Waiting.ToString()).ToList();
		}

		public async Task<ReturnResponse> GetUsersWaiting(int userId)
		{
			try
			{
				var result = _dbContext.Withdraws
					.Include(w => w.User)
					.Where(w => w.User.Id == userId && w.Status == WithdrawStatusEnum.Waiting.ToString())
					.ToList();

				return new OkResponse<List<Withdraw>>(result);
			}
			catch (Exception e)
			{
				return new InternalErrorResponse();
			}
		}
	}
}
