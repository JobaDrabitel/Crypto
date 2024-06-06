using FP.Core.Database;
using Microsoft.EntityFrameworkCore;

namespace FP.Core.Project.Extentions;

public static class WebApplicationExtensions
{

	public static async Task<IApplicationBuilder> SetupDatabaseAsync(this WebApplication app)
	{
		try
		{
			using var scope = app.Services.CreateScope();
			var conn = scope.ServiceProvider.GetRequiredService<FpDbContext>();

			await conn.Database.MigrateAsync();

			return app;
		}
		catch (Exception e)
		{
			Console.WriteLine($"Cannot setup database: {e.Message}");
			throw;
		}
	}
}
