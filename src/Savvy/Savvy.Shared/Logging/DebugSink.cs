using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Serilog.Events;

namespace Savvy.Logging
{
    public class DebugSink : Serilog.Core.ILogEventSink
    {
        public void Emit(LogEvent logEvent)
        {
            Debug.WriteLine(logEvent.RenderMessage());
        }
    }
}
