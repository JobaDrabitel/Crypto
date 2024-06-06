using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace VerifyBot;

public class VerifyTelegramBot
{
	private static TelegramBotClient _botClient;
	private static readonly HttpClient _client = new HttpClient();
	private string _botToken;

	public VerifyTelegramBot(string botToken)
	{
		_botToken = botToken;
	}

	private ReceiverOptions _receiverOptions = new()
	{
		AllowedUpdates = new[]
		{
			UpdateType.Message,
		},
	};
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
			if (e.Message?.Text == null) return;
			if (!e.Message.Text.StartsWith("/start")) return;

			var code = GetCodeFromStartCommand(e.Message.Text);
			if (code == string.Empty)
			{
				await _botClient.SendTextMessageAsync(e.Message.Chat.Id, "Invalid query param");
				return;
			}
			
			var result =
				await _client.PutAsync($"http://fp:80/api/user/verifyTelegram?tg={code}&chatId={e.Message.Chat.Id}",
					null);
			if (!result.IsSuccessStatusCode)
			{
				await _botClient.SendTextMessageAsync(e.Message.Chat.Id, "Cannot verify");
				return;
			}

			await _botClient.SendTextMessageAsync(e.Message.Chat.Id, "Success");
		}
		catch (Exception ex)
		{
			await _botClient.SendTextMessageAsync(e.Message.Chat.Id, "Internal error");
			Console.WriteLine(ex.Message);
		}
	}

	private string GetCodeFromStartCommand(string startCommand)
	{
		var code = startCommand.Split(' ');
		if(code.Length > 0)
			return code[1];

		return string.Empty;
	}
	private async Task HandlePollingErrorAsync(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
	{
		return;
	}
}
