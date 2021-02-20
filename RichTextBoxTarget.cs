using NLog;
using NLog.Common;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMuteUs_Portable
{
    class RichTextBoxTarget : Target
    {
        public event EventHandler<LogEventInfo> OnLog;

        public RichTextBoxTarget(string name, LogLevel minLevel, LogLevel maxLevel)
        {
            var config = LogManager.Configuration;

            config.AddTarget(name, this);
            config.AddRule(minLevel, maxLevel, name, "*");

            LogManager.Configuration = config;
        }

        [Obsolete]
        protected override void Write(AsyncLogEventInfo[] logEvents)
        {
            foreach (var logEvent in logEvents)
            {
                Write(logEvent.LogEvent);
            }
        }

        protected override void Write(AsyncLogEventInfo logEvent)
        {
            Write(logEvent.LogEvent);
        }

        protected override void Write(LogEventInfo logEvent)
        {
            OnLog(this, logEvent);
        }
    }
}
