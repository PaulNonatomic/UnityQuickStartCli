using System.Collections.Generic;

namespace UnityQuickStart.App.IO
{
    public class CommandLineArgs
    {
        private readonly Dictionary<string, string> _params;
        private readonly Dictionary<string, string> _aliases;

        public CommandLineArgs(string[] args)
        {
            _params = new Dictionary<string, string>();
            _aliases = new Dictionary<string, string>
            {
                { "h", "help" },
                { "p", "path" },
                { "c", "clear" },
                { "v", "version" },
            };

            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                if (!arg.StartsWith("-"))
                {
                    continue;
                }

                string key;
                string value;

                if (arg.StartsWith("-") && arg.Length == 2 && i + 1 < args.Length && !args[i + 1].StartsWith("-"))
                {
                    key = arg[1..];
                    value = args[++i];
                }
                else if (arg.StartsWith("--"))
                {
                    var parts = arg[2..].Split(new[] { '=' }, 2);
                    key = parts[0];
                    value = parts.Length > 1 ? parts[1] : string.Empty;
                    if (string.IsNullOrEmpty(value) && i + 1 < args.Length && !args[i + 1].StartsWith("-"))
                    {
                        value = args[++i];
                    }
                }
                else
                {
                    key = arg[1..];
                    value = string.Empty;
                }

                if (_aliases.TryGetValue(key, out var alias))
                {
                    key = alias;
                }

                _params[key.ToLower()] = value;
            }
        }

        public bool Contains(string arg)
        {
            return _params.ContainsKey(arg.ToLower());
        }

        public string Get(string arg, string defaultValue = "")
        {
            return _params.TryGetValue(arg.ToLower(), out var param) ? param : defaultValue;
        }

        public int GetInt(string arg, int defaultValue = 0)
        {
            return int.TryParse(Get(arg), out var intValue) ? intValue : defaultValue;
        }

        public float GetFloat(string arg, float defaultValue = 0)
        {
            return float.TryParse(Get(arg), out var floatValue) ? floatValue : defaultValue;
        }

        public bool GetBool(string arg, bool defaultValue = false)
        {
            return bool.TryParse(Get(arg), out var boolValue) ? boolValue : defaultValue;
        }
    }
}
