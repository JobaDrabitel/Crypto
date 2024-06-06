using FP.Core.Loger;
using Loger.Enums;
using Loger.Interfaces;
using Loger.LogServices;

namespace Loger;

public class LogService<T>
{
	private readonly string? NameSpace;
	private readonly List<ActionLoger> _logers = new()
	{
		new FileService(),
		new ConsoleService(),
	};

	public LogService() => NameSpace = typeof(T).FullName;

	public void LogAction(string message, string[]? addition = null, LogType logType = LogType.Inforamation, short trace = 100, Exception? exception = null)
	{
		string logMessage = CastMessage(message, addition, logType, trace, exception);
		DataLogger.AddLog(logMessage);
	}

	private string CastMessage(string message, string[]? addition = null, LogType logType = LogType.Inforamation, short trace = 000, Exception? exception = null)
	{
		string logMessage = exception == null ?
				$"{DateTime.Now} | {trace} | {logType} | {NameSpace} | {message}\n" :
				$"{DateTime.Now} | {trace} | {logType} | {NameSpace} | {message} | {exception}\n";

		if (addition != null)
			foreach (var item in addition)
			{
				logMessage += $"{DateTime.Now} | {trace} |\t\t\t-> {item}\n";
			}

		return logMessage;
	}
}
