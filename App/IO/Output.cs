using UnityQuickStart.App.Settings;

namespace UnityQuickStart.App.IO
{
	public class Output
	{
		public const char Tick = '\u2713';
		
		public static void WriteLine()
		{
			Console.WriteLine();
		}
		
		public static void WriteLine(string message, ConsoleColor color = ConsoleColor.White)
		{
			Console.ForegroundColor = color;
			Console.WriteLine(message);
			Console.ResetColor();
		}

		public static void Write(string message, ConsoleColor color = ConsoleColor.White)
		{
			Console.ForegroundColor = color;
			Console.Write(message);
			Console.ResetColor();
		}

		public static void WriteSuccessWithTick(string message)
		{
			WriteLine($"{message} {Tick}{Environment.NewLine}", ConsoleColor.Green);
		}
		
		public static void WriteError(string message)
		{
			WriteLine($"{message}{Environment.NewLine}", OutputColor.Error);
		}

		public static void WriteWarning(string message)
		{
			WriteLine(message, OutputColor.Warning);
		}

		public static void WriteInfo(string message)
		{
			WriteLine(message, OutputColor.Info);
		}

		public static void WriteSuccess(string message)
		{
			WriteLine(message, OutputColor.Success);
		}

		public static void WriteSectionHeader(string header)
		{
			WriteLine($"=== {header} ===", OutputColor.SectionHeader);
		}

		public static void WriteHint(string message)
		{
			WriteLine($"[{message}]", OutputColor.Hint);
		}
	}
}