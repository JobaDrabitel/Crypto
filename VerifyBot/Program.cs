using VerifyBot;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

VerifyTelegramBot verifyTelegramBot = new VerifyTelegramBot(builder.Configuration["Token"]!);
await verifyTelegramBot.BotStart();

app.Run();
