using System.Reflection.Metadata.Ecma335;

namespace TpsEbReader;

public class Logger
{
    private readonly ErrorLevel _loggerLevel;

    public Logger(ErrorLevel loggerLevel)
    {
        _loggerLevel = loggerLevel;
    }


    public void LogMessage(ErrorLevel errorLevel, string message, Exception? ex = null)
    {
        if (_loggerLevel > errorLevel) return;
        Console.WriteLine(ex != null
            ? $"{errorLevel}: {message}\nStackTrace\n{ex.StackTrace}"
            : $"{errorLevel}: {message}");
    }
}