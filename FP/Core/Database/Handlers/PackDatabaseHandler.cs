using FP.Core.Api.ApiDto;
using FP.Core.Api.Controllers;
using FP.Core.Api.Services;
using FP.Core.Api.Responses;
using FP.Core.Database.Models;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Utilities.Collections;

namespace FP.Core.Database.Handlers
{
	public class PackDatabaseHandler
	{
		private readonly ILogger<UserController> _logger;
		private readonly FpDbContext _dbContext;
		private readonly InvestmentDatabaseHandler _investmentDatabaseHandler;
		private readonly UserDatabaseHandler _userDatabaseHandler;

		public PackDatabaseHandler(FpDbContext dbContext, ILogger<UserController> logger,
			InvestmentDatabaseHandler investmentDatabaseHandler, UserDatabaseHandler userDatabaseHandler)
		{
			_dbContext = dbContext;
			_logger = logger;
			_investmentDatabaseHandler = investmentDatabaseHandler;
			_userDatabaseHandler = userDatabaseHandler;
		}

		public async Task<ReturnResponse> CreatePacks(PackDto packInfo, int userId)
		{
			ReturnResponse response;
			try
			{
				var packs = new List<Pack>();

				if (packInfo.PackDtos == null)
					return new InvalidData("Pack dto");
				
				foreach (var packDto in packInfo.PackDtos)
				{
					var packType = await _dbContext.PackTypes.FirstOrDefaultAsync(p => p.Id == packDto.PackTypeId);
					if (packType == null) return new InvalidData("PackTypeId");
					
					Pack pack = new()
					{
						EndDate = packDto.IsMaxEndDate ? DateTime.UtcNow.AddDays(packType.MaxDuration) : DateTime.UtcNow.AddDays(packType.MinDuration),
						PackTypeId = packDto.PackTypeId,
						ByMaxDuration = packDto.IsMaxEndDate,
						Yield = packType.Yield,
						CurrentYield = packType.Yield
					};
					
					packs.Add(pack);
					
					var user = await _userDatabaseHandler.GetUserById(userId) as OkResponse<User>;
					if (user is not { Status: true })
						return new InvalidData("userId");
				
					var res = await _investmentDatabaseHandler.AddPackToInvestment(packInfo.InvestmentCode, pack, userId);
					
					if (res is not OkResponse<Investment> invest)
						return new InvalidData("Investment code");

					if(invest.ObjectData?.MaxPacksCount == 0)
					{
						invest.ObjectData.MaxPacksCount = packInfo.PackDtos.Length;
						_dbContext.Update(invest.ObjectData);
					}
					
					packType.ActiveSum +=  invest.ObjectData!.TotalSum / packInfo.PackDtos.Length;
					_dbContext.Update(user.ObjectData!);
					await _dbContext.SaveChangesAsync();
				}

				_logger.LogInformation("Packs created");
				response = new OkResponse<List<Pack>>(packs);
			}
			catch (Exception ex)
			{
				_logger.LogInformation("Cannot create packs");
				response = new InternalErrorResponse();
			}

			return response;
		}
		public async Task<ReturnResponse> ReactivatePack(ReactivatePackDto packDto, int userId)
		{
			ReturnResponse response;
			try
			{
				var investment = await _dbContext.Investments.Include(p => p.Packs)
					.FirstOrDefaultAsync(i => i != null && i.Code == packDto.InvestmentCode);
				if (investment == null)
					return new InvalidData("Investment code");
				if (investment.PacksCount > investment.MaxPacksCount)
					return new InvalidData("Packs count");

				if (packDto.IsMaxEndDate &&  investment.Packs.Where(p=>p.HasLastAccrual==false).Count(p => p.ByMaxDuration==false) >= 3)
					return new InvalidData("Pack duration");

				var packType = await _dbContext.PackTypes.FirstOrDefaultAsync(p => p.Id == packDto.PackTypeId);
				if (packType == null)
					return new InvalidData("PackTypeId");

				var pack = new Pack
				{
					PackTypeId = packDto.PackTypeId,
					InvestmentId = investment.Id,
					EndDate = packDto.IsMaxEndDate ? DateTime.UtcNow.AddDays(packType.MaxDuration) : DateTime.UtcNow.AddDays(packType.MinDuration),
					Yield = packType.Yield,
					ByMaxDuration = packDto.IsMaxEndDate
				};

				var duration = packDto.IsMaxEndDate ? packType.MaxDuration : packType.MinDuration;

				investment.EndDate = investment.EndDate < DateTime.UtcNow.AddDays(duration)
					? DateTime.UtcNow.AddDays(duration)
					: investment.EndDate;
				
				await _dbContext.Packs.AddAsync(pack);
				investment.TotalYield += packType.Yield;
				investment.PacksCount = investment.Packs.Where(p=>p.HasLastAccrual == false).Count();
				_dbContext.Investments.Update(investment);
				await _dbContext.SaveChangesAsync();
				
				return new OkResponse<Pack>(pack);
			}
			catch (Exception exception)
			{
				response = new InternalErrorResponse();
			}

			return response;
		}
		
		public async Task<ReturnResponse> GetPacksByInvestId(int investId)
		{
			ReturnResponse response;
			try
			{
				var packs = _dbContext.Packs.Where(p => p.InvestmentId == investId).ToList();
				if (packs != null)
				{
					response = new OkResponse<List<Pack>>(packs);
				}
				else
				{
					response = new NotFoundResponse();
				}
			}
			catch (Exception ex)
			{
				response = new InternalErrorResponse();
			}

			return response;
		}
		public async Task<ReturnResponse> GetPacksByInvestCode(string investCode)
		{
			ReturnResponse response;
			try
			{
				var packs = _dbContext.Packs
					.Include(p => p.PackType)
					.Include(p => p.Investment)
					.Where(p => p.Investment.Code == investCode)
					.ToList();
				response = new OkResponse<List<Pack>>(packs);
			}
			catch (Exception ex)
			{
				response = new InternalErrorResponse();
			}

			return response;
		}
		public async Task<List<PackType>> GetAllPackTypes()
		{
			try
			{
				return _dbContext.PackTypes.ToList();
			}
			catch
			{
				return null;
			}
		}
	}
}
