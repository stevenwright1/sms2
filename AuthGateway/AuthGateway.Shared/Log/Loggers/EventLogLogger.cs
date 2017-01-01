using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceProcess;

namespace AuthGateway.Shared.Log.Loggers
{
    public class EventLogLogger : ILogger
    {
        private ServiceBase service;
        public EventLogLogger(ServiceBase service)
        {
            this.service = service;
            this.service.EventLog.Log = "Application";
        }

        public void Write(LogEntry entry)
        {
            this.service.EventLog.WriteEntry(entry.Message);
        }
    }
}
