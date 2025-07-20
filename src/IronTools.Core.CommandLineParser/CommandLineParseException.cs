namespace IronTools.Core.CommandLineParser;

public class CommandLineParseException : Exception
{
    public CommandLineParseException(string message) : base(message) { }
    public CommandLineParseException(string message, Exception innerException) : base(message, innerException) { }
}
