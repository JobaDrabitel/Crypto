using FP.Core.Database;
using Microsoft.EntityFrameworkCore;

namespace FP.Core.Api.Services;

public class UpdateRang
{
    private FpDbContext _dbContext;

    public UpdateRang(FpDbContext fpdbContext)
    {
        _dbContext = fpdbContext;
    }

    public async Task UpdateLineIncome(decimal balance, string referrerCode, int line = 1)
    {
        if (line == 0 || line > 10)
            return;

        try
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.ReferralCode == referrerCode);

            if (user == null)
                return;

            user.LinesIncome += balance;
            user.LinesIncome = user.LinesIncome < 0 ? 0 : user.LinesIncome;
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
            await Update(user.ReferralCode);
            await UpdateLineIncome(balance, user.ReferrerCode, line + 1);
        }
        catch (Exception e)
        {
            return;
        }
    }

    private async Task Update(string referralCode)
    {
        try
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.ReferralCode == referralCode);
            if (user is { IsFixed: true })
                return;
            var referrals = await _dbContext.Referrals
                .Include(r => r.Ref)
                .Where(r => r.ReferrerId == user.Id && r.Inline == 1)
                .ToListAsync();
            var usersInFirstLine = referrals.Select(r => r.Ref).OrderByDescending(r => r.LinesIncome + r.CurrentIncome).ToArray();
            var nextRang = RangSystem.SumToNextRang(user.Rang);
            var currentRang = RangSystem.SumToNextRang(user.Rang - 1);
            if (nextRang == 0)
                return;

            var inlines = usersInFirstLine.Select(u => u.LinesIncome + u.CurrentIncome).OrderDescending().ToArray();

            inlines[0] = usersInFirstLine[0].LinesIncome + usersInFirstLine[0].CurrentIncome > nextRang / 2.0m
                ? nextRang / 2.0m
                : usersInFirstLine[0].LinesIncome + usersInFirstLine[0].CurrentIncome;

            var allInline = inlines.Sum();
            if (allInline >= nextRang)
            {
                user.Rang++;
                _dbContext.Users.Update(user);
                await _dbContext.SaveChangesAsync();
                await Update(referralCode);
            }
            else if (allInline < currentRang)
            {
                user.Rang--;
                _dbContext.Users.Update(user);
                await _dbContext.SaveChangesAsync();
                await Update(referralCode);
            }
        }
        catch (Exception e)
        {
            return;
        }
    }
}