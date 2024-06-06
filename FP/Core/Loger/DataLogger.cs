using Loger.Interfaces;
using Loger.LogServices;

namespace FP.Core.Loger;

public class DataLogger
{
	private static List<string> _logs = new();
	private static readonly List<ActionLoger> _logers = new()
	{
		new FileService(),
		new ConsoleService(),
	};

	public static void AddLog(string log) => _logs.Add(log);

	public static async void StartLogging()
	{
		await Task.Run(() =>
		{
			while (true)
			{
				if (_logs.Count > 0)
				{
					foreach (var item in _logers)
					{
						item.Log(_logs[0]);
					}

					_logs.Remove(_logs[0]);
				}
			}
		});
	}
}
