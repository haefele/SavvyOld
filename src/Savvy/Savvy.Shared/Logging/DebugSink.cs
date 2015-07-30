using System.Diagnostics;
using Serilog.Core;
using Serilog.Events;

namespace Savvy.Logging
{
    public class DebugSink : ILogEventSink
    {
        public void Emit(LogEvent logEvent)
        {
            Debug.WriteLine(logEvent.RenderMessage());
        }
    }
}
