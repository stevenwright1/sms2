using System;

namespace AuthGateway.Shared.Log.Loggers
{
    public class ConsoleLogger : ILogger
    {
        public void Write(LogEntry entry)
        {
            Console.WriteLine(string.Format("{0}\t{1}", entry.LogTime, entry.Message).Trim());
        }
    }
}
