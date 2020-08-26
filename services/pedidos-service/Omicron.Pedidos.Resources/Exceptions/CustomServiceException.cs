// <summary>
// <copyright file="CustomServiceException.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.LeadToCash.Resources.Exceptions
{
    using System;
    using System.Net;
    using System.Runtime.Serialization;

    /// <summary>
    /// Class Custom Service.
    /// </summary>
    public class CustomServiceException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomServiceException"/> class.
        /// </summary>
        public CustomServiceException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomServiceException"/> class.
        /// </summary>
        /// <param name="message">Message Exception.</param>
        public CustomServiceException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomServiceException"/> class.
        /// </summary>
        /// <param name="message">Message Exception.</param>
        /// <param name="innerException">Inner Exception.</param>
        public CustomServiceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomServiceException"/> class.
        /// </summary>
        /// <param name="message">the message.</param>
        /// <param name="status">the statucs code.</param>
        public CustomServiceException(string message, HttpStatusCode status)
            : base(message)
        {
            this.Status = status;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomServiceException"/> class.
        /// </summary>
        /// <param name="message">the message.</param>
        /// <param name="status">the statucs code.</param>
        /// <param name="body">The body to send.</param>
        public CustomServiceException(string message, HttpStatusCode status, object body)
            : base(message)
        {
            this.Status = status;
            this.ResponseBody = body;
        }

        /// <summary>
        /// Gets the Status.
        /// </summary>
        /// <value>
        /// The Status.
        /// </value>
        public virtual HttpStatusCode Status { get; }

        /// <summary>
        /// Gets the Status.
        /// </summary>
        /// <value>
        /// The Status.
        /// </value>
        public virtual object ResponseBody { get; }
    }
}
