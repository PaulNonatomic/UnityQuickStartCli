using System.Diagnostics;
using UnityQuickStart.App.IO;
using UnityQuickStart.App.Project;

namespace UnityQuickStart.App.Unity
{
	public class UnityCli
	{
		private string GetPathToUnityVersion(string installPath, string version)
		{
			return Path.Combine(installPath, version, "Editor", "Unity.exe");
		}

		public async Task OpenUnityProject(QuickStartProject project, string unityVersion, string installPath)
		{
			var fileName = GetPathToUnityVersion(installPath, unityVersion);
			var cliArgs = $"-projectPath {project.ProjectPath}";

			var createRepo = UserInput.GetYesNo($"Would you like to open the Unity project at {project.ProjectPath}:");
			if (!createRepo)
			{
				Output.WriteSuccessWithTick($"Ok skip opening the project");
				return;
			}

			try
			{
				var cts = new CancellationTokenSource();
				var spinnerTask = Task.Run(() => Spinner.Spin(cts.Token, "Opening Unity project"));
				
				var psi = new ProcessStartInfo
				{
					FileName = fileName,
					Arguments = cliArgs,
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
						Output.WriteSuccessWithTick($"Ok Unity {unityVersion} project opened at {project.ProjectPath}");
					}
					else
					{
						Output.WriteError($"Opening Unity project at {project.ProjectPath} failed: {error}");
					}
				}
			}
			catch (Exception ex)
			{
				Output.WriteError($"Opening Unity project at {project.ProjectPath} failed: {ex.Message}");
			}
		}

		public async Task<bool> CreateUnityProject(QuickStartProject project, string unityVersion, string installPath)
		{
			var success = false;
			var fileName = GetPathToUnityVersion(installPath, unityVersion);
			var cliArgs = @$"-batchmode -quit -createProject {project.ProjectPath}";

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

					await Task.Run(() => process.Start());
					await Task.Run(() => process.WaitForExit());

					cts.Cancel();
					await spinnerTask;

					var output = process.StandardOutput.ReadToEnd();
					var error = process.StandardError.ReadToEnd();

					if (process.ExitCode == 0)
					{
						success = true;
						Output.WriteSuccessWithTick($"Ok Unity {unityVersion} project created at {project.ProjectPath}");
					}
					else
					{
						success = false;
						Output.WriteError($"Unity project creation failed: {error}");
					}
				}
			}
			catch (Exception ex)
			{
				success = false;
				Output.WriteError($"Unity project creation failed: {ex.Message}");
			}

			return success;
		}
	}
}