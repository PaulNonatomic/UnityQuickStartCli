using System.Diagnostics;
using UnityQuickStart.App.IO;
using UnityQuickStart.App.Project;

namespace UnityQuickStart.App.Unity
{
	public class UnityCli
	{
		private string GetPathToUnityVersion(string installPath, string version)
		{
			return Path.Combine(installPath, version, "Editor", "Unity.exe");
		}

		public async Task OpenUnityProject(QuickStartProject project, string unityVersion, string installPath)
		{
			var createRepo = UserInput.GetYesNo($"Would you like to open the Unity project at {project.ProjectPath}:");
			if (!createRepo)
			{
				Output.WriteSuccessWithTick($"Ok skip opening the project");
				return;
			}
			
			const string processMsg = "Opening Unity project";
			var fileName = GetPathToUnityVersion(installPath, unityVersion);
			var args = $"-projectPath {project.ProjectPath}";
		
			await ProcessExecutor.ExecuteProcess(fileName,args, processMsg, 
				(output) =>
				{
					Output.WriteSuccessWithTick($"Ok Unity {unityVersion} project opened at {project.ProjectPath}");
				},
				(error) =>
				{
					Output.WriteError($"Opening Unity project at {project.ProjectPath} failed: {error}");
				});
		}

		public async Task<bool> CreateUnityProject(QuickStartProject project, string unityVersion, string installPath)
		{
			const string processMsg = "Creating Unity project";
			var fileName = GetPathToUnityVersion(installPath, unityVersion);
			var args = @$"-batchmode -quit -createProject {project.ProjectPath}";
			var success = false;
		
			await ProcessExecutor.ExecuteProcess(fileName,args, processMsg, 
				(output) =>
				{
					success = true;
					Output.WriteSuccessWithTick($"Ok Unity {unityVersion} project created at {project.ProjectPath}");
				},
				(error) =>
				{
					success = false;
					Output.WriteError($"Unity project creation failed: {error}");
				});

			return success;
		}
	}
}