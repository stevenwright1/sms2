using System;
using System.Collections.Generic;
using System.Text;

namespace AuthGateway.Shared.Log
{
    public interface ILogger
    {
        void Write(LogEntry message);
    }
}
