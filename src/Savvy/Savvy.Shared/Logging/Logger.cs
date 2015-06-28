using System;
using System.Text.RegularExpressions;
using Caliburn.Micro;
using Serilog;
using Serilog.Events;

namespace Savvy.Logging
{
    public class Logger
    {
        #region Fields
        private readonly ILogger _actualLogger;
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether debug logging is enabled.
        /// </summary>
        public bool IsDebugEnabled
        {
            get { return this._actualLogger.IsEnabled(LogEventLevel.Debug); }
        }
        /// <summary>
        /// Gets a value indicating whether information logging is enabled.
        /// </summary>
        public bool IsInformationEnabled
        {
            get { return this._actualLogger.IsEnabled(LogEventLevel.Information); }
        }
        /// <summary>
        /// Gets a value indicating whether warning logging is enabled.
        /// </summary>
        public bool IsWarningEnabled
        {
            get { return this._actualLogger.IsEnabled(LogEventLevel.Warning); }
        }
        /// <summary>
        /// Gets a value indicating whether error logging is enabled.
        /// </summary>
        public bool IsErrorEnabled
        {
            get { return this._actualLogger.IsEnabled(LogEventLevel.Error); }
        }
        /// <summary>
        /// Gets a value indicating whether fatal logging is enabled.
        /// </summary>
        public bool IsFatalEnabled
        {
            get { return this._actualLogger.IsEnabled(LogEventLevel.Fatal); }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        /// <param name="actualLogger">The actual logger.</param>
        public Logger(ILogger actualLogger)
        {
            this._actualLogger = actualLogger;
        }
        #endregion

        #region Methods
        public void Debug(string format, params object[] args)
        {
            this.ExtractMethodNameAndLineNumberThenExecute(format, (logger, newFormat) =>
                logger.Debug(newFormat, args));
        }

        public void Debug(Exception exception, string format, params object[] args)
        {
            this.ExtractMethodNameAndLineNumberThenExecute(format, (logger, newFormat) =>
                logger.Debug(exception, newFormat, args));
        }

        public void Information(string format, params object[] args)
        {
            this.ExtractMethodNameAndLineNumberThenExecute(format, (logger, newFormat) =>
                logger.Information(newFormat, args));
        }

        public void Information(Exception exception, string format, params object[] args)
        {
            this.ExtractMethodNameAndLineNumberThenExecute(format, (logger, newFormat) =>
                logger.Information(exception, newFormat, args));
        }

        public void Warning(string format, params object[] args)
        {
            this.ExtractMethodNameAndLineNumberThenExecute(format, (logger, newFormat) =>
                logger.Warning(newFormat, args));
        }

        public void Warning(Exception exception, string format, params object[] args)
        {
            this.ExtractMethodNameAndLineNumberThenExecute(format, (logger, newFormat) =>
                logger.Warning(exception, newFormat, args));
        }

        public void Error(string format, params object[] args)
        {
            this.ExtractMethodNameAndLineNumberThenExecute(format, (logger, newFormat) =>
                logger.Error(newFormat, args));
        }

        public void Error(Exception exception, string format, params object[] args)
        {
            this.ExtractMethodNameAndLineNumberThenExecute(format, (logger, newFormat) =>
                logger.Error(exception, newFormat, args));
        }

        public void Fatal(string format, params object[] args)
        {
            this.ExtractMethodNameAndLineNumberThenExecute(format, (logger, newFormat) =>
                logger.Fatal(newFormat, args));
        }

        public void Fatal(Exception exception, string format, params object[] args)
        {
            this.ExtractMethodNameAndLineNumberThenExecute(format, (logger, newFormat) =>
                logger.Fatal(exception, newFormat, args));
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Extracts the method name and line number from the message.
        /// Then it executes the specified <paramref name="loggerAction"/>.
        /// </summary>
        /// <param name="format">The message.</param>
        /// <param name="loggerAction">The logger action.</param>
        private void ExtractMethodNameAndLineNumberThenExecute(string format, Action<ILogger, string> loggerAction)
        {
            string methodName = this.ExtractMethodName(format);
            string lineNumber = this.ExtractLineNumber(format);

            string newFormat = this.RemoveMethodNameAndLineNumber(format);
            var logger = this._actualLogger
                .ForContext("Method", methodName)
                .ForContext("Line", lineNumber);

            loggerAction(logger, newFormat);
        }
        /// <summary>
        /// Extracts the method name from the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        private string ExtractMethodName(string message)
        {
            var match = Regex.Match(message, @"Method: '(.*)'\..*");
            return match.Groups[1].Value;
        }
        /// <summary>
        /// Extracts the line number from the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        private string ExtractLineNumber(string message)
        {
            var match = Regex.Match(message, @".*\. Line: (.*)\..*");
            return match.Groups[1].Value;
        }
        /// <summary>
        /// Removes the method name and line number.
        /// </summary>
        /// <param name="message">The message.</param>
        private string RemoveMethodNameAndLineNumber(string message)
        {
            var lineNumber = this.ExtractLineNumber(message);
            var index = message.IndexOf(lineNumber, StringComparison.Ordinal);

            const int dotAndWhitespaceCount = 2;
            return message.Substring(index + lineNumber.Length + dotAndWhitespaceCount);
        }
        #endregion
    }
}