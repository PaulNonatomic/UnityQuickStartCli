using UnityQuickStart.App.Github;
using UnityQuickStart.App.HelpSystem;
using UnityQuickStart.App.Input;
using UnityQuickStart.App.Settings;
using UnityQuickStart.App.Unity;
using UnityQuickStart.Cli.Utilities;

namespace UnityQuickStart.App;

public class Program
{
	private static GithubCli _github;
	private static UnityCli _untiyCli;
	private static UserSettings _userSettings;
	private static CommandLineArgs _cmdArgs;

	private const string DefaultUnityPath = @"C:\Program Files\Unity\Hub\Editor";
	
	static void Main(string[] args)
	{
		var aliases = new Dictionary<string, string>
		{
			{ "h", "help" }
		};
		
		_cmdArgs = new CommandLineArgs(args, aliases);
		
		if (DisplayHelp()) return;
		
		_github = new GithubCli();
		_untiyCli = new UnityCli();
		_userSettings = new UserSettings();
		
		GetUnityInstallPath();
		CreateLocalRepo();
		CreateRemoteRepo();
		CreateUnityProject();
	}

	private static bool DisplayHelp()
	{
		if (!_cmdArgs.HasFlag("help")) return false;
		
		HelpPage.Display();
		return true;
	}

	private static void GetUnityInstallPath()
	{
		var unityPath = _userSettings.GetUnityInstallPath();

		if (string.IsNullOrEmpty(unityPath))
		{
			unityPath = DefaultUnityPath;
		}
		
		// Update Unity Install Path
		var newUnityPath = UserInput.GetString(
			"Enter the Unity installation path: ",
			required: false,
			additionalInfo: $@"This is the path to the folder containing all installed Unity versions, typically found at {unityPath}",
			infoColor: ConsoleColor.Yellow
		);
		
		if (string.IsNullOrEmpty(newUnityPath)) return;
		
		_userSettings.SetUnityInstallPath(newUnityPath);
		Console.WriteLine($"Unity Install Path updated to: {newUnityPath}");
	}

	private static void CreateUnityProject()
	{
		throw new NotImplementedException();
	}

	private static void CreateRemoteRepo()
	{
		throw new NotImplementedException();
	}

	private static void CreateLocalRepo()
	{
		throw new NotImplementedException();
	}
}