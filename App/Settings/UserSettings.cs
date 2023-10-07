using Newtonsoft.Json;

namespace UnityQuickStart.App.Settings
{
	public class UserSettings
	{
		private readonly string _settingsPath;

		public string UnityInstallPath { get; set; }
		public string UnityVersion { get; set; }
		public string GithubOrganisation { get; set; }

		public UserSettings()
		{
			_settingsPath = Path.Combine(AppContext.BaseDirectory, "settings.json");
		}

		public async Task LoadSettings()
		{
			if (File.Exists(_settingsPath))
			{
				var json = await File.ReadAllTextAsync (_settingsPath);
				JsonConvert.PopulateObject(json, this);
			}
			else
			{
				Clear();
			}
		}

		private void SaveSettings()
		{
			File.WriteAllText(_settingsPath, JsonConvert.SerializeObject(this, Formatting.Indented));
		}

		public void SetUnityInstallPath(string newPath)
		{
			UnityInstallPath = newPath;
			SaveSettings();
		}
		
		public void SetUnityVersion(string version)
		{
			UnityVersion = version;
			SaveSettings();
		}

		public void Clear()
		{
			UnityVersion = string.Empty;
			UnityInstallPath = string.Empty;
			SaveSettings();
		}

		public void SetGithubOrganisation(string organisation)
		{
			GithubOrganisation = organisation;
			SaveSettings();
		}
	}
}