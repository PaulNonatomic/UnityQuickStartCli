namespace UnityQuickStart.App.Input
{
	public class CommandLineArgs
	{
		public Dictionary<string, string> NamedArguments { get; private set; }
		public List<string> PositionalArguments { get; private set; }
		public HashSet<string> Flags { get; private set; }

		public CommandLineArgs(string[] args)
		{
			NamedArguments = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			PositionalArguments = new List<string>();
			Flags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

			Parse(args);
		}

		private void Parse(string[] args)
		{
			for (var i = 0; i < args.Length; i++)
			{
				var arg = args[i];
				if (arg.StartsWith("--"))
				{
					var key = arg[2..];
					if (i + 1 < args.Length && !args[i + 1].StartsWith("-"))
					{
						NamedArguments[key] = args[++i];
					}
					else
					{
						Flags.Add(key);
					}
				}
				else if (arg.StartsWith("-"))
				{
					var key = arg[1..];
					Flags.Add(key);
				}
				else
				{
					PositionalArguments.Add(arg);
				}
			}
		}

		public bool TryGetNamedArgument(string key, out string value)
		{
			return NamedArguments.TryGetValue(key, out value);
		}

		public bool HasFlag(string flag)
		{
			return Flags.Contains(flag);
		}

		public string GetPositionalArgument(int index)
		{
			return index < PositionalArguments.Count ? PositionalArguments[index] : null;
		}
	}
}