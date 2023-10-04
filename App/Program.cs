
using UnityQuickStart.App.Github;
using UnityQuickStart.App.IO;
using UnityQuickStart.App.Project;
using UnityQuickStart.App.Settings;
using UnityQuickStart.App.Unity;
using CommandLine;

namespace UnityQuickStart.App
{
	public class Program
	{
		private static Git _git;
		private static UnityCli _untiyCli;
		private static QuickStartProject _project;
		private static UserSettings _userSettings;

		static async Task Main(string[] args)
		{
			Console.OutputEncoding = System.Text.Encoding.UTF8;

			_git = new Git();
			_untiyCli = new UnityCli();
			_project = new QuickStartProject();
			_userSettings = new UserSettings();
			await _userSettings.LoadSettings();
			
			await Parser.Default.ParseArguments<Options>(args).WithParsedAsync(async options =>
			{
				if (options.Clear)
				{
					_userSettings.Clear();
					Output.WriteSuccessWithTick($"Settings cleared");
				}

				if (!string.IsNullOrEmpty(options.InstallPath))
				{
					_userSettings.SetUnityInstallPath(options.InstallPath);
					Output.WriteSuccessWithTick($"Unity install path set to: {options.InstallPath}");
				}

			});

			if (string.IsNullOrEmpty(_userSettings.UnityInstallPath))
			{
				await EnterUnityPath();
			}

			await SetUnityVersion();
			
			var installPath = _userSettings.UnityInstallPath;
			var unityVersion = _userSettings.UnityVersion;
			
			await _project.SetProjectPath();
			await _project.SetProjectName();
			
			var createdLocalRepo = await _git.CreateLocalRepo(_project);
			if(createdLocalRepo) await _git.CreateGitIgnoreFile(_project);
			if(createdLocalRepo) await _git.CreateRemoteRepo(_project, _userSettings);
			
			var createdUnityProject = await _untiyCli.CreateUnityProject(_project, unityVersion, installPath);
			if(createdUnityProject) await _untiyCli.OpenUnityProject(_project, unityVersion, installPath);
			if(createdUnityProject) Output.WriteSuccessWithTick("Complete");
			
			if(createdUnityProject) _project.OpenProjectDirectory();
		}

		public static async Task EnterUnityPath()
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

			_userSettings.SetUnityInstallPath(newUnityPath);
			Output.WriteSuccessWithTick($"Unity Install Path updated to: {newUnityPath}");
		}

		public static async Task SetUnityVersion()
		{
			while (true)
			{
				var installPath = _userSettings.UnityInstallPath;
				var versions = PathUtils.CommaSeperatedDirectoryList(installPath);
				var lastVersion = _userSettings.UnityVersion;

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

				_userSettings.SetUnityVersion(selectedVersion);
				Output.WriteSuccessWithTick($"Ok lets use version: {selectedVersion}");
				break;
			}
		}
	}
}