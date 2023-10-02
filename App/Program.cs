
using UnityQuickStart.App.Github;
using UnityQuickStart.App.HelpSystem;
using UnityQuickStart.App.IO;
using UnityQuickStart.App.Project;
using UnityQuickStart.App.Settings;
using UnityQuickStart.App.Unity;

namespace UnityQuickStart.App
{
	public class Program
	{
		private static Git _git;
		private static UnityCli _untiyCli;
		private static QuickStartProject _project;

		static async Task Main(string[] args)
		{
			Console.OutputEncoding = System.Text.Encoding.UTF8;

			_project = new QuickStartProject(args);
			_git = new Git();
			_untiyCli = new UnityCli();
		
			if (DisplayHelp()) return;
			
			AppHeader.Write();
		
			//ClearSettings();
			
			await _untiyCli.SetUnityPath(_project);
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
			
		private static bool DisplayHelp()
		{
			if(!_project.Args.Contains(Constants.Help)) return false;
			
			HelpPage.Display();
			return true;
		}
	}
}