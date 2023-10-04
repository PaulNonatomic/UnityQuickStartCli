using System.Reflection;
using UnityQuickStart.App.IO;
using UnityQuickStart.App.Project;
using UnityQuickStart.App.Settings;

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
		
		const string processMsg = "Creating local repo";
		const string fileName = "git";
		const string args = "init";
		
		await ProcessExecutor.ExecuteProcess(fileName,args, processMsg, 
			(output) =>
			{
				success = true;
				Output.WriteSuccessWithTick($"Ok local repo created in {project.ProjectPath}");
			},
			(error) =>
			{
				success = false;
				Output.WriteError($"Repo creation failed: {error}");
			});

		return success;
	}

	public async Task<bool> IsUserLoggedIn(QuickStartProject project)
	{
		const string processMsg = "Check Github auth status";
		const string fileName = "gh";
		const string args = @"auth status";
		var isLoggedIn = false;
		
		await ProcessExecutor.ExecuteProcess(fileName,args, processMsg, 
			(output) =>
			{
				var result = output.Replace("\n", "");

				isLoggedIn = result.Contains("Logged in to");
			},
			(error) =>
			{
				isLoggedIn = false;
			});

		return isLoggedIn;
	}

	public async Task<string> Login(QuickStartProject project)
	{
		const string processMsg = "Logging into Github";
		const string fileName = "gh";
		const string args = @"auth login";
		var result = string.Empty;
		
		await ProcessExecutor.ExecuteProcess(fileName,args, processMsg, 
			(output) =>
			{
				result = output.Replace("\n", "");
				Output.WriteError($"Github {result}");
			},
			(error) =>
			{
				Output.WriteError($"Github could not authenticate. Please login: {error}");
			});

		return result;
	}

	/// <summary>
	/// @Todo add support for Github teams
	/// @Todo add support for Github templates
	/// </summary>
	/// <param name="project"></param>
	/// <returns></returns>
	public async Task<bool> CreateRemoteRepo(QuickStartProject project, UserSettings userSettings)
	{
		var success = false;
		
		var createRepo = UserInput.GetYesNo($"Would you like to connect your local repo to a github repo:");
		if (!createRepo)
		{
			Output.WriteSuccessWithTick($"Ok skipping github repo");
			return success;
		}

		var loggedIn = await IsUserLoggedIn(project);
		if (!loggedIn) await Login(project);

		var username = await GetGithubUsername(project);
		if (string.IsNullOrEmpty(username))
		{
			//not logged into Github
			return false;
		}
		
		var organisations = await GetGithubOrganisations(project);
		
		if (organisations.Count > 0)
		{
			var selectedOrganisation = await SelectOrganisation(project, userSettings, organisations);
			if (!string.IsNullOrEmpty(selectedOrganisation))
			{
				username = selectedOrganisation;
			}
		}
		
		var projectExists = await DoesRepoExist(project, username);
		if (projectExists)
		{
			var connect = UserInput.GetYesNo($"Do you want to connect to existing repo https://github.com/{username}/{project.ProjectName}.git");
			if (connect)
			{
				success = await LinkToRemoteRepo(project, username);
			}
			else
			{
				Output.WriteSuccessWithTick("Ok you should consider using a different project name, github account, or organisation");
			}
		}
		else
		{
			success = await MakeRemoteRepo(project, username);
		}
		
		return success;
	}

	private async Task<string> SelectOrganisation(QuickStartProject project, UserSettings userSettings, List<string> organisations)
	{
		var orgString = string.Join(", ", organisations);
		Output.WriteLine();
		Output.WriteInfo($"Available organisations: {orgString}");
		var useOrg = UserInput.GetYesNo("Would you like to use an organisation:");
		if (!useOrg)
		{
			return string.Empty;
		}
		
		var lastOrgansation = userSettings.GithubOrganisation;
		if (string.IsNullOrEmpty(lastOrgansation)) lastOrgansation = organisations[0];
		
		Output.WriteLine();
		Output.WriteInfo($"Available organisations: {orgString}");
		Output.WriteHint($"Press enter to use organisation: {lastOrgansation}");
		var selectedOrganisation = UserInput.GetString($"Enter an organisation:", required: false);
		if (string.IsNullOrEmpty(selectedOrganisation))
		{
			selectedOrganisation = lastOrgansation;
		}

		userSettings.SetGithubOrganisation(selectedOrganisation);
		Output.WriteSuccessWithTick($"Ok using organisation: {selectedOrganisation}");
		return selectedOrganisation;
	}

	private async Task<string> GetGithubUsername(QuickStartProject project)
	{
		const string processMsg = "Fetching Github username";
		const string fileName = "gh";
		const string args = @"api user --jq .login";
		var username = string.Empty;
		
		await ProcessExecutor.ExecuteProcess(fileName,args, processMsg, 
			(output) =>
			{
				username = output.Replace("\n", "");
			},
			(error) =>
			{
				Output.WriteError($"Github could not authenticate. Please login: {error}");
			});

		return username;
	}
	
	private async Task<List<string>> GetGithubOrganisations(QuickStartProject project)
	{
		const string processMsg = "Fetching Github organisations";
		const string fileName = "gh";
		const string args = @"org list";
		var organisations = new List<string>();
		
		await ProcessExecutor.ExecuteProcess(fileName,args, processMsg, 
			(output) =>
			{
				organisations = output.Split("\n").ToList();
			},
			(error) =>
			{
				Output.WriteError($"Github could not authenticate. Please login: {error}");
			});

		return organisations;
	}

	private async Task<bool> DoesRepoExist(QuickStartProject project, string username)
	{
		var processMsg = $"Checking for existing repos with name: {project.ProjectName}";
		const string fileName = "gh";
		var args = $"repo view {username}/{project.ProjectName}";
		var success = false;
		
		await ProcessExecutor.ExecuteProcess(fileName,args, processMsg, 
			(output) =>
			{
				success = true;
			},
			(error) =>
			{
				success = false;
			});

		return success;
		
	}
	
	private async Task<bool> MakeRemoteRepo(QuickStartProject project, string username)
	{
		const string processMsg = "Creating remote Githubt repo";
		const string fileName = "gh";
		var args = $"repo create {username}/{project.ProjectName} --private --source {project.ProjectPath}";
		var success = false;
		
		await ProcessExecutor.ExecuteProcess(fileName,args, processMsg, 
			(output) =>
			{
				Output.WriteSuccessWithTick($"Ok Github repo {project.ProjectName} created");
				success = true;
			},
			(error) =>
			{
				Output.WriteError($"Github repo creation failed: {error}");
			});

		return success;
	}
	
	private async Task<bool> LinkToRemoteRepo(QuickStartProject project, string username)
	{
		const string processMsg = "Linking local repo to remote repo";
		const string fileName = "git";
		var args = $"remote add origin https://github.com/{username}/{project.ProjectName}.git";
		var success = false;
		
		await ProcessExecutor.ExecuteProcess(fileName,args, processMsg, 
			(output) =>
			{
				Output.WriteSuccessWithTick($"Ok Github repo {project.ProjectName} linked");
				success = true;
			},
			(error) =>
			{
				Output.WriteError($"Linking Github repo failed: {error}");
			});

		return success;
	}
}