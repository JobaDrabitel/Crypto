using FP.Core.Api.ApiDto;
using FP.Core.Api.Responses;
using FP.Core.Api.Services;
using FP.Core.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TronNet.ABI.Util;


namespace FP.Core.Database.Handlers;

public class InvestmentDatabaseHandler
{
	private readonly ILogger<InvestmentDatabaseHandler> _logger;
	private readonly FpDbContext _dbContext;
	private readonly OperationDatabaseHandler _operationDatabaseHandler;
	private readonly PromocodeDatabaseHandler _promocodeDatabaseHandler;
	public InvestmentDatabaseHandler(FpDbContext dbContext, ILogger<InvestmentDatabaseHandler> logger,
		OperationDatabaseHandler operationDatabaseHandler, PromocodeDatabaseHandler promocodeDatabaseHandler)
	{
		_dbContext = dbContext;
		_logger = logger;
		_operationDatabaseHandler = operationDatabaseHandler;
		_promocodeDatabaseHandler = promocodeDatabaseHandler;
	}

	public async Task<ReturnResponse> CreateInvestment(int userId, string code, int poolId)
	{
		try
		{
			var pool = await _dbContext.Pools.FirstOrDefaultAsync(p => p.Id == poolId);
			if (pool == null || pool.IsClosed)
				return new InvalidData("pool id");
			
			var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

			var sum = CodeService.GetCodeSum(code);

			if (sum == 0.0m && code.Length == 42)
				return new InvalidData("code");
			
			if (user == null)
				return new InvalidData("userId");
			
			if (!user.IsRegistrationEnded || !user.IsVerified)
				return new NotVerified();
			if(user.BalanceIncome < sum)
				return new InvalidData("Not enough money");
			if (await _promocodeDatabaseHandler.IsCodeFree(code) && !user.IsCodeUsed)
			{
				user.IsCodeUsed = true;
				return await CreatePromo(code, userId, sum);
			}
			
			if (!CodeService.IsCodeContains(code))
				return new InvalidData(code);
			
			Investment investment = new()
			{
				UserId = userId,
				Code = code,
				PoolId = poolId,
				TotalSum = sum,
				MaxSum = sum,
				TotalYield = 0
			};
			
			user.CurrentIncome +=  sum;
			user.BalanceIncome -= sum;

			var addedInvestment = await _dbContext.Investments.AddAsync(investment);
			var userRangHelper = new UpdateRang(_dbContext);
			await userRangHelper.UpdateLineIncome(sum, user.ReferrerCode);
			_dbContext.Users.Update(user);
			await _dbContext.SaveChangesAsync();
			CodeService.ActivateCode(code);
			
			_logger.LogInformation("Investment created");
			return new OkResponse<Investment?>(addedInvestment.Entity);
		}
		catch (Exception ex)
		{
			_logger.LogInformation("Cannot create pack");
			return new InternalErrorResponse();
		}

	}
	
	public async Task<ReturnResponse> AddPackToInvestment(string code, Pack pack, int userId)
	{
		try
		{
			var invest = await _dbContext.Investments
				.Include(i => i.User)
				.Include(i => i.Packs)
				.Include(i => i.Pool)
				.FirstOrDefaultAsync(x => x.Code == code);

			if (invest == null || invest.IsEnded)
				return new NotFoundResponse();
                
			if (invest.User.Id != userId)
				return new InvalidData("userId");
			
			if (!invest.User.IsRegistrationEnded || !invest.User.IsVerified)
				return new NotVerified();

			if (invest.Packs.Count != 0 && invest.Packs.Count >= invest.Pool.MaxPacksCount)
				return new InvalidData("Too many packs");
			if (invest.Packs.Count(pack => !pack.ByMaxDuration) > 3)
				return new InvalidData("Pack duration");

			pack.InvestmentId = invest.Id;
			
			invest.TotalYield += pack.Yield;
			invest.PacksCount++;
			invest.EndDate = invest.EndDate > pack.EndDate ?
				invest.EndDate :
				pack.EndDate;
			await _dbContext.Packs.AddAsync(pack);
			_dbContext.Investments.Update(invest);
			await _dbContext.SaveChangesAsync();
			
			return new OkResponse<Investment>(invest);
		}
		catch (Exception e)
		{
			return new InternalErrorResponse();
		}
	}
	
	public async Task<ReturnResponse> GetAllInvestments(int userId)
	{
		try
		{
			var invests = await _dbContext.Investments
				.Include(i=> i.Pool)
				.Include(i=>i.Packs)
				.Where(i => i.UserId == userId && !i.IsClosed).ToListAsync();
			
			return new OkResponse<List<Investment>>(invests);
		}
		catch 
		{
			return new InternalErrorResponse();
		}
	}

	public List<DefiTransactions> GetAllDefiTransactions(int id)
	{
		try
		{
			return _dbContext.DefiTransactions.Where(d => d.InvestId == id && !d.IsClosed).ToList();
		}
		catch (Exception e)
		{
			return new List<DefiTransactions>();
		}
	}

	public async Task<ReturnResponse> TransferBalanceToFound(int id, decimal amount)
	{
		try
		{
			var investment = await _dbContext.Investments.FirstOrDefaultAsync(i => i.Id == id);
			
			if (investment == null) return new NotFoundResponse();
			if (investment.TotalAccrual == 0) return new InvalidData("amount equal zero");
			if (investment.TotalAccrual < amount) return new InvalidData("invalid amount");

			var defi = new DefiTransactions()
			{
				Sum = amount,
				InvestId = investment.Id,
			};
			investment.TotalAccrual -= amount;
			_dbContext.Update(investment);
			await _dbContext.DefiTransactions.AddAsync(defi);
			await _dbContext.SaveChangesAsync();
			return new OkResponse<Investment>(investment);

		}
		catch (Exception e)
		{
			return new InternalErrorResponse();
		}
	}
	public async Task<ReturnResponse> TransferFromFound(int id)
	{
		try
		{
			var defi = await _dbContext.DefiTransactions.Include(d => d.Investment).FirstOrDefaultAsync(d => d.Id == id && !d.IsClosed);
			if (defi == null) return new NotFoundResponse();
			
			var investment = await _dbContext.Investments.FirstOrDefaultAsync(i => i.Id == defi.InvestId);
			if (investment == null) return new NotFoundResponse();


			investment.TotalAccrual += defi.Sum;
			defi.IsClosed = true;
			
			_dbContext.Update(defi);
			_dbContext.Update(investment);
			await _dbContext.SaveChangesAsync();
			
			return new OkResponse<Investment>(investment);

		}
		catch (Exception e)
		{
			return new InternalErrorResponse();
		}
	}

	public async Task<Investment?> GetInvestmentByCode(string code)
	{
		try
		{
			return await _dbContext.Investments.FirstOrDefaultAsync(i => i != null && i.Code == code);
		}
		catch (Exception e)
		{
			return null;
		}
	}

	public async Task<ReturnResponse> CloseInvest(int id, User user)
	{
		var investment = await _dbContext.Investments.FirstOrDefaultAsync(i => i.Id == id && i.IsEnded == true);
		if (investment is null)
			return new NotFoundResponse();
		foreach(var defi in investment.DefiTransactions)
		{
			investment.TotalAccrual += defi.Sum;
			defi.IsClosed = true;
		}
					
		if (investment.Code.Length == 42)
		{
			user.BalanceIncome += investment.TotalSum;
			user.CurrentIncome = user.CurrentIncome>=investment.TotalSum? user.CurrentIncome -= investment.TotalSum : 0;
		}
		user.BalanceIncome += investment.TotalAccrual;
		var rangHelper = new UpdateRang(_dbContext); 
		await rangHelper.UpdateLineIncome(-investment.TotalSum, user.ReferrerCode);
		investment.IsClosed = true;
		investment.TotalSum = 0;
		await _dbContext.SaveChangesAsync();
		return new OkResponse<Investment>(investment);
	}
	public ReturnResponse GetPools()
	{
		try
		{
			var pools = _dbContext.Pools.ToList();
			var response = pools.Select(pool => new PoolResponseDto(pool)).ToList();

			return new OkResponse<List<PoolResponseDto>>(response);
		}
		catch (Exception e)
		{
			return new InternalErrorResponse();
		}
	}

	public async Task<ReturnResponse> TransferBalanceToIncome(int userId, int investId)
	{
		try
		{
			var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
			var invest = await _dbContext.Investments.FirstOrDefaultAsync(i => i.Id == investId);

			if (invest == null)
			{
				return new NotFoundResponse();
			}

			if (user != null) user.BalanceIncome += invest.TotalAccrual;
			else return new NotFoundResponse();
			
			invest.TotalAccrual = 0;

			_dbContext.Users.Update(user);
			_dbContext.Investments.Update(invest);
			await _dbContext.SaveChangesAsync();

			return new OkResponse<User>(user);
		}
		catch (Exception e)
		{
			return new InternalErrorResponse();
		}
	}

	public async Task<ReturnResponse> CreatePromo(string code, int userId, decimal sum)
	{
		try
		{
			if (await _promocodeDatabaseHandler.IsCodeFree(code))
				return new InvalidData("code");

			var investment = new Investment
			{
				PoolId = 2,
				Code = code,
				UserId = userId,
				TotalSum = sum,
				TotalYield = 1,
				MaxPacksCount = 0,
				EndDate = DateTime.UtcNow.AddDays(7),
			};
			var user = await _dbContext.Users.FindAsync(userId);
			await _operationDatabaseHandler.CreateOperation(userId, investment.TotalSum / 10, OperationTypeEnum.TopUp.ToString(), (int)OperationTypeEnum.TopUp, user);
			//var referrer = await _dbContext.Users.FirstOrDefaultAsync(u => u.ReferralCode == user!.ReferrerCode);
			//referrer!.BalanceAgent += 10;
			//await _operationDatabaseHandler.CreateOperation(referrer.Id, 10, OperationTypeEnum.RefBonus.ToString(), (int)OperationTypeEnum.RefBonus, referrer);
			//_dbContext.Users.Update(referrer);
			await _dbContext.Investments.AddAsync(investment);
			await _dbContext.SaveChangesAsync();

			return new OkResponse<Investment>(investment);
		}
		catch (Exception e)
		{
			return new InternalErrorResponse();
		}
	}
}