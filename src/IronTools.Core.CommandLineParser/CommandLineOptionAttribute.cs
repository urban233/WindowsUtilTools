namespace IronTools.Core.CommandLineParser;

// Attribute to mark command line options
[AttributeUsage(AttributeTargets.Property)]
public class CommandLineOptionAttribute : Attribute
{
    public string ShortName { get; set; }
    public string LongName { get; set; }
    public string Description { get; set; }
    public bool Required { get; set; }
    public object DefaultValue { get; set; }
        
    public CommandLineOptionAttribute(string shortName = null, string longName = null)
    {
        ShortName = shortName;
        LongName = longName;
    }
}
