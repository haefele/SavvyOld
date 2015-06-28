using System.Collections.Generic;
using System.Text;
using Serilog;

namespace Savvy.Logging
{
    public static class LoggerFactory
    {
        static LoggerFactory()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Sink<DebugSink>()
                .CreateLogger();
        }

        public static Logger GetLogger<T>()
        {
            return new Logger(Log.ForContext<T>());
        }
    }
}
