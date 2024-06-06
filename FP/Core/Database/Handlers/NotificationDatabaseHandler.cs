using FP.Core.Api.ApiDto;
using FP.Core.Api.Providers.Interfaces;
using FP.Core.Api.Responses;
using FP.Core.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace FP.Core.Database.Handlers
{
    public class NotificationDatabaseHandler
	{
		private readonly FpDbContext _dbContext;


		public NotificationDatabaseHandler(FpDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<Notify?> CreateNotification(INotify notification)
		{
			try
			{
				var notify = new Notify
				{
					UserId = notification.UserId,
					Message = notification.ToString(),
					Type = notification.NotifyType,
				};

				_dbContext.Notifications.Add(notify);
				await _dbContext.SaveChangesAsync();

				return notify;
			}
			catch (Exception ex)
			{
				// Обработка исключения
			}

			return null;
		}
		public async Task<Notify?> ReadNotifyAsync(int id)
		{
			try
			{
				var notify = await _dbContext.Notifications.FindAsync(id);
				notify!.IsRead = true;
				_dbContext.Notifications.Update(notify);
				await _dbContext.SaveChangesAsync();
				return notify;
			}
			catch 
			{
				return null;
			}
		}
		public async Task<List<Notify>?> ReadAllNotifiesByUser(int userId)
		{
			try
			{
				var notifies = _dbContext.Notifications.Where(n=> n.UserId == userId).ToList();
				foreach (var notify in notifies)
					notify.IsRead = true;
				_dbContext.Notifications.UpdateRange(notifies);
				await _dbContext.SaveChangesAsync();
				return notifies;
			}
			catch 
			{
				return null;
			}
		}
		public async Task<Notify?> GetNotifyAsync(int id)
		{
			try
			{
				return await _dbContext.Notifications.FindAsync(id);
			}
			catch
			{
				return null;
			}
		}

		public async Task<ReturnResponse> GetAllNotifyAsync(int userId)
		{
			try
			{
				var notifications = _dbContext.Notifications.Where(n => n.UserId == userId).ToList();
				return new OkResponse<List<Notify>>(notifications);
			}
			catch (Exception e)
			{
				return new InternalErrorResponse();
			}
		}
	}
}
