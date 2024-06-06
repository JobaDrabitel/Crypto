using Loger.Interfaces;

namespace Loger.LogServices;

internal class FileService : ActionLoger, IFileLog
{
	readonly string LogFilePath = "LogFile.log";

	public async Task WriteLog(string message)
	{
		bool writeSucceeded = false;

		while (!writeSucceeded)
		{
			try
			{
				using var writer = File.AppendText(LogFilePath);
				writer.WriteLine(message);

				writeSucceeded = true;
			}
			catch
			{
				await Task.Delay(100);
			}
		}
	}

	public async Task<string> ReadLogs()
	{
		return "";
	}

	public override void Log(string message)
	{
		_ = WriteLog(message);
	}
}
