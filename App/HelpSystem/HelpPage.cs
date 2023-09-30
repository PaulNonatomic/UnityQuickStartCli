namespace UnityQuickStart.App.HelpSystem
{
	public class HelpPage
	{
		public static void Display()
		{
			Console.WriteLine("UnityQuickStart.Cli Help Page");
			Console.WriteLine("=============================");
			Console.WriteLine();

			// Description
			Console.WriteLine("Description:");
			Console.WriteLine("  A CLI tool to automate the setup and creation of Unity projects, local and remote Git repositories.");
			Console.WriteLine();

			// Usage
			Console.WriteLine("Usage:");
			Console.WriteLine("  unityquick [options]");
			Console.WriteLine();

			// Options
			Console.WriteLine("Options:");
			Console.WriteLine("  --help                  Show this help page.");
			Console.WriteLine("  --set-unity-path        Set the Unity installation path.");
			Console.WriteLine();

			// Examples
			Console.WriteLine("Examples:");
			Console.WriteLine(@"  unityquick --set-unity-path C:\Program Files\Unity\Hub\Editor");
			Console.WriteLine();
		}
	}
}