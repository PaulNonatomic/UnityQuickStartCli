using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityQuickStart.App.IO;
using UnityQuickStart.App.Settings;

namespace UnityQuickStart.App.Project
{
	public class QuickStartProject
	{
		public UserSettings UserSettings { get; private set; } = new();
		public string ProjectName { get; private set; }
		public string ProjectPath { get; private set; }
		public CommandLineArgs Args { get; }
		
		public QuickStartProject(string[] args)
		{
			Args = new CommandLineArgs(args);
			ProcessArgs();
		}

		private void ProcessArgs()
		{
			ClearSettings();
			SetUnityPath();
		}

		private void SetUnityPath()
		{
			var path = Args.Get(Constants.Path);
			if (!string.IsNullOrEmpty(path))
			{
				UserSettings.SetUnityInstallPath(path);
			}
		}

		private void ClearSettings()
		{
			if (!Args.Contains(Constants.Clear)) return;
		
			UserSettings.Clear();
		
			Output.WriteSuccessWithTick($"Settings cleared");
		
			return;
		}
		
		public async Task SetProjectName()
		{
			var currentDirectoryName = Path.GetFileName(Environment.CurrentDirectory);
		
			Output.WriteHint($"Press enter to use current directory name: {currentDirectoryName}");
			var projectName = UserInput.GetString(
				"Enter the project name: ",
				required: false
			);
		
			if (string.IsNullOrEmpty(projectName))
			{
				projectName = currentDirectoryName;
			}
			else
			{
				//check if the project name matches the current directory name
				if (currentDirectoryName != projectName)
				{
					var createSubDir = UserInput.GetYesNo($"Would you like to create a {projectName} sub directory as your project root?");
					var newProjectPath = Path.Combine(ProjectPath, projectName);
				
					if (createSubDir)
					{
						try
						{
							Directory.CreateDirectory(newProjectPath);
						
							ProjectPath = newProjectPath;
							Directory.SetCurrentDirectory(ProjectPath);
							Output.WriteSuccessWithTick($"Ok project path updated to: {ProjectPath}");
						}
						catch (Exception e)
						{
							Output.WriteError(e.Message);
							await SetProjectName();
						}
					}
				}
			}
		
			ProjectName = projectName;
			Output.WriteSuccessWithTick($"Ok project name set: {projectName}");
		}
		
		public async Task SetProjectPath()
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
		
			ProjectPath = projectPath;
			Directory.SetCurrentDirectory(projectPath);
			Output.WriteSuccessWithTick($"Ok project path set: {projectPath}");
		}

		public void OpenProjectDirectory()
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				Process.Start("explorer.exe", ProjectPath);
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				Process.Start("open", ProjectPath);
			}
			else
			{
				Console.WriteLine("Unsupported OS");
			}
		}
	}
}