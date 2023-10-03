using System.Diagnostics;
using UnityQuickStart.App.IO;

namespace UnityQuickStart.App;

public class ProcessExecutor
{
	public static async Task ExecuteProcess(string fileName, string arguments, string processingMsg, Action<string> onSuccess, Action<string> onFail)
	{
		try
		{
			var cts = new CancellationTokenSource();
			var spinnerTask = Task.Run(() => Spinner.Spin(cts.Token, processingMsg));
			
			var psi = new ProcessStartInfo
			{
				FileName = fileName,
				Arguments = arguments,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true
			};
			
			using (var process = new Process())
			{
				process.StartInfo = psi;

				await Task.Run(() => process.Start());
				await Task.Run(() => process.WaitForExit());

				cts.Cancel();
				await spinnerTask;

				var output = await process.StandardOutput.ReadToEndAsync();
				var error = await process.StandardError.ReadToEndAsync();

				if (process.ExitCode == 0)
				{
					onSuccess(output);
				}
				else
				{
					onFail(error);
				}
			}
		}
		catch (Exception ex)
		{
			onFail(ex.Message);
		}
	}
}