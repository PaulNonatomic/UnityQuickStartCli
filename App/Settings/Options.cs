using CommandLine;

namespace UnityQuickStart.App.Settings;

public class Options
{
	[Option('p', "path", Required = false, HelpText = "Set the Unity installation path.")]
	public string? InstallPath { get; set; }
	
	[Option('c', "clear", Required = false, HelpText = "Resets the settings to default.")]
	public bool Clear { get; set; }
}