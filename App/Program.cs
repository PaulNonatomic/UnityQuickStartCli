using System.Diagnostics;
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
	private static string _projectName;
	private static bool _localRepoCreated;
	private static bool _githubRepoCreated;
	private static bool _unityProjectCreated;

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
		OpenUnityProject();
	}

	private static void SetProjectName()
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
				var createSubDir = UserInput.GetYesNo($"Would you like to create a {projectName} sub directory?");
				var newProjectPath = Path.Combine(_projectPath, projectName);
				
				if (createSubDir)
				{
					try
					{
						Directory.CreateDirectory(newProjectPath);
						
						_projectPath = newProjectPath;
						Directory.SetCurrentDirectory(_projectPath);
						Output.WriteSuccessWithTick($"Ok project path updated to: {_projectPath}");
					}
					catch (Exception e)
					{
						Output.WriteError(e.Message);
						SetProjectName();
						return;
					}
				}
			}
		}
		
		_projectName = projectName;
		Output.WriteSuccessWithTick($"Ok project name set: {projectName}");
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
		Directory.SetCurrentDirectory(_projectPath);
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
		try
		{
			var cts = new CancellationTokenSource();
			var spinnerTask = Task.Run(() => Spinner(cts.Token, "Creating Unity project"));
			var psi = new ProcessStartInfo
			{
				FileName = GetPathToUnityVersion(),
				Arguments = @$"-batchmode -quit -createProject {_projectPath}",
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
					_unityProjectCreated = true;
					Output.WriteSuccessWithTick($"Ok Unity {_userSettings.GetUnityVersion()} project created at {_projectPath}");
				}
				else
				{
					_unityProjectCreated = false;
					Output.WriteError($"Unity project creation failed: {error}");
				}
			}
		}
		catch (Exception ex)
		{
			_unityProjectCreated = false;
			Output.WriteError($"Unity project creation failed: {ex.Message}");
		}
	}

	private static string GetPathToUnityVersion()
	{
		return Path.Combine(_userSettings.GetUnityInstallPath(), 
			_userSettings.GetUnityVersion(), 
			"Editor",
			"Unity.exe");
	}

	private static void OpenUnityProject()
	{
		var createRepo = UserInput.GetYesNo($"Would you like to open the Unity project at {_projectPath}:");
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
					FileName = GetPathToUnityVersion(),
					Arguments = $"-projectPath {_projectPath}",
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					UseShellExecute = false,
					CreateNoWindow = true
				}
			};

			process.Start();

			var output = process.StandardOutput.ReadToEnd();
			var error = process.StandardError.ReadToEnd();
			process.WaitForExit();
			
			if (process.ExitCode == 0)
			{
				Output.WriteSuccessWithTick($"Ok Unity {_userSettings.GetUnityVersion()} project opened at {_projectPath}");
			}
			else
			{
				Output.WriteError($"Opening Unity project at {_projectPath} failed: {error}");
			}
		}
		catch (Exception ex)
		{
			Output.WriteError($"Opening Unity project at {_projectPath} failed: {ex.Message}");
		}
	}
	
	private static void CreateRemoteRepo()
	{
		if (!_localRepoCreated) return;
		
		var createRepo = UserInput.GetYesNo($"Would you like to connect your local repo to a github repo:");
		if (!createRepo)
		{
			Output.WriteSuccessWithTick($"Ok skipping github repo");
			return;
		}
		
		try
		{
			var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = "gh",
					Arguments = $"repo create {_projectName} --private --source {_projectPath}",
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					UseShellExecute = false,
					CreateNoWindow = true
				}
			};

			process.Start();

			var output = process.StandardOutput.ReadToEnd();
			var error = process.StandardError.ReadToEnd();
			process.WaitForExit();
			
			if (process.ExitCode == 0)
			{
				_githubRepoCreated = true;
				Output.WriteSuccessWithTick($"Ok Github repo {_projectName} created");
			}
			else
			{
				_githubRepoCreated = false;
				Output.WriteError($"Github repo creation failed: {error}");
			}
		}
		catch (Exception ex)
		{
			_githubRepoCreated = false;
			Output.WriteError($"Github repo creation failed: {ex.Message}");
		}
	}

	private static void CreateLocalRepo()
	{
		var createRepo = UserInput.GetYesNo($"Would you like to create a local git repo:");
		if (!createRepo)
		{
			Output.WriteSuccessWithTick($"Ok skipping local repo");
			return;
		}
		
		try
		{
			var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = "git",
					Arguments = "init",
					WorkingDirectory = _projectPath,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					UseShellExecute = false,
					CreateNoWindow = true
				}
			};

			process.Start();

			var output = process.StandardOutput.ReadToEnd();
			var error = process.StandardError.ReadToEnd();

			process.WaitForExit();

			if (process.ExitCode == 0)
			{
				_localRepoCreated = true;
				Output.WriteSuccessWithTick($"Ok local repo created in {_projectPath}");
			}
			else
			{
				_localRepoCreated = false;
				Output.WriteError($"Repo creation failed: {error}");
			}
		}
		catch (Exception ex)
		{
			_localRepoCreated = false;
			Output.WriteError($"Repo creation failed: {ex.Message}");
		}
	}
	
	private static void Spinner(CancellationToken token, string message = "Processing")
	{
		var spinner = new char[] { '|', '/', '-', '\\' };
		var i = 0;
		
		while (!token.IsCancellationRequested)
		{
			Console.Write($"\r{message} {spinner[i++ % 4]}");
			Thread.Sleep(200);
		}
		
		Console.Write("\r                  \r");
	}
}