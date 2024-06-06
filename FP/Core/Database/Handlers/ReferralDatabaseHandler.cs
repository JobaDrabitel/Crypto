using FP.Core.Api.Responses;
using FP.Core.Api.Services;
using FP.Core.Database.Models;

namespace FP.Core.Database.Handlers;

public class ReferralDatabaseHandler
{
    private readonly FpDbContext _dbContext;
    private readonly UserDatabaseHandler _userDatabaseHandler;
    private readonly NotificationDatabaseHandler _notificationDatabaseHandler;
    private readonly ILogger<UserDatabaseHandler> _logger;
    
    public ReferralDatabaseHandler(FpDbContext dbContext, UserDatabaseHandler userDatabaseHandler, NotificationDatabaseHandler notificationDatabaseHandler, ILogger<UserDatabaseHandler> logger)
    {
        _dbContext = dbContext;
        _userDatabaseHandler = userDatabaseHandler;
        _notificationDatabaseHandler = notificationDatabaseHandler;
        _logger = logger;
    }

    public async Task<bool> CreateReferralTree(User user)
    {
        var isSuccess = true;

        try
        {
            var referrer = await _userDatabaseHandler.GetUserByReferralCode(user.ReferrerCode) as OkResponse<User>;
            if(!referrer.Status)
                return false;
          
            for (int i = 0; i < 10; i++)
            {
                Referral referral = new()
                {
                    Ref = user,
                    Referrer = referrer.ObjectData,
                    Inline = i + 1
                };
                await _dbContext.Referrals.AddAsync(referral);
                await _notificationDatabaseHandler.CreateNotification(new ReferralNotify(referrer.ObjectData.Id, user.ReferralCode, i + 1));
                var responce = await _userDatabaseHandler.GetUserByReferralCode(referrer.ObjectData.ReferrerCode);
                if (!responce.Status)
                {
                    break;
                }
                referrer = responce as OkResponse<User>;
            }

            await _dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Cannot create referral tree");
            isSuccess = false;
        }
        
        return isSuccess;
    }

    public User[] GetReferrersByUserId(int userId)
    {
        try
        {
            var users = _dbContext.Referrals.Where(u => u.RefId == userId).ToArray();
        }
        catch
        {
            return Array.Empty<User>();
        }
        
        return Array.Empty<User>();
    }
}