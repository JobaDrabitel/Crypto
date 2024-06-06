using System.Net.Http.Json;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using FP.Core.Api.Responses;
using System.ComponentModel.DataAnnotations.Schema;

namespace VerifyTelegramBot
{
	internal class Program
	{
		public class VerifyTelegramBot
		{
			private static TelegramBotClient _botClient;
			private static readonly HttpClient _client = new HttpClient();
			private const string _botToken = "6693488806:AAFg91DLGtGd1sGARb7cfaHPb4wdst2zAP0";
			private ReceiverOptions _receiverOptions = new()
			{
				AllowedUpdates = new[]
				{
				UpdateType.Message,
			},
			};
			public static async Task Main()
			{
				VerifyTelegramBot bot = new VerifyTelegramBot();
				await bot.BotStart();
			}
			public async Task BotStart()
			{
				_botClient = new TelegramBotClient(_botToken);
				_botClient.StartReceiving(updateHandler: HandleUpdateAsync, HandlePollingErrorAsync, _receiverOptions);
				var me = await _botClient.GetMeAsync();
				await Console.Out.WriteLineAsync(me.FirstName);
			}

			private async Task HandleUpdateAsync(ITelegramBotClient arg1, Update e, CancellationToken arg3)
			{
				try
				{
					if (e.Message.Text != null)
					{
						if (e.Message.Text.StartsWith("/start"))
						{
							if (!int.TryParse(GetCodeFromStartCommand(e.Message.Text), out int code))
								return;
							var result = await _client.GetAsync($"https://localhost:7093/api/verify/get/{code}");
							if (!result.IsSuccessStatusCode)
								return;
							var resultContent = await result.Content.ReadFromJsonAsync<int>();
							var userId = resultContent;
							if (userId == null)
								return;
							result = await _client.PutAsync($"https://localhost:7093/api/user/verifyTelegram?userId={userId}&chatId={e.Message.Chat.Id}", null);
							return;
						}
					}
					return;
				}
				catch (Exception ex) { return; }
			}

			private string GetCodeFromStartCommand(string startCommand)
			{
				var code = startCommand.Split(' ')[1];
				return code;
			}
			private async Task HandlePollingErrorAsync(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
			{
				return;
			}
		}
	}
	public class VerificationCode
	{
		public int Id { get; set; }
		public int Code { get; set; }
		public bool IsActive { get; set; } = true;
		public int? UserId { get; set; }
	}
}
