using Loger.Interfaces;

namespace Loger.LogServices;

internal class ConsoleService : ActionLoger, IFileLog
{
	readonly object locker = new object();

	public async Task WriteLog(string message)
	{
		await Console.Out.WriteLineAsync(message);
	}

	public async Task<string> ReadLogs()
	{
		lock (locker)
		{
			return "";
		}
	}

	public override void Log(string message)
	{
		_ = WriteLog(message);
	}
}
