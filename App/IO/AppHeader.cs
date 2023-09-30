using System.Xml;

namespace UnityQuickStart.App.IO;

public class AppHeader
{
	public static void Write()
	{
		Output.WriteSectionHeader("UnityQuickStart.Cli");
		Output.WriteLine();
	}
}