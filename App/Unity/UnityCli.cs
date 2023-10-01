using System.Diagnostics;
using UnityQuickStart.App.IO;
using UnityQuickStart.App.Project;
using UnityQuickStart.App.Settings;

namespace UnityQuickStart.App.Unity
{
	public class UnityCli
	{
		private string GetPathToUnityVersion(string installPath, string version)
		{
			return Path.Combine(installPath, version, "Editor", "Unity.exe");
		}

		public async Task OpenUnityProject(QuickStartProject project)
		{
			var version = project.UserSettings.GetUnityVersion();
			var installPath = project.UserSettings.GetUnityInstallPath();
			var fileName = GetPathToUnityVersion(installPath, version);
			var cliArgs = $"-projectPath {project.ProjectPath}";

			var createRepo = UserInput.GetYesNo($"Would you like to open the Unity project at {project.ProjectPath}:");
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

				await Task.Run(() => process.Start());
				var error = await process.StandardError.ReadToEndAsync();
				await Task.Run(() => process.WaitForExit());

				if (process.ExitCode == 0)
				{
					Output.WriteSuccessWithTick($"Ok Unity {version} project opened at {project.ProjectPath}");
				}
				else
				{
					Output.WriteError($"Opening Unity project at {project.ProjectPath} failed: {error}");
				}
			}
			catch (Exception ex)
			{
				Output.WriteError($"Opening Unity project at {project.ProjectPath} failed: {ex.Message}");
			}
		}

		public async Task<bool> CreateUnityProject(QuickStartProject project)
		{
			var success = false;
			var version = project.UserSettings.GetUnityVersion();
			var installPath = project.UserSettings.GetUnityInstallPath();
			var fileName = GetPathToUnityVersion(installPath, version);
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
						Output.WriteSuccessWithTick($"Ok Unity {version} project created at {project.ProjectPath}");
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

		public async Task SetUnityPath(QuickStartProject project)
		{
			if (project.Args.Contains(Constants.Path))
			{
				var path = project.Args.Get(Constants.Path);
				if (!string.IsNullOrEmpty(path))
				{
					project.UserSettings.SetUnityInstallPath(path);
					Output.WriteSuccessWithTick($"Unity Install Path updated to: {path}");
					return;
				}

				await EnterUnityPath(project.UserSettings);
				return;
			}

			if (string.IsNullOrEmpty(project.UserSettings.GetUnityInstallPath()))
			{
				await EnterUnityPath(project.UserSettings);
			}
		}

		public async Task EnterUnityPath(UserSettings userSettings)
		{
			Output.WriteInfo($@"This is the path to the folder containing all installed Unity versions");
			Output.WriteHint($"Press enter to use typical: {Constants.DefaultUnityPath}");
			var newUnityPath = UserInput.GetString(
				"Enter the Unity installation path: ",
				required: false
			);

			if (string.IsNullOrEmpty(newUnityPath))
			{
				newUnityPath = Constants.DefaultUnityPath;
			}

			userSettings.SetUnityInstallPath(newUnityPath);
			Output.WriteSuccessWithTick($"Unity Install Path updated to: {newUnityPath}");
		}

		public async Task SetUnityVersion(QuickStartProject project)
		{
			while (true)
			{
				var installPath = project.UserSettings.GetUnityInstallPath();
				var versions = PathUtils.CommaSeperatedDirectoryList(installPath);
				var lastVersion = project.UserSettings.GetUnityVersion();

				Output.WriteInfo($@"Available versions: {versions}");

				if (!string.IsNullOrEmpty(lastVersion))
				{
					Output.WriteHint($"Press Enter to use last version: {lastVersion}.");
				}

				var selectedVersion = UserInput.GetString("Enter the Unity version: ", required: false);

				if (string.IsNullOrEmpty(selectedVersion))
				{
					selectedVersion = lastVersion;
				}

				var validVersion = PathUtils.PathContainsDirectory(installPath, selectedVersion);
				if (!validVersion)
				{
					Output.WriteError($"Invalid version {Environment.NewLine}");
					continue;
				}

				project.UserSettings.SetUnityVersion(selectedVersion);
				Output.WriteSuccessWithTick($"Ok lets use version: {selectedVersion}");
				break;
			}
		}
	}
}