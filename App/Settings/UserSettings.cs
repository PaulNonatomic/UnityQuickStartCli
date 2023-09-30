using Newtonsoft.Json;

namespace UnityQuickStart.App.Settings
{
	public class UserSettings
	{
		private readonly string _settingsPath;
		private Dictionary<string, string> _settings;

		public UserSettings(string settingsPath = "settings.json")
		{
			_settingsPath = settingsPath;
			LoadSettings();
		}

		private void LoadSettings()
		{
			if (File.Exists(_settingsPath))
			{
				_settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(_settingsPath));
			}
			else
			{
				_settings = new Dictionary<string, string>
				{
					{ "UnityInstallPath", "C:\\Program Files\\Unity\\Hub\\Editor" }
				};
				SaveSettings();
			}
		}

		private void SaveSettings()
		{
			File.WriteAllText(_settingsPath, JsonConvert.SerializeObject(_settings, Formatting.Indented));
		}

		public string GetUnityInstallPath()
		{
			return _settings["UnityInstallPath"];
		}

		public void SetUnityInstallPath(string newPath)
		{
			_settings["UnityInstallPath"] = newPath;
			SaveSettings();
		}
	}
}