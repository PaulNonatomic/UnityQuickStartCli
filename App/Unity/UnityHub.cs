using System.Diagnostics;
using Newtonsoft.Json.Linq;
using UnityQuickStart.App.IO;
using UnityQuickStart.App.Project;

namespace UnityQuickStart.App.Unity;

public class UnityHub
{
	public async Task AddProjectToHub(QuickStartProject project, string projectName, string projectPath, string version)
	{
		var addToHub = UserInput.GetYesNo("Would you like to add this project to Unity Hub:");
		if(!addToHub) Output.WriteSuccessWithTick("Skipped adding project to Hub");

		var unityHubPath = string.Empty;
		var isHubOpen = IsUnityHubOpen(out unityHubPath);
		var closedHub = isHubOpen && await CloseUnityHub();
		
		//get projects
		var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		var projectJsonPath = Path.Combine(appData, "UnityHub", "projects-v1.json");
		var projectJsonTxt = await File.ReadAllTextAsync(projectJsonPath);
		var projectsJson = JObject.Parse(projectJsonTxt);
		
		var directoryInfo = new DirectoryInfo(projectPath);
		var newProject = new JObject
		{
			["title"] = projectName,
			["lastModified"] = 1700000000000,
			["isCustomEditor"] = false,
			["path"] = projectPath,
			["containingFolderPath"] = directoryInfo.Parent?.FullName,
			["version"] = version
		};
		projectsJson["data"]![$@"{projectPath}"] = newProject;

		//write projects
		await File.WriteAllTextAsync(projectJsonPath, projectsJson.ToString());

		if (closedHub)
		{
			await OpenHub(unityHubPath);
		}
	}

	private async Task OpenHub(string fileName)
	{
		Process.Start(new ProcessStartInfo()
		{
			FileName = fileName,
			RedirectStandardInput = true,
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			UseShellExecute = false,
			CreateNoWindow = true
		});
	}
	
	private bool IsUnityHubOpen(out string unityHubPath)
	{
		unityHubPath = string.Empty;
		var processes = Process.GetProcessesByName("Unity Hub");

		if (processes.Length > 0)
		{
			unityHubPath = processes[0].MainModule.FileName;
			return true;
		}

		return false;
	}

	private async Task<bool> CloseUnityHub()
	{
		const string processMsg = "Closing Unity Hub";
		const string fileName = "taskkill";
		const string args = "/F /IM \"Unity Hub.exe\"";
		var success = false;
		
		await ProcessExecutor.ExecuteProcess(fileName,args, processMsg, 
			(output) =>
			{
				success = true;
				Output.WriteSuccessWithTick($"Ok close Hub");
				
				//wait to ensure hub to close
				Task.Delay(2000);
			},
			(error) =>
			{
				success = false;
				Output.WriteError($"Failed to close Hub: {error}");
			});

		return success;
	}
}