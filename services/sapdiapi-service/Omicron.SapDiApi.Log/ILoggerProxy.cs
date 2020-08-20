// <summary>
// <copyright file="LoggerProxy.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summar
namespace Omicron.SapDiApi.Log
{
    using System;

    /// <summary>
    /// Contract of logger proxy
    /// </summary>
    public interface ILoggerProxy
    {
        /// <summary>
        /// Log message
        /// </summary>
        /// <param name="message">Message to log</param>
        void Info(string message);

        /// <summary>
        /// Log debug message
        /// </summary>
        /// <param name="message">Message to log</param>
        void Debug(string message);

        /// <summary>
        /// Log warning message
        /// </summary>
        /// <param name="message">Message to log</param>
        void Warning(string message);

        /// <summary>
        /// Log error
        /// </summary>
        /// <param name="message">Message to log</param>
        void Error(string message);

        /// <summary>
        /// Log message with exception
        /// </summary>
        /// <param name="message">Message to log</param>
        void Error(string message, Exception ex);
    }
}
