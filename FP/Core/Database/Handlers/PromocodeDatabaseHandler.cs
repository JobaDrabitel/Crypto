using FP.Core.Api.Services;
using FP.Core.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace FP.Core.Database.Handlers
{
	public class PromocodeDatabaseHandler
	{
		private readonly FpDbContext _dbContext;
		public PromocodeDatabaseHandler(FpDbContext dbContext)
		{
			_dbContext = dbContext;
		}
		public async Task<List<Promocode>> CreatePromocodeAsync(PromocodeType type, int count, int userId)
		{
			var promocodes = new List<Promocode>();
			var stringBuilder = new RandomStringBuilder();
			if (_dbContext.Promocodes.Count(p => p.UserId == userId) + count > 50) return promocodes;
			for (var i = 0; i < count; i++)
			{
				var code = "0x" + stringBuilder.Create(40) + "w";
				var promocode = new Promocode()
				{
					Code = code,
					CreationTime = DateTime.UtcNow,
					Type = type.ToString(),
					UserId = userId,
					DealSum = 100m
				};
				
				promocodes.Add(promocode);
			}

			_dbContext.Promocodes.AddRange(promocodes);
			await _dbContext.SaveChangesAsync();
			return promocodes;
		}

		public async Task<User?> GetCodeLeader(string code)
		{
			var promo = await _dbContext.Promocodes.Include(p => p.User).FirstOrDefaultAsync(p => p.Code == code && !p.IsActived);
			return promo?.User;
		}

		public async Task<List<Promocode>?> GetPromocodes(PromocodeType type, int userId)
		{
			try
			{
				var codes = _dbContext.Promocodes.Where(c => c.UserId == userId);
				return codes.ToList();
			}
			catch (Exception ex)
			{
				return null;
			}
		}
		public async Task<bool> IsCodeFree(string code)
		{
			try
			{
				var promocode = await _dbContext.Promocodes.FirstOrDefaultAsync(p => p.Code == code);
				if (promocode == null)
					return false;
				if (promocode.IsActived) return false;
				
				promocode.IsActived = true;
				return true;
			}
			catch 
			{
				return false; 
			}
		}

		public async Task<int> GetWeeklyCount(int userId)
		{
			try
			{
				return _dbContext.Promocodes.Count(c => c.UserId == userId);
			}
			catch (Exception e)
			{
				return 0;
			}
		}
	}
}
