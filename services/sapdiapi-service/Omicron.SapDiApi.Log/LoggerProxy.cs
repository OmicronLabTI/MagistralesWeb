// <summary>
// <copyright file="LoggerProxy.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summar
namespace Omicron.SapDiApi.Log
{
    using NLog;
    using System;

    public class LoggerProxy : ILoggerProxy
    {
        /// <summary>
        /// Logger instance
        /// </summary>
        private readonly Logger _logger;

        public LoggerProxy()
        {
            this._logger = LogManager.GetCurrentClassLogger();
        }
        
        /// <summary>
        /// Log debug message
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Debug(string message)
        {
            this._logger.Debug(message);
        }

        /// <summary>
        /// Log warning message
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Warning(string message)
        {
            this._logger.Warn(message);
        }

        /// <summary>
        /// Log error
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Error(string message)
        {
            this._logger.Error(message);
        }

        /// <summary>
        /// Log message with exception
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Error(string message, Exception ex)
        {
            this._logger.Error(ex, message);
        }
    }
}