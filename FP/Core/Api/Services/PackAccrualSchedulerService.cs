using FP.Core.Api.Services;
using FP.Core.Database;
using FP.Core.Database.Handlers;
using FP.Core.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

namespace FP.Core.Api.Services
{
	public class PackAccrualSchedulerService : BackgroundService
	{
		private readonly IDbContextFactory<FpDbContext> _dbContextFactory;
		private readonly ILogger<PackAccrualSchedulerService> _logger;

		public PackAccrualSchedulerService(IDbContextFactory<FpDbContext> dbContextFactory, 
			ILogger<PackAccrualSchedulerService> logger)
		{
			_dbContextFactory = dbContextFactory;
			_logger = logger;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("Timed Hosted Service running.");

			using PeriodicTimer timer = new(TimeSpan.FromMinutes(5));

			try
			{
				while (await timer.WaitForNextTickAsync(stoppingToken))
				{
					await Task.Run(async () =>
					{
						var investments = await InvestmentsAccrualAsync();
						foreach (var investment in investments)
						{
							await PacksAccrualAsync(investment);
						}
					}, stoppingToken);
				}
			}
			catch (OperationCanceledException)
			{
				_logger.LogInformation("Timed Hosted Service is stopping.");
			}
		}

		private async Task<Investment?[]> InvestmentsAccrualAsync()
		{
			try
			{
				await using var context = await _dbContextFactory.CreateDbContextAsync();
				var investments = context.Investments
					.Include(i=> i.Pool)
					.Where(i => !i.IsEnded && DateTime.UtcNow > i.LastAccrualDate.AddHours(24)).ToArray();
				context.Investments.UpdateRange(investments);
				await context.SaveChangesAsync();
				
				return investments;
			}
			catch
			{
				return Array.Empty<Investment>();
			}
		}

		private async Task PacksAccrualAsync(Investment? investment)
		{
			try
			{
				await using var context = await _dbContextFactory.CreateDbContextAsync();
				var packs = context.Packs.Where(p => p.InvestmentId == investment.Id).ToArray();
				var user = await context.Users.FirstOrDefaultAsync(u => u.Id == investment.UserId);
				var defis = context.DefiTransactions.Where(d => investment != null && d.InvestId == investment.Id).ToArray();

				if (user == null)
					return;

				var sum = investment.TotalSum + investment.TotalAccrual * investment.TotalYield / 100m;

				foreach (var defi in defis)
				{
					defi.Sum += defi.Sum * defi.AdditionPercent / 100m;
				}

				await TopUpIncome(context, user, sum, investment, defis);
				var referrerals = context.Referrals.Where(u => u.RefId == user.Id).ToArray();
				var refsId = referrerals.Select(r => r.ReferrerId).ToArray();
				var referrers = context.Users.Where(u => refsId.Contains(u.Id)).ToArray();

				if (investment.ReferralPay)
				{
					foreach (var t in referrers)
					{
						var inline = referrerals.FirstOrDefault(r => r.ReferrerId == t.Id)!.Inline;
						await TopUpAgent(context, t, sum * DefineIncome(t.Rang, inline), investment.Code, user);
					}
				}
				
				foreach (var pack in packs)
				{
					if (pack.HasLastAccrual)
						continue;

					if (pack.EndDate < DateTime.UtcNow)
						pack.HasLastAccrual = true;

					pack.CurrentYield = pack.Yield;
					var dispersion = (decimal)Math.Round(Random.Shared.NextDouble() * 0.04 - 0.02, 2);
					pack.CurrentYield += dispersion;
				}
				
				investment.TotalYield = packs.Where(p => !p.HasLastAccrual).Sum(p => p.CurrentYield);
				investment.PacksCount = packs.Count(p => !p.HasLastAccrual);

				if (packs.Length > 0)
					investment.IsEnded = packs.All(p => p.HasLastAccrual);
				else if (investment.EndDate <= DateTime.UtcNow)
					investment.IsEnded = true;
				if (investment.IsEnded)
					investment.Pool.ActiveSum -= investment.TotalSum;

				context.Users.Update(user);
				context.Packs.UpdateRange(packs);
				context.Users.UpdateRange(referrers);
				context.Investments.Update(investment);

				await context.SaveChangesAsync();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}

		private decimal DefineIncome(int rang, int inline)
		{
			switch (inline)
			{
				case 1:
					if (rang >= 1)
						return 0.1m;
					break;
				case 2:
					if (rang >= 2)
						return 0.1m;
					break;
				case 3:
					if (rang >= 3)
						return 0.1m;
					break;
				case 4:
					if (rang >= 4)
						return 0.1m;
					break;
				case 5:
					if (rang >= 5)
						return 0.1m;
					break;
				case 6:
					if (rang >= 6)
						return 0.1m;
					break;
				case 7:
					if (rang >= 7)
						return 0.1m;
					break;
				case 8:
					if (rang >= 8)
						return 0.1m;
					break;
				case 9:
					if (rang >= 9)
						return 0.1m;
					break;
				case 10:
					if (rang >= 10)
						return 0.1m;
					break;
			}

			return 0m;
		}

		private async Task TopUpAgent(FpDbContext context, User user, decimal sum, string code, User partner)
		{
			try
			{
				user.BalanceAgent += sum;
				if (sum > 0)
					user.TotalIncome += sum;
				var operation = new Operation()
				{
					IsAgentBalance = true,
					Sum = sum,
					UserId = user.Id,
					OperationTypeId = (int)OperationTypeEnum.RefBonus,
					Source = code,
					PartnerId = partner.Id
				};
				
				var notify = new Notify
				{
					Message = new DividendNotify(partner.Id, "Agent", user.ReferralCode, user.BalanceAgent).ToString(),
					UserId = user.Id,
					Type = NotificationType.Dividend.ToString()
				};
				
				context.Notifications.Add(notify);
				context.Users.Update(user);
				context.Operations.Add(operation);
				await context.SaveChangesAsync();
			}
			catch (Exception ex) { }
		}

		private static decimal GetPercentage(int days)
		{
			return days switch
			{
				>= 200 => 2m,
				>= 100 => 1.9m,
				>= 50 => 1.7m,
				>= 30 => 1.6m,
				>= 20 => 1.5m,
				>= 10 => 1.3m,
				>= 1 => 1m,
				_ => 0
			};
		}
		
		private async Task TopUpIncome(FpDbContext context, User user, decimal sum, Investment? invest, DefiTransactions[] defis)
		{
			try
			{
				if (invest != null)
				{

					invest.TotalAccrual += sum;
					invest.LastAccrualDate = DateTime.UtcNow;

					foreach (var transaction in defis)
					{
						transaction.DaysWithoutWithdraws++;
						transaction.AdditionPercent = GetPercentage(transaction.DaysWithoutWithdraws);
					}
					var operation = new Operation()
					{
						IsAgentBalance = false,
						Sum = sum,
						UserId = user.Id,
						OperationTypeId = (int)OperationTypeEnum.Accrual,
						Source = invest.Code,
					};

					var notify = new Notify
					{
						Message = new DividendNotify(user.Id, "Internal", invest.Code, invest.TotalAccrual+invest.TotalSum).ToString(),
						UserId = user.Id,
						Type = NotificationType.Dividend.ToString()
					};
					invest.Pool.TotalIncome += sum;
					context.Operations.Add(operation);
					context.Notifications.Add(notify);
					context.Investments.Update(invest);
					context.Users.Update(user);
					
					await context.SaveChangesAsync();
				}
			}
			catch (Exception ex)
			{
				// ignored
			}
		}
	}
}
