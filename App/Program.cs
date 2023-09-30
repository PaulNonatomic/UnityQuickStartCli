using UnityQuickStart.App.Github;
using UnityQuickStart.App.Input;
using UnityQuickStart.App.Settings;
using UnityQuickStart.App.Unity;

namespace UnityQuickStart.App;

public class Program
{
	private static GithubCli _github;
	private static UnityCli _untiyCli;
	private static UserSettings _userSettings;

	static void Main(string[] args)
	{
		_github = new GithubCli();
		_untiyCli = new UnityCli();
		_userSettings = new UserSettings();

		GetUnityInstallPath();
		CreateLocalRepo();
		CreateRemoteRepo();
		CreateUnityProject();
	}

	private static void GetUnityInstallPath()
	{
		var unityPath = _userSettings.GetUnityInstallPath();

		if (string.IsNullOrEmpty(unityPath))
		{
			unityPath = @"C:\Program Files\Unity\Hub\Editor";
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