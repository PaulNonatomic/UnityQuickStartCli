using Newtonsoft.Json;

namespace UnityQuickStart.App.Settings
{
	public class UserSettings
	{
		private readonly string _settingsPath;

		public string UnityInstallPath { get; private set; } = string.Empty;
		public string UnityVersion { get; private set; } = string.Empty;

		public UserSettings(string settingsPath = "settings.json")
		{
			_settingsPath = settingsPath;
			LoadSettings();
		}

		private void LoadSettings()
		{
			if (File.Exists(_settingsPath))
			{
				var json = File.ReadAllText(_settingsPath);
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
	}
}