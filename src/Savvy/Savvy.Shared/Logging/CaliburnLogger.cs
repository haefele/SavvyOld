using System;
using Caliburn.Micro;
using Serilog;

namespace Savvy.Logging
{
    public class CaliburnLogger : ILog
    {
        private readonly ILogger _logger;

        public CaliburnLogger(ILogger logger)
        {
            this._logger = logger;
        }

        public void Info(string format, params object[] args)
        {
            this._logger.Information(format, args);
        }

        public void Warn(string format, params object[] args)
        {
            this._logger.Warning(format, args);
        }

        public void Error(Exception exception)
        {
            this._logger.Error(exception, string.Empty);
        }
    }
}