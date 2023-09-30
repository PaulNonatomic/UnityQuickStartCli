using UnityQuickStart.App.Github;
using UnityQuickStart.App.HelpSystem;
using UnityQuickStart.App.IO;
using UnityQuickStart.App.Settings;
using UnityQuickStart.App.Unity;

namespace UnityQuickStart.App;

public class Program
{
	private static GithubCli _github;
	private static UnityCli _untiyCli;
	private static UserSettings _userSettings;
	private static CommandLineArgs _cmdArgs;
	private static string _projectPath;

	static void Main(string[] args)
	{
		Console.OutputEncoding = System.Text.Encoding.UTF8;
		
		var aliases = new Dictionary<string, string>
		{
			{ Constants.HelpArgShort, Constants.HelpArg },
			{ Constants.ClearSettingsArgShort, Constants.ClearSettingsArg },
			{ Constants.SetUnityPathArgShort, Constants.SetUnityPathArg },
			
		};
		
		_cmdArgs = new CommandLineArgs(args, aliases);
		_github = new GithubCli();
		_untiyCli = new UnityCli();
		_userSettings = new UserSettings();
		
		if (DisplayHelp()) return;
		
		AppHeader.Write();

		ClearSettings();
		SetUnityPath();
		SetUnityVersion();
		SetProjectPath();
		SetProjectName();
		CreateLocalRepo();
		CreateRemoteRepo();
		CreateUnityProject();
	}

	private static void SetProjectName()
	{
		//@Todo get the project name !!
		
		// Output.WriteHint($"Press enter to use current directory name: {Environment.CurrentDirectory}");
		// Path.GetFileName
	}

	private static void SetProjectPath()
	{
		Output.WriteHint($"Press enter to use current directory: {Environment.CurrentDirectory}");
		
		var projectPath = UserInput.GetString(
			"Enter the project path: ",
			required: false
		);

		if (string.IsNullOrEmpty(projectPath))
		{
			projectPath = Environment.CurrentDirectory;
		}

		//@Todo add support for relative paths
		var isPathRooted = Path.IsPathRooted(projectPath);
		
		var validDirectory = Directory.Exists(projectPath);
		if (!validDirectory)
		{
			Output.WriteError("Invalid path");
			var createPath = UserInput.GetYesNo($"Would you like to create the path: {projectPath}");

			if (createPath)
			{
				try
				{
					Directory.CreateDirectory(projectPath);
				}
				catch (Exception e)
				{
					Output.WriteError(e.Message);
					SetProjectPath();
					return;
				}
			}
		}

		_projectPath = projectPath;
		Output.WriteSuccessWithTick($"Ok project path set: {projectPath}");
	}

	private static void SetUnityPath()
	{
		if (!_cmdArgs.HasFlag(Constants.SetUnityPathArg))
		{
			if (string.IsNullOrEmpty(_userSettings.GetUnityInstallPath()))
			{
				EnterUnityPath();
			}
			
			return;
		}
		
		var newUnityPath = UserInput.GetString(
			"Enter the Unity installation path: ",
			required: false,
			additionalInfo: $@"This is the path to the folder containing all installed Unity versions, typically found at {Constants.DefaultUnityPath}",
			infoColor: OutputColor.Info
		);
		
		if (string.IsNullOrEmpty(newUnityPath)) return;
		
		_userSettings.SetUnityInstallPath(newUnityPath);
		Output.WriteSuccessWithTick($"Unity Install Path updated to: {newUnityPath}");
	}

	private static void ClearSettings()
	{
		if (!_cmdArgs.HasFlag(Constants.ClearSettingsArg)) return;

		_userSettings.Clear();
		
		Output.WriteSuccessWithTick($"Settings cleared");
		
		return;
	}

	private static bool DisplayHelp()
	{
		if (!_cmdArgs.HasFlag("help")) return false;
		
		HelpPage.Display();
		return true;
	}

	private static void EnterUnityPath()
	{
		var unityPath = _userSettings.GetUnityInstallPath();

		if (string.IsNullOrEmpty(unityPath))
		{
			unityPath = Constants.DefaultUnityPath;
		}
		
		// Update Unity Install Path
		var newUnityPath = UserInput.GetString(
			"Enter the Unity installation path: ",
			required: false,
			additionalInfo: $@"This is the path to the folder containing all installed Unity versions, typically found at {unityPath}",
			infoColor: OutputColor.Info
		);
		
		if (string.IsNullOrEmpty(newUnityPath)) return;
		
		_userSettings.SetUnityInstallPath(newUnityPath);
		Output.WriteSuccessWithTick($"Unity Install Path updated to: {newUnityPath}");
	}

	private static void SetUnityVersion()
	{
		while (true)
		{
			var installPath = _userSettings.GetUnityInstallPath();
			var versions = PathUtils.CommaSeperatedDirectoryList(installPath);
			var lastVersion = _userSettings.GetUnityVersion();

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

	private static void CreateUnityProject()
	{
		
	}

	private static void CreateRemoteRepo()
	{
		
	}

	private static void CreateLocalRepo()
	{
		
	}
}