namespace TpsEbReader;

public class Logger
{
    public void LogMessage(ErrorLevel errorLevel, string message, Exception ex = null)
    {
        if (ex != null)
        {
            Console.WriteLine($"{errorLevel}: {message}\nStackTrace\n{ex.StackTrace}");
        }
        else
        {
            Console.WriteLine($"{errorLevel}: {message}");
        }
    }
}