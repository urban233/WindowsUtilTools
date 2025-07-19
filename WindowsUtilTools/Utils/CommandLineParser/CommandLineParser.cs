using System.Reflection;
using System.Text;

namespace Utils.CommandLineParser;

public class CommandLineParser<T> where T : class, new()
    {
        private readonly List<OptionInfo> _options;
        private readonly string _applicationName;

        public CommandLineParser(string applicationName = null)
        {
            _applicationName = applicationName ?? Assembly.GetEntryAssembly()?.GetName().Name ?? "Application";
            _options = ExtractOptionsFromType();
        }

        public T Parse(string[] args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            var result = new T();
            var parsedArgs = new HashSet<string>();
            var arguments = new Queue<string>(args);

            // Check for help request
            if (args.Length == 0 || args.Any(arg => arg == "-h" || arg == "--help" || arg == "/?"))
            {
                ShowHelp();
                Environment.Exit(0);
            }

            while (arguments.Count > 0)
            {
                var arg = arguments.Dequeue();
                
                if (arg.StartsWith("--"))
                {
                    ParseLongOption(arg, arguments, result, parsedArgs);
                }
                else if (arg.StartsWith("-") && arg.Length > 1)
                {
                    ParseShortOptions(arg, arguments, result, parsedArgs);
                }
                else
                {
                    throw new CommandLineParseException($"Unexpected argument: {arg}");
                }
            }

            // Validate required options
            ValidateRequiredOptions(parsedArgs);
            
            // Set default values for unparsed options
            SetDefaultValues(result, parsedArgs);

            return result;
        }

        private List<OptionInfo> ExtractOptionsFromType()
        {
            var options = new List<OptionInfo>();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                var attr = property.GetCustomAttribute<CommandLineOptionAttribute>();
                if (attr != null)
                {
                    options.Add(new OptionInfo
                    {
                        Property = property,
                        Attribute = attr,
                        ShortName = attr.ShortName,
                        LongName = attr.LongName ?? property.Name.ToLowerInvariant(),
                        Description = attr.Description ?? $"Set {property.Name}",
                        Required = attr.Required,
                        DefaultValue = attr.DefaultValue
                    });
                }
            }

            return options;
        }

        private void ParseLongOption(string arg, Queue<string> arguments, T result, HashSet<string> parsedArgs)
        {
            string optionName, optionValue = null;
            
            if (arg.Contains("="))
            {
                var parts = arg.Split('=', 2);
                optionName = parts[0].Substring(2);
                optionValue = parts[1];
            }
            else
            {
                optionName = arg.Substring(2);
            }

            var option = _options.FirstOrDefault(o => o.LongName.Equals(optionName, StringComparison.OrdinalIgnoreCase));
            if (option == null)
            {
                throw new CommandLineParseException($"Unknown option: --{optionName}");
            }

            SetOptionValue(option, optionValue, arguments, result, parsedArgs);
        }

        private void ParseShortOptions(string arg, Queue<string> arguments, T result, HashSet<string> parsedArgs)
        {
            var shortOptions = arg.Substring(1);
            
            for (int i = 0; i < shortOptions.Length; i++)
            {
                var shortName = shortOptions[i].ToString();
                var option = _options.FirstOrDefault(o => o.ShortName?.Equals(shortName, StringComparison.OrdinalIgnoreCase) == true);
                
                if (option == null)
                {
                    throw new CommandLineParseException($"Unknown option: -{shortName}");
                }

                string optionValue = null;
                
                // For non-boolean options, check if value is attached or next argument
                if (option.Property.PropertyType != typeof(bool) && option.Property.PropertyType != typeof(bool?))
                {
                    if (i < shortOptions.Length - 1)
                    {
                        // Value might be attached (e.g., -fvalue)
                        optionValue = shortOptions.Substring(i + 1);
                        i = shortOptions.Length; // Skip remaining characters
                    }
                }

                SetOptionValue(option, optionValue, arguments, result, parsedArgs);
            }
        }

        private void SetOptionValue(OptionInfo option, string value, Queue<string> arguments, T result, HashSet<string> parsedArgs)
        {
            var propertyType = option.Property.PropertyType;
            var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
            
            try
            {
                if (underlyingType == typeof(bool))
                {
                    // Boolean flags don't require values
                    option.Property.SetValue(result, true);
                }
                else
                {
                    // Get value if not provided
                    if (value == null)
                    {
                        if (arguments.Count == 0)
                        {
                            throw new CommandLineParseException($"Option {GetOptionDisplayName(option)} requires a value");
                        }
                        value = arguments.Dequeue();
                    }

                    // Convert and set value
                    var convertedValue = ConvertValue(value, underlyingType, option);
                    option.Property.SetValue(result, convertedValue);
                }

                parsedArgs.Add(option.LongName);
            }
            catch (Exception ex) when (!(ex is CommandLineParseException))
            {
                throw new CommandLineParseException($"Failed to set value for {GetOptionDisplayName(option)}: {ex.Message}", ex);
            }
        }

        private object ConvertValue(string value, Type targetType, OptionInfo option)
        {
            try
            {
                if (targetType.IsEnum)
                {
                    return Enum.Parse(targetType, value, true);
                }
                
                if (targetType == typeof(string))
                {
                    return value;
                }
                
                if (targetType == typeof(int))
                {
                    return int.Parse(value);
                }
                
                if (targetType == typeof(long))
                {
                    return long.Parse(value);
                }
                
                if (targetType == typeof(double))
                {
                    return double.Parse(value);
                }
                
                if (targetType == typeof(decimal))
                {
                    return decimal.Parse(value);
                }
                
                if (targetType == typeof(DateTime))
                {
                    return DateTime.Parse(value);
                }
                
                if (targetType == typeof(TimeSpan))
                {
                    return TimeSpan.Parse(value);
                }

                // Use Convert.ChangeType for other types
                return Convert.ChangeType(value, targetType);
            }
            catch (Exception ex)
            {
                throw new CommandLineParseException($"Cannot convert '{value}' to {targetType.Name} for {GetOptionDisplayName(option)}", ex);
            }
        }

        private void ValidateRequiredOptions(HashSet<string> parsedArgs)
        {
            var missingRequired = _options
                .Where(o => o.Required && !parsedArgs.Contains(o.LongName))
                .ToList();

            if (missingRequired.Any())
            {
                var missing = string.Join(", ", missingRequired.Select(GetOptionDisplayName));
                throw new CommandLineParseException($"Missing required options: {missing}");
            }
        }

        private void SetDefaultValues(T result, HashSet<string> parsedArgs)
        {
            foreach (var option in _options.Where(o => !parsedArgs.Contains(o.LongName) && o.DefaultValue != null))
            {
                option.Property.SetValue(result, option.DefaultValue);
            }
        }

        private string GetOptionDisplayName(OptionInfo option)
        {
            var names = new List<string>();
            if (!string.IsNullOrEmpty(option.ShortName))
                names.Add($"-{option.ShortName}");
            names.Add($"--{option.LongName}");
            return string.Join(", ", names);
        }

        public void ShowHelp()
        {
            var help = new StringBuilder();
            help.AppendLine($"Usage: {_applicationName} [OPTIONS]");
            help.AppendLine();
            help.AppendLine("Options:");

            var maxOptionLength = _options.Max(o => GetOptionDisplayName(o).Length);
            
            foreach (var option in _options.OrderBy(o => o.LongName))
            {
                var optionDisplay = GetOptionDisplayName(option);
                var padding = new string(' ', maxOptionLength - optionDisplay.Length + 2);
                var description = option.Description;
                
                if (option.Required)
                    description += " (Required)";
                else if (option.DefaultValue != null)
                    description += $" (Default: {option.DefaultValue})";

                help.AppendLine($"  {optionDisplay}{padding}{description}");
            }

            help.AppendLine();
            help.AppendLine("  -h, --help              Show this help message");

            Console.WriteLine(help.ToString());
        }

        private class OptionInfo
        {
            public PropertyInfo Property { get; set; }
            public CommandLineOptionAttribute Attribute { get; set; }
            public string ShortName { get; set; }
            public string LongName { get; set; }
            public string Description { get; set; }
            public bool Required { get; set; }
            public object DefaultValue { get; set; }
        }
    }
    