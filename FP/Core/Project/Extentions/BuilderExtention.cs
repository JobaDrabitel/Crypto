using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using FP.Core.Api.ApiDto;
using FP.Core.Api.Providers.Interfaces;
using FP.Core.Api.Providers.Providers;
using FP.Core.Api.Providers.Providers.Networks.BEP20;
using FP.Core.Api.Providers.Providers.Networks.TRC20;
using FP.Core.Api.Responses;
using FP.Core.Api.Services;
using FP.Core.Database;
using FP.Core.Database.Handlers;
using FP.Core.Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FP.Core.Project.Extentions;

public static class BuilderExtention
{
    private static void UploadSmtpData(this WebApplicationBuilder builder)
    {
        try
        {
            var fileData = File.ReadAllText("./Data/SmtpData.json");
            var data = JsonSerializer.Deserialize<SmtpData>(fileData);

            if (data == null)
                throw new InvalidOperationException();
            
            ConfirmEmailService.SmtpAddress = data.SmtpAddress;
            ConfirmEmailService.SmtpEmail = data.SmtpEmail;
            ConfirmEmailService.SmtpPassword = data.SmtpPassword;
            ConfirmEmailService.SmtpPort = data.SmtpPort;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.UploadSmtpData();
        
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDbContextFactory<FpDbContext>(o => o.UseNpgsql(builder.Configuration["ConnectionStrings:string"]));
        builder.Services.AddCors();

        builder.Services.AddScoped<InvestmentDatabaseHandler>();
        builder.Services.AddScoped<PackDatabaseHandler>();
        builder.Services.AddScoped<ReferralDatabaseHandler>();
        builder.Services.AddScoped<TransactionDatabaseHandler>();
        builder.Services.AddScoped<UserDatabaseHandler>();
        builder.Services.AddScoped<WalletDatabaseHandler>();
        builder.Services.AddScoped<VerificationCodeDatabaseHandler>();
        builder.Services.AddScoped<OperationDatabaseHandler>();
        builder.Services.AddScoped<WithdrawDatabaseHandler>();
        builder.Services.AddScoped<NotificationDatabaseHandler>();
        builder.Services.AddScoped<PromocodeDatabaseHandler>();
        builder.Services.AddHostedService<PackAccrualSchedulerService>();
        builder.Services.AddHostedService<CodeService>();
        builder.Services.AddScoped<JwtService>();
        builder.Services.AddScoped<ConfirmEmailService>();

        builder.Services.AddSingleton<ICryptoFactory, CryptoFactory>();
        
        builder.Services.AddScoped<TestDataService>();
        builder.Services.AddTransient<IPasswordHasher<User>, PasswordHasher<User>>();
        builder.Services.AddTransient<IPasswordHasher<WalletDto>, PasswordHasher<WalletDto>>();

        builder.Services.AddHttpClient("Crypto", client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["CryptoApiTrc"]!);
        });

        builder.Services.AddHttpClient("Eth", client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["CryptoApiEth"]!);
        });
        
        var networks = Assembly.GetExecutingAssembly().GetTypes()
            .Where(x => x.GetInterfaces().Contains(typeof(ICryptoApiProvider)));

        foreach (var network in networks)
        {
            builder.Services.Add(new ServiceDescriptor(typeof(ICryptoApiProvider), network, ServiceLifetime.Singleton));
        }
        
        
        builder.Services.AddHttpClient("Fiat", client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["Fiat:BaseApiUri"]!);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", builder.Configuration["Fiat:ApiKey"]!);
        });
    }
}