namespace Loger.Interfaces;

internal interface IFileLog
{
	Task WriteLog(string message);
	Task<string> ReadLogs();
}
