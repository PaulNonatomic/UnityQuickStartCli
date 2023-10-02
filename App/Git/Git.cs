using System.Diagnostics;
using System.Reflection;
using UnityQuickStart.App.IO;
using UnityQuickStart.App.Project;

namespace UnityQuickStart.App.Github;

public class Git
{
	public async Task CreateGitIgnoreFile(QuickStartProject project)
	{
		var createGitIgnore = UserInput.GetYesNo($"Would you like to include a Unity gitignore:");
		if (!createGitIgnore)
		{
			Output.WriteSuccessWithTick($"Ok skipping gitignore");
			return; 
		}
		
		// Get the directory of the executing assembly
		var assembly = Assembly.GetEntryAssembly();
		var templateContent = string.Empty;
		var gitIgnorePath = Path.Combine(project.ProjectPath, ".gitignore");
		
		using (var stream = assembly.GetManifestResourceStream("UnityQuickStart.data.gitIgnoreTemplate.txt"))
		using (var reader = new StreamReader(stream))
		{
			templateContent = await reader.ReadToEndAsync();
			await File.WriteAllTextAsync(gitIgnorePath, templateContent);
		}
		
		Output.WriteSuccessWithTick($"Ok gitignore add at {gitIgnorePath}");
	}
	
	public async Task<bool> CreateLocalRepo(QuickStartProject project)
	{
		var success = false;
		
		var createRepo = UserInput.GetYesNo($"Would you like to create a local git repo:");
		if (!createRepo)
		{
			Output.WriteSuccessWithTick($"Ok skipping local repo");
			return success; 
		}
        		
		try
		{
			var cts = new CancellationTokenSource();
			var spinnerTask = Task.Run(() => Spinner.Spin(cts.Token, "Creating local repo"));
			
			var psi = new ProcessStartInfo
			{
				FileName = "git",
				Arguments = "init",
				WorkingDirectory = project.ProjectPath,
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
					success = true;
					Output.WriteSuccessWithTick($"Ok local repo created in {project.ProjectPath}");
				}
				else
				{
					success = false;
					Output.WriteError($"Repo creation failed: {error}");
				}
			}
		}
		catch (Exception ex)
		{
			success = false;
			Output.WriteError($"Repo creation failed: {ex.Message}");
		}

		return success;
	}

	/// <summary>
	/// @Todo add support for Github teams
	/// @Todo add support for Github templates
	/// </summary>
	/// <param name="project"></param>
	/// <returns></returns>
	public async Task<bool> CreateRemoteRepo(QuickStartProject project)
	{
		var success = false;
		
		var createRepo = UserInput.GetYesNo($"Would you like to connect your local repo to a github repo:");
		if (!createRepo)
		{
			Output.WriteSuccessWithTick($"Ok skipping github repo");
			return success;
		}

		var username = await GetGithubUsername(project);
		if (string.IsNullOrEmpty(username))
		{
			//not logged into Github
			return false;
		}
		
		var projectExists = await DoesRepoExist(project, username);
		if (projectExists)
		{
			//connect existing repo
			success = await LinkToRemoteRepo(project, username);
		}
		else
		{
			success = await MakeRemoteRepo(project);
		}
		
		return success;
	}
	
	private async Task<string> GetGithubUsername(QuickStartProject project)
	{
		try
		{
			var cts = new CancellationTokenSource();
			var spinnerTask = Task.Run(() => Spinner.Spin(cts.Token, "Opening Unity project"));
			
			var psi = new ProcessStartInfo
			{
				FileName = "gh",
				Arguments = @"api user --jq .login",
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
					return output.Replace("\n", "");
				}
				else
				{
					Output.WriteError($"Github could not authenticate login");
					return string.Empty;
				}
			}
		}
		catch (Exception ex)
		{
			Output.WriteError($"Github could not authenticate. Please login");
			return string.Empty;
		}
	}

	private async Task<bool> DoesRepoExist(QuickStartProject project, string username)
	{
		try
		{
			var cts = new CancellationTokenSource();
			var spinnerTask = Task.Run(() => Spinner.Spin(cts.Token, $"Checking for existing repos with name: {project.ProjectName}"));
			
			var psi = new ProcessStartInfo
			{
				FileName = "gh",
				Arguments = $"repo view {username}/{project.ProjectName}",
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

				return process.ExitCode == 0;
			}
		}
		catch (Exception ex)
		{
			return false;
		}
	}
	
	private async Task<bool> MakeRemoteRepo(QuickStartProject project)
	{
		var success = false;
		
		try
		{
			var cts = new CancellationTokenSource();
			var spinnerTask = Task.Run(() => Spinner.Spin(cts.Token, "Creating remote Githubt repo"));
			
			var psi = new ProcessStartInfo
			{
				FileName = "gh",
				Arguments = $"repo create {project.ProjectName} --private --source {project.ProjectPath}",
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
					success = true;
					Output.WriteSuccessWithTick($"Ok Github repo {project.ProjectName} created");
				}
				else
				{
					success = false;
					Output.WriteError($"Github repo creation failed: {error}");
				}
			}
		}
		catch (Exception ex)
		{
			success = false;
			Output.WriteError($"Github repo creation failed: {ex.Message}");
		}
		
		return success;
	}
	
	private async Task<bool> LinkToRemoteRepo(QuickStartProject project, string username)
	{
		var success = false;
		
		try
		{
			var cts = new CancellationTokenSource();
			var spinnerTask = Task.Run(() => Spinner.Spin(cts.Token, "Linking local repo to remote repo"));
			
			var psi = new ProcessStartInfo
			{
				FileName = "git",
				Arguments = $"remote add origin https://github.com/{username}/{project.ProjectName}.git",
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
					success = true;
					Output.WriteSuccessWithTick($"Ok Github repo {project.ProjectName} linked");
				}
				else
				{
					success = false;
					Output.WriteError($"Linking Github repo failed: {error}");
				}
			}
		}
		catch (Exception ex)
		{
			success = false;
			Output.WriteError($"Linking Github repo failed: {ex.Message}");
		}
		
		return success;
	}
}