using FP.Core.Api.ApiDto;
using FP.Core.Api.Providers;
using FP.Core.Api.Providers.Interfaces;
using FP.Core.Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FP.Core.Api.Responses;
using FP.Core.Database.Models.ResponseDTO;
using FP.Core.Api.Services;


namespace FP.Core.Database.Handlers;

public class UserDatabaseHandler
{
	private readonly ILogger<UserDatabaseHandler> _logger;
	private readonly FpDbContext _dbContext;
	private readonly IServiceProvider _serviceProvider;
	private readonly WalletDatabaseHandler _walletDatabaseHandler;
	private readonly ICryptoFactory _cryptoFactory;
	private readonly ReferralDatabaseHandler _referralDatabaseHandler;
	private readonly OperationDatabaseHandler _operationDatabaseHandler;
	private readonly InvestmentDatabaseHandler _investmentDatabaseHandler;
	private readonly VerificationCodeDatabaseHandler _verificationCodeDatabaseHandler;
	private readonly NotificationDatabaseHandler _notificationDatabaseHandler;

	public UserDatabaseHandler(FpDbContext dbContext, WalletDatabaseHandler walletDatabaseHandler, IServiceProvider service,
		ILogger<UserDatabaseHandler> logger, ICryptoFactory cryptoFactory, 
		OperationDatabaseHandler operationDatabaseHandler, InvestmentDatabaseHandler investmentDatabaseHandler,
		VerificationCodeDatabaseHandler verificationCodeDatabaseHandler , NotificationDatabaseHandler notificationDatabaseHandler)
	{
		_dbContext = dbContext;
		_serviceProvider = service;
		_logger = logger;
		_walletDatabaseHandler = walletDatabaseHandler;
		_cryptoFactory = cryptoFactory;
		_referralDatabaseHandler = new ReferralDatabaseHandler(dbContext, this, notificationDatabaseHandler, logger);
		_operationDatabaseHandler = operationDatabaseHandler;
		_investmentDatabaseHandler = investmentDatabaseHandler;
		_verificationCodeDatabaseHandler = verificationCodeDatabaseHandler;
		_notificationDatabaseHandler = notificationDatabaseHandler;
	}

	public async Task<ReturnResponse> CreateUser(UserCreateDto userCreateData)
	{
		_logger.LogInformation($"Start to add user in database {userCreateData}");

		ReturnResponse response;
		var hasher = _serviceProvider.GetRequiredService<IPasswordHasher<User>>();
		RandomStringBuilder stringBuilder = new();

		try
		{
			User user = new()
			{
				ReferrerCode = userCreateData.ReferrerCode,
				ReferralCode = "0x" + stringBuilder.Create(40),
				BalanceIncome = 0m,
				BalanceCrypto = 0m,
				BalanceAgent = 0m,
			};
		
			if (userCreateData.IsEmail)
				user.Email = userCreateData.Login;
			else
				user.Telegram = userCreateData.Login;
		
			user.Passwordhash = hasher.HashPassword(user, userCreateData.Password);
			var referrer = await GetUserByReferralCode(userCreateData.ReferrerCode);
			if (!referrer.Status)
				return new InvalidData("Referrer code");
			if (userCreateData.IsEmail)
			{
				var result = await _dbContext.Users.AnyAsync(u => u.Email == user.Email);
				if (!result)
				{
					var addedUser = await _dbContext.Users.AddAsync(user);

					await _dbContext.SaveChangesAsync();
					var walletTrc20 = await _walletDatabaseHandler.CreateTrc20Wallet(addedUser.Entity.Id);
					var walletBep20 = await _walletDatabaseHandler.CreateEthWallet(WalletType.Bep20, addedUser.Entity.Id);
					var walletEth20 = await _walletDatabaseHandler.CreateEthWallet(WalletType.Erc20, addedUser.Entity.Id);
					
					var res = await _referralDatabaseHandler.CreateReferralTree(addedUser.Entity);
					if (!res)
						return new InvalidData("referral tree");
					_logger.LogInformation("User created");
					response = new OkResponse<User>(addedUser.Entity);
				}
				else
				{
					_logger.LogInformation("Cannot create user with email {Email}", user.Email);
					response = new InvalidData("email");
				}
			}
			else
			{
				var result = await _dbContext.Users.AnyAsync(u => u.Telegram == user.Telegram);
				if (!result)
				{
					var addedUser = await _dbContext.Users.AddAsync(user);
					await _dbContext.SaveChangesAsync();
					var res = await _referralDatabaseHandler.CreateReferralTree(addedUser.Entity);
					if (!res)
						return new InvalidData("referral tree");
					_logger.LogInformation("User created");
					response = new OkResponse<User>(addedUser.Entity);
				}
				else
				{
					_logger.LogInformation("Cannot create user with email {Email}", user.Email);
					response = new InvalidData("telegram");
				}
			}
		}
		catch (Exception ex)
		{
			response = new InternalErrorResponse();
			_logger.LogInformation(ex, "Cannot create user");
		}
		return response;
	}

	public async Task<ReturnResponse> UpdateUser(int userId, UserUpdateDto data)
	{
		try
		{
			var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

			if (user == null)
				return new InvalidData("userId");
			
			if (string.IsNullOrEmpty(data.Avatar))
				user.Avatar = null;
			else
				user.Avatar = data.Avatar;

			if (data.Name != null)
				user.Name = data.Name;

			if (data.Surname != null)
				user.Surname = data.Surname;

			if (data.Nickname != null)
				user.Nickname = data.Nickname;

			if (data.Country != null)
				user.Country = data.Country;

			if (data.City != null)
				user.City = data.City;

			if (data.Phone != null)
				user.Phone = data.Phone;

			if (data.Telegram != null)
				user.Telegram = data.Telegram;

			if (data.Email != null)
				user.Email = data.Email;
			
			user.ShowEmail = data.ShowEmail;
			user.ShowTg = data.ShowTg;
			user.ShowPhone = data.ShowPhone;
			user.IsRegistrationEnded = true;

			_dbContext.Update(user);
			await _dbContext.SaveChangesAsync();

			return new OkResponse<User>(user);
		}
		catch (Exception e)
		{
			return new InternalErrorResponse();
		}
	}

	public async Task<ReturnResponse> WithdrawInternal(int userId, decimal sum)
	{
		try
		{
			var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
			if (user == null || user.BalanceIncome < sum)
				return new InvalidData("userId or sum");
			user.BalanceIncome -= sum;
			_dbContext.Update(user);
			await _dbContext.SaveChangesAsync();
			return new OkResponse<User>(user);
		}
		catch 
		{
			return new InternalErrorResponse();
		}
	}
	
	public async Task<ReturnResponse> WithdrawCrypto(int userId, decimal sum)
	{
		try
		{
			var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
			if (user == null || user.BalanceCrypto < sum)
				return new InvalidData("userId or sum");
			user.BalanceCrypto -= sum;
			_dbContext.Update(user);
			await _dbContext.SaveChangesAsync();
			return new OkResponse<User>(user);
		}
		catch 
		{
			return new InternalErrorResponse();
		}
	}
	
	public async Task<ReturnResponse> WithdrawAgent(int userId, decimal sum)
	{
		try
		{
			var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
			if (user == null || user.BalanceAgent < sum)
				return new InvalidData("userId or sum");
			user.BalanceAgent -= sum;
			_dbContext.Update(user);
			await _dbContext.SaveChangesAsync();
			return new OkResponse<User>(user);
		}
		catch 
		{
			return new InternalErrorResponse();
		}
	}

	public async Task<ReturnResponse> GetUserByReferralCode(string code)
	{
		try
		{
			var users = _dbContext.Users.ToList();
			var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.ReferralCode == code);
			return user == null ? new NotFoundResponse() : new OkResponse<User>(user);
		}
		catch (Exception e)
		{
			return new InternalErrorResponse();
		}
	}

	public async Task<ReturnResponse> GetMainTeamGood(int userId)
	{
		try
		{
			var refs = _dbContext.Referrals
				.Include(r => r.Ref)
				.Where(r => r.ReferrerId == userId && r.Inline == 1).ToList();
			if (refs.Count == 0)
				return new OkResponse<decimal>(0);
			var maxLineIncome = refs.Max(r => r.Ref.LinesIncome + r.Ref.CurrentIncome);

			return new OkResponse<decimal>(maxLineIncome);
		}
		catch (Exception e)
		{
			return new InternalErrorResponse();
		}
	}
	
	public async Task<ReturnResponse> LoginUser(LoginDto userData)
	{
		_logger.LogInformation("Start to find user in database {userData}", userData.Login);

		ReturnResponse response;
		var hasher = _serviceProvider.GetRequiredService<IPasswordHasher<User>>();
		try
		{
			var result = await _dbContext.Users
				.Include(u => u.Wallets)
				.FirstOrDefaultAsync(u => u.Email == userData.Login || u.Telegram == userData.Login);
			if (result != null)
			{
				if (hasher.VerifyHashedPassword(result, result.Passwordhash, userData.Password) == PasswordVerificationResult.Success)
				{
					_logger.LogInformation("User found");
					result.LastActivityTime = DateTime.UtcNow;
					_dbContext.Update(result);
					await _dbContext.SaveChangesAsync();
					response = new OkResponse<User>(result);
				}
				else
				{
					response = new InvalidData("password");
				}
			}
			else
			{
				response = new InvalidData("email");
			}
		}
		catch (Exception ex)
		{
			_logger.LogInformation(ex, "Cannot create user");
			response = new InternalErrorResponse();
		}
		return response;

	}
	public async Task<ReturnResponse> GetUserWalletsById(int userId)
	{
		try
		{
			_logger.LogInformation("Start to find user in database {userId}", userId);
			var user = await _dbContext.Users.Include(u => u.Wallets).FirstOrDefaultAsync(u => userId == u.Id);
			if (user == null) 
			{ 
				return new NotFoundResponse(); 
			}
			return new OkResponse<ICollection<Wallet>>(user.Wallets);
		}
		catch
		{
			return new InternalErrorResponse();
		}
	}

	public async Task<ReturnResponse> ConfirmEmailAsync(int userId)
	{
		try
		{
			var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
			
			if (user == null)
				return new InvalidData("userId");
			
			user.IsVerified = true;
			_dbContext.Update(user);
			await _dbContext.SaveChangesAsync();
			return new OkResponse<User>(user);
		}
		catch (Exception e)
		{
			return new InternalErrorResponse();
		}
	}

	public async Task<ReturnResponse> GetUserById(int userId)
	{
		try
		{
			var user = await _dbContext.Users
				.Include(u => u.Wallets)
				.FirstOrDefaultAsync(u => u.Id == userId);

			return user == null ? new InvalidData("userId") : new OkResponse<User>(user);
		}
		catch (Exception e)
		{
			_logger.LogInformation(e, "Cannot find user");

			return new InternalErrorResponse();
		}
	}
	public async Task<ReturnResponse> UserTopUpInternal(int userId, decimal amount)
	{
		try
		{
			var user = await _dbContext.Users.FirstOrDefaultAsync<User>(u => u.Id == userId);
			user.BalanceIncome += amount;
			_dbContext.Users.Update(user);
			await _dbContext.SaveChangesAsync();
			return new OkResponse<User>(user);
		}
		catch (Exception e) 
		{
			return new InternalErrorResponse(); 
		}
	}
	public async Task<ReturnResponse> UserTopUpAgent(int userId, decimal amount)
	{
		try
		{
			var user = await _dbContext.Users.FirstOrDefaultAsync<User>(u => u.Id == userId);
			user.BalanceAgent += amount;
			_dbContext.Users.Update(user);
			await _dbContext.SaveChangesAsync();
			return new OkResponse<User>(user);
		}
		catch (Exception e) 
		{
			return new InternalErrorResponse(); 
		}
	}

	public async Task<decimal?> CheckWalletsBalance(int userId)
	{
		try
		{
			var user = await _dbContext.Users
				.Include(u => u.Investments)
				.Include(u => u.Packs)
				.Include(user => user.Wallets)
				.FirstOrDefaultAsync(u => u.Id == userId);
			var masterWallets = _dbContext.Users
							   .Include(u => u.Wallets)
							   .FirstOrDefault(u => u.Id == 1) 
							   ?.Wallets.ToList(); 
			var wallets = _dbContext.Wallets.Where(w => w.UserId == user.Id).ToList();
			decimal balance = 0;
			var copiedBalance = 0.0m;
			foreach (var network in _cryptoFactory.Networks)
			{
				balance += await network.GetWalletBalance(wallets.FirstOrDefault(w=>w.WalletType == network.Type.ToString()).WalletAddress) ?? 0.0m;
				if (balance is > 0)
				{
					copiedBalance += balance;
					var res = await network.TransferToken(user.Wallets.FirstOrDefault(w => w.WalletType == network.Type.ToString()),
						masterWallets.FirstOrDefault(w=>w.WalletType==network.Type.ToString()), balance);
					if (res)
						return null;
				}
			}
				user!.BalanceCrypto += copiedBalance;
				var notify = new TopUpNotify(user.Id, copiedBalance);
				await _notificationDatabaseHandler.CreateNotification(notify);
				_dbContext.Users.Update(user);
				await _dbContext.SaveChangesAsync();

				return balance;
			 
		}
		catch (Exception e)
		{
			return null;
		}
	}
	
	public async Task<ReturnResponse> TransferAsync(Transaction transaction)
	{
		ReturnResponse response;
		try
		{
			if (!transaction.FromAgent)
			{
				response = await WithdrawInternal(transaction.FromUserId, transaction.DealSum);
				if (!response.Status) return new InvalidData("fromInternal to Agent");
				
				response = await UserTopUpInternal(transaction.ToUserId, transaction.DealSum);
				await _operationDatabaseHandler.CreateOperation(transaction.FromUserId, transaction.DealSum,
					"Transfer", (int)OperationTypeEnum.Withdraw, transaction.ToUser);
				await _operationDatabaseHandler.CreateOperation(transaction.ToUserId, transaction.DealSum,
					"Transfer", (int)OperationTypeEnum.Accrual, transaction.FromUser);
				return response;
			}

			if (transaction.ToAgent)
			{
				response = await WithdrawAgent(transaction.FromUserId, transaction.DealSum);
				if (response.Status)
					response = await UserTopUpAgent(transaction.ToUserId, transaction.DealSum);
			}
			else
			{
				response = await WithdrawAgent(transaction.FromUserId, transaction.DealSum);
				if (response.Status)
					response = await UserTopUpInternal(transaction.ToUserId, transaction.DealSum);
			}

			await _operationDatabaseHandler.CreateOperation(transaction.FromUserId, transaction.DealSum, "Transfer",
				(int)OperationTypeEnum.Withdraw, transaction.ToUser);
			await _operationDatabaseHandler.CreateOperation(transaction.ToUserId, transaction.DealSum, "Transfer",
				(int)OperationTypeEnum.Accrual, transaction.FromUser);
			return response;
		}
		catch
		{
			return new InternalErrorResponse();
		}
	}

	public async Task<ReturnResponse> GetReferralStructure(int userId)
	{
		var infos = new List<ReferralTreeResponseDto>();
		for (var i = 0; i < 11; i++)
		{
			var result = await GetReferralLineInfo(userId, i + 1);
			
			if(result.Status)
				infos.Add(((OkResponse<ReferralTreeResponseDto>)result)?.ObjectData!);
			else
				break;
		}

		return  new OkResponse<List<ReferralTreeResponseDto>>(infos);
	}

	public async Task<ReturnResponse> GetLineFullInfo(int userId, int line)
	{
		try
		{
			var referrals = _dbContext.Referrals
				.Include(u => u.Referrer)
				.Include(referral => referral.Ref)
				.Where(r => r.ReferrerId == userId && r.Inline == line)
				.ToList();

			if (referrals.Count == 0)
				return new NotFoundResponse();
			
			var users = referrals.Select(u => u.Ref).Where(u => u is
			{
				IsRegistrationEnded: true, 
				IsVerified: true
			}).ToList();
			var usersInLine = new List<ReferralLineResponseDto>();

			foreach (var user in users)
			{
				var response = new ReferralLineResponseDto();
				var userInvestment = await _investmentDatabaseHandler.GetAllInvestments(user.Id);

				if (userInvestment.Status)
				{
					response.ActivePacks = (userInvestment as OkResponse<List<Investment>>).ObjectData.Sum(i => i.TotalSum);
				}
				
				var operations = _dbContext.Operations
					.Where(o => o.PartnerId == user.Id && o.UserId == userId && o.OperationTypeId  == (int)OperationTypeEnum.RefBonus)
					.ToList();

				
				response.MyIncome = operations.Sum(o => o.Sum);
				response.Income = user.TotalIncome;
				response.UsersCount = _dbContext.Referrals.Count(u => u.Referrer == user);
				response.User = user;
				
				usersInLine.Add(response);
			}

			return new OkResponse<List<ReferralLineResponseDto>>(usersInLine);
		}
		catch (Exception e)
		{
			return new InternalErrorResponse();
		}
	}

	private async Task<ReturnResponse> GetReferralLineInfo(int userId, int line)
	{
		try
		{
			var referrals = _dbContext.Referrals
				.Include(u => u.Referrer)
				.Include(referral => referral.Ref)
				.Where(r => r.ReferrerId == userId && r.Inline == line)
				.ToList();

			if (referrals?.Count == 0)
				return new NotFoundResponse();
			
			var users = referrals.Select(u => u.Ref);
			var response = new ReferralTreeResponseDto();

			foreach (var user in users)
			{
				var userInvestment = await _investmentDatabaseHandler.GetAllInvestments(user.Id);

				if (userInvestment.Status)
				{
					response.ActivePacks += (userInvestment as OkResponse<List<Investment>>).ObjectData.Sum(i => i.TotalSum);
				}
				
				var operations = _dbContext.Operations
					.Where(o => o.PartnerId == user.Id && o.UserId == userId && o.OperationTypeId  == (int)OperationTypeEnum.RefBonus)
					.ToList();
				
				response.MyIncome += operations.Sum(o => o.Sum);
				response.Income += user.TotalIncome;
			}

			response.UsersCount = users.Count(u => u.IsRegistrationEnded && u.IsVerified);

			return new OkResponse<ReferralTreeResponseDto>(response);
		}
		catch (Exception e)
		{
			return new InvalidData("userId");
		}
	}
	public async Task<ReturnResponse> AddRecoveryCode(RecoveryDto recoveryDto)
	{
		try
		{
			var args = recoveryDto.Url.Split('=');
			var email = args[1].Split('&');
			var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email[0]);
			if (user == null)
				return new NotFoundResponse();
			var genCode = Random.Shared.Next(100000, 999999);
			var code = await _verificationCodeDatabaseHandler.Create(genCode.ToString(), user.Id);
			
			return code;
		}
		catch 
		{
			return new InternalErrorResponse(); 
		}
		
	}
	public async Task<ReturnResponse> RecoverPassword(PasswordRecoveryDto passwordRecoveryDto)
	{
		try
		{
			var hasher = _serviceProvider.GetRequiredService<IPasswordHasher<User>>();
			var result = await _verificationCodeDatabaseHandler.FindCode(passwordRecoveryDto.Code);
			if(!result.Status)
				return new InvalidData("Code");
			var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == passwordRecoveryDto.Email);
			if(user == null)
				return new NotFoundResponse();
			user.Passwordhash = hasher.HashPassword(user, passwordRecoveryDto.Password);
			_dbContext.Update(user);
			await _dbContext.SaveChangesAsync();
			return new OkResponse<User>(user);
		}
		catch
		{
			return new InternalErrorResponse();
		}
	}
	public async Task<ReturnResponse> VerifyTelegramUser(string tg, long chatId)
	{
		try
		{
			if (_dbContext.Users.Any(u => u.TelegramId == chatId))
				return new InvalidData("chatId already exist");
			var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Telegram == '@' + tg);
			if (user == null)
				return new NotFoundResponse();
			user.IsVerified = true;
			user.TelegramId = chatId;
			_dbContext.Update(user);
			await _dbContext.SaveChangesAsync();
			return new OkResponse<User>(user);
		}
		catch
		{
			return new InternalErrorResponse();
		}
	}
	public async Task<ReturnResponse> ChangePassword(int userId, string password)
	{
		var hasher = _serviceProvider.GetRequiredService<IPasswordHasher<User>>();
		RandomStringBuilder stringBuilder = new();
		User user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
		if (user == null)
			return new NotFoundResponse();
		user.Passwordhash = hasher.HashPassword(user, password);
		_dbContext.Update(user);
		await _dbContext.SaveChangesAsync();
		return new OkResponse<User>(user);
	}
	public async Task<ReturnResponse> GetReferralInfo(int referrerId, int referralId)
	{
		var refInfo = await _dbContext.Referrals
			.Include(r=>r.Ref)
			.Include(r=>r.Referrer)
			.Where(r=> r.ReferrerId==referrerId && r.RefId==referralId)
			.FirstOrDefaultAsync();
		if (refInfo == null)
			return new NotFoundResponse();
		return new OkResponse<Referral>(refInfo);
	}

	public async Task<ReturnResponse> GetUserByEmail(string email)
	{
		try
		{
			var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
			return user == null ? new InvalidData("email") : new OkResponse<User>(user);
		}
		catch (Exception e)
		{
			return new InternalErrorResponse();
		}
	}
	public async Task<ReturnResponse> SetLeader(int id)
	{
		try
		{
			var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
			if(user==null) return new NotFoundResponse();
			user.IsLeader = !user.IsLeader;
			_dbContext.Update(user);
			await _dbContext.SaveChangesAsync();
			return new OkResponse<User>(user);
		}
		catch(Exception e) 
		{
			return new InternalErrorResponse();
		}
	}

	public async Task<ReturnResponse> TransferUSDTtoTST(int userId, decimal amount)
	{
		try
		{
			var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
			if (user == null)
				return new NotFoundResponse();

			if (user.BalanceCrypto < amount)
				return new InvalidData("USDT amount");

			user.BalanceCrypto -= amount;
			const decimal commission = 0.01m;
			user.BalanceIncome += amount * (1.0m - commission);

			_dbContext.Users.Update(user);
			await _dbContext.SaveChangesAsync();
			
			return new OkResponse<User>(user);
		}
		catch (Exception e)
		{
			return new InternalErrorResponse();
		}
	}
	
	public async Task<ReturnResponse> TransferTSTtoUSDT(int userId, decimal amount, bool isAgent)
	{
		try
		{
			var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
			if (user == null)
				return new NotFoundResponse();

			var result = isAgent ? await WithdrawAgent(userId, amount) : await WithdrawInternal(userId, amount);

			if (!result.Status)
				return result;

			const decimal commission = 0.00m;
			user.BalanceCrypto += amount * (1.0m - commission);

			_dbContext.Users.Update(user);
			await _dbContext.SaveChangesAsync();

			return new OkResponse<User>(user);
		}
		catch (Exception e)
		{
			return new InternalErrorResponse();
		}
	}
}
