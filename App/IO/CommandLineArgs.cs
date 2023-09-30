namespace UnityQuickStart.App.IO
{
    public class CommandLineArgs
    {
        public Dictionary<string, string> NamedArguments { get; private set; }
        public List<string> PositionalArguments { get; private set; }
        public HashSet<string> Flags { get; private set; }

        private readonly Dictionary<string, string> _aliases;

        public CommandLineArgs(string[] args, Dictionary<string, string> aliases = null)
        {
            NamedArguments = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            PositionalArguments = new List<string>();
            Flags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            _aliases = aliases ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            Parse(args);
        }

        private void Parse(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                if (arg.StartsWith("--"))
                {
                    string key = arg.Substring(2);
                    key = ResolveAlias(key);

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
                    string key = arg.Substring(1);
                    key = ResolveAlias(key);
                    Flags.Add(key);
                }
                else
                {
                    PositionalArguments.Add(arg);
                }
            }
        }

        private string ResolveAlias(string key)
        {
            return _aliases.TryGetValue(key, out string primary) ? primary : key;
        }

        public bool TryGetNamedArgument(string key, out string value)
        {
            return NamedArguments.TryGetValue(ResolveAlias(key), out value);
        }

        public bool HasFlag(string flag)
        {
            return Flags.Contains(ResolveAlias(flag));
        }

        public string GetPositionalArgument(int index)
        {
            return index < PositionalArguments.Count ? PositionalArguments[index] : null;
        }
    }
}
