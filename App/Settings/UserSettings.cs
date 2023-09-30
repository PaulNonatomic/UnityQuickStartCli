using Newtonsoft.Json;

namespace UnityQuickStart.App.Settings
{
	public class UserSettings
	{
		private readonly string _settingsPath;
		private Dictionary<string, string> _settings = new();
		
		private const string UnityInstallPath = "UnityInstallPath";
		private const string UnityVersion = "UnityVersion";

		public UserSettings(string settingsPath = "settings.json")
		{
			_settingsPath = settingsPath;
			
			InstantiateSettings();
			LoadSettings();
		}

		private void LoadSettings()
		{
			if (File.Exists(_settingsPath))
			{
				var settingsJson = File.ReadAllText(_settingsPath);
				var settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(settingsJson);
				if (settings == null) return;
				
				foreach (var kvp in settings)
				{
					if (_settings.ContainsKey(kvp.Key))
					{
						_settings[kvp.Key]= kvp.Value;
					}
					else
					{
						_settings.Add(kvp.Key, kvp.Value);
					}
				}
			}
			else
			{
				Clear();
			}
		}

		private void SaveSettings()
		{
			File.WriteAllText(_settingsPath, JsonConvert.SerializeObject(_settings, Formatting.Indented));
		}

		public string GetUnityInstallPath()
		{
			return _settings[UnityInstallPath];
		}

		public void SetUnityInstallPath(string newPath)
		{
			_settings[UnityInstallPath] = newPath;
			SaveSettings();
		}
		
		public string GetUnityVersion()
		{
			return _settings[UnityVersion];
		}

		public void SetUnityVersion(string version)
		{
			_settings[UnityVersion] = version;
			SaveSettings();
		}

		public void Clear()
		{
			InstantiateSettings();
			SaveSettings();
		}

		private void InstantiateSettings()
		{
			_settings = new Dictionary<string, string>
			{
				{ UnityInstallPath, string.Empty },
				{ UnityVersion, string.Empty }
			};
		}
	}
}