
using UnityQuickStart.App.Github;
using UnityQuickStart.App.HelpSystem;
using UnityQuickStart.App.IO;
using UnityQuickStart.App.Project;
using UnityQuickStart.App.Settings;
using UnityQuickStart.App.Unity;

namespace UnityQuickStart.App;

public class Program
{
	private static Git _git;
	private static UnityCli _untiyCli;
	private static QuickStartProject _project;
	private static CommandLineArgs _cmdArgs;
	
	static async Task Main(string[] args)
	{
		Console.OutputEncoding = System.Text.Encoding.UTF8;
		
		var aliases = new Dictionary<string, string>
		{
			{ Constants.HelpArgShort, Constants.HelpArg },
			{ Constants.ClearSettingsArgShort, Constants.ClearSettingsArg },
			{ Constants.SetUnityPathArgShort, Constants.SetUnityPathArg },
			
		};
		
		_cmdArgs = new CommandLineArgs(args, aliases);
		_git = new Git();
		_untiyCli = new UnityCli();
		_project = new QuickStartProject();
		
		if (DisplayHelp()) return;
		
		AppHeader.Write();

		ClearSettings();
		
		await _untiyCli.SetUnityPath(_cmdArgs, _project);
		await _untiyCli.SetUnityVersion(_project);
		
		await _project.SetProjectPath();
		await _project.SetProjectName();
		
		var createdLocalRepo = await _git.CreateLocalRepo(_project);
		if(createdLocalRepo) await _git.CreateGitIgnoreFile(_project);
		if(createdLocalRepo) await _git.CreateRemoteRepo(_project);
		
		var createdUnityProject = await _untiyCli.CreateUnityProject(_project);
		if(createdUnityProject) await _untiyCli.OpenUnityProject(_project);
		if(createdUnityProject) Output.WriteSuccessWithTick("Complete");
	}

	private static void ClearSettings()
	{
		if (!_cmdArgs.HasFlag(Constants.ClearSettingsArg)) return;

		_project.UserSettings.Clear();
		
		Output.WriteSuccessWithTick($"Settings cleared");
		
		return;
	}

	private static bool DisplayHelp()
	{
		if (!_cmdArgs.HasFlag("help")) return false;
		
		HelpPage.Display();
		return true;
	}

	

	
}