using System.Diagnostics;
using UnityQuickStart.App.IO;
using UnityQuickStart.App.Settings;

namespace UnityQuickStart.App.Unity;

public class UnityCli
{
	private string GetPathToUnityVersion(string installPath, string version)
	{
		return Path.Combine(installPath, version, "Editor", "Unity.exe");
	}
	
	public void OpenUnityProject(string projectPath, UserSettings userSettings)
	{
		var version = userSettings.GetUnityVersion();
		var installPath = userSettings.GetUnityInstallPath();
		var fileName = GetPathToUnityVersion(installPath, version);
		var cliArgs = $"-projectPath {projectPath}";
		
		var createRepo = UserInput.GetYesNo($"Would you like to open the Unity project at {projectPath}:");
		if (!createRepo)
		{
			Output.WriteSuccessWithTick($"Ok skip opening the project");
			return;
		}
		
		try
		{
			var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = fileName,
					Arguments = cliArgs,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					UseShellExecute = false,
					CreateNoWindow = true
				}
			};

			process.Start();
			
			var error = process.StandardError.ReadToEnd();
			process.WaitForExit();
			
			if (process.ExitCode == 0)
			{
				Output.WriteSuccessWithTick($"Ok Unity {version} project opened at {projectPath}");
			}
			else
			{
				Output.WriteError($"Opening Unity project at {projectPath} failed: {error}");
			}
		}
		catch (Exception ex)
		{
			Output.WriteError($"Opening Unity project at {projectPath} failed: {ex.Message}");
		}
	}
	
	public void CreateUnityProject(string projectPath, UserSettings userSettings)
	{
		var version = userSettings.GetUnityVersion();
		var installPath = userSettings.GetUnityInstallPath();
		var fileName = GetPathToUnityVersion(installPath, version);
		var cliArgs = @$"-batchmode -quit -createProject {projectPath}";
		
		try
		{
			var cts = new CancellationTokenSource();
			var spinnerTask = Task.Run(() => Spinner.Spin(cts.Token, "Creating Unity project"));
			var psi = new ProcessStartInfo
			{
				FileName = fileName,
				Arguments = cliArgs,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true,
				Verb = "runas"
			};
			
			using (var process = new Process())
			{
				process.StartInfo = psi;
				process.Start();
				process.WaitForExit();

				cts.Cancel();
				spinnerTask.Wait();
				
				var output = process.StandardOutput.ReadToEnd();
				var error = process.StandardError.ReadToEnd();

				if (process.ExitCode == 0)
				{
					
					Output.WriteSuccessWithTick($"Ok Unity {version} project created at {projectPath}");
				}
				else
				{
					Output.WriteError($"Unity project creation failed: {error}");
				}
			}
		}
		catch (Exception ex)
		{
			Output.WriteError($"Unity project creation failed: {ex.Message}");
		}
	}
}