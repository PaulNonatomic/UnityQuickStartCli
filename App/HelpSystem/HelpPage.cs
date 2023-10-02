using UnityQuickStart.App.IO;
using UnityQuickStart.App.Settings;

namespace UnityQuickStart.App.HelpSystem
{
	public class HelpPage
	{
		public static void Display()
		{
			Output.WriteSectionHeader("UnityQuickStart.Cli Help Page");
			Output.WriteLine();

			// Description
			Output.WriteSectionHeader("Description:");
			Output.Write("  A CLI tool to automate the setup and creation of Unity projects, local and remote Git repositories.");
			Output.WriteLine();

			// Usage
			Output.WriteSectionHeader("Usage:");
			Output.WriteLine("  unityquick [options]");
			Output.WriteLine();

			// Options
			Output.WriteSectionHeader("Options:");
			Output.WriteLine("  -h --help     Show the help page.");
			Output.WriteLine("  -p --path     Set the Unity installation path.");
			Output.WriteLine("  -c --clear    Resets the settings to default.");
			Output.WriteLine("  -v --version  Outputs the UnityQuickStart version.");
			Output.WriteLine();
				
			// Examples
			Output.WriteSectionHeader("Examples:");
			Output.WriteLine($@"  unityquick --set-unity-path {Constants.DefaultUnityPath}");
			Output.WriteLine();
		}
	}
}