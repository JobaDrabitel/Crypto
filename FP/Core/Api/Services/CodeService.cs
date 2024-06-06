using FP.Core.Database;
using FP.Core.Database.Handlers;
using FP.Core.Database.Models;
using Microsoft.EntityFrameworkCore;
using TronNet.Protocol;

namespace FP.Core.Api.Services;

public class CodeService : BackgroundService
{

    private static List<Promocode> Codes { get; set; } = new();
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(TimeSpan.FromSeconds(10));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            var codesToRemove = Codes.Where(code => DateTime.UtcNow > code.CreationTime.AddMinutes(5)).ToList();

            foreach (var codeToRemove in codesToRemove)
            {
                Codes.Remove(codeToRemove);
            }
        }
    }
    private static void AddCode(string code, decimal sum) => Codes.Add(new Promocode
    {
        Code = code, 
        Type = PromocodeType.Common.ToString(), 
        DealSum = sum
    });
    public static bool IsCodeContains(string code) => Codes.Select(c=> c.Code).ToList().Contains(code);
    public static decimal GetCodeSum(string code)
    {
        var promo = Codes.FirstOrDefault(c => c.Code == code);

        return promo?.DealSum ?? 0.0m;
    }
    public static void ActivateCode(string code) => Codes.RemoveAll(c => c.Code == code);

    public static string GenerateCode(PromocodeType promocodeType, decimal sum)
    {
        var stringBuilder = new RandomStringBuilder();
		var code = "0x" + stringBuilder.Create(40);
        if(promocodeType == PromocodeType.Common)
		    AddCode(code, sum);

		return code;
    }
}