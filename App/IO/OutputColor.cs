namespace UnityQuickStart.App.IO;

public class OutputColor
{
	public static ConsoleColor SectionHeader { get; } = ConsoleColor.Magenta;
	public static ConsoleColor Success { get; } = ConsoleColor.Green;
	public static ConsoleColor Info { get; } = ConsoleColor.Cyan;
	public static ConsoleColor Warning { get; } = ConsoleColor.Yellow;
	public static ConsoleColor Error { get; } = ConsoleColor.Red;
	public static ConsoleColor Hint { get; } = ConsoleColor.DarkCyan ;
}