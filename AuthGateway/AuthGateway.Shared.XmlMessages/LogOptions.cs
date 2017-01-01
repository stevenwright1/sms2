using System;
using AuthGateway.Shared.Log;

namespace AuthGateway.Shared.XmlMessages
{
    public class LogOptionsCommandBase : CommandBase
    {
        public LogLevel Level = LogLevel.DebugVerbose; 
    }

    public class LogOptionsRetBase : RetBase
    {
        public LogLevel Level = LogLevel.DebugVerbose; 
    }
}
