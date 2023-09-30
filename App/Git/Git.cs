﻿using System.Diagnostics;
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
			var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = "git",
					Arguments = "init",
					WorkingDirectory = project.ProjectPath,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					UseShellExecute = false,
					CreateNoWindow = true
				}
			};
        
			await Task.Run(() => process.Start());
			var error = await process.StandardError.ReadToEndAsync();
			await Task.Run(() => process.WaitForExit());
        
			process.WaitForExit();
        
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
		catch (Exception ex)
		{
			success = false;
			Output.WriteError($"Repo creation failed: {ex.Message}");
		}

		return success;
	}

	public async Task<bool> CreateRemoteRepo(QuickStartProject project)
	{
		var success = false;
		
		var createRepo = UserInput.GetYesNo($"Would you like to connect your local repo to a github repo:");
		if (!createRepo)
		{
			Output.WriteSuccessWithTick($"Ok skipping github repo");
			return success;
		}
		
		try
		{
			var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = "gh",
					Arguments = $"repo create {project.ProjectName} --private --source {project.ProjectPath}",
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
				success = true;
				Output.WriteSuccessWithTick($"Ok Github repo {project.ProjectName} created");
			}
			else
			{
				success = false;
				Output.WriteError($"Github repo creation failed: {error}");
			}
		}
		catch (Exception ex)
		{
			success = false;
			Output.WriteError($"Github repo creation failed: {ex.Message}");
		}

		return success;
	}
}