// <summary>
// <copyright file="FileResultModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Reporting.Entities.Model
{
    using System.IO;

    /// <summary>
    /// The file result model.
    /// </summary>
    public class FileResultModel
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public int Code { get; set; }

        /// <summary>
        /// Gets or sets User Error.
        /// </summary>
        /// <value>The user error.</value>
        public string UserError { get; set; }

        /// <summary>
        /// Gets or sets exception message.
        /// </summary>
        /// <value>The exception message.</value>
        public string ExceptionMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Success.
        /// </summary>
        /// <value><c>true</c> if success; otherwise, <c>false</c>.</value>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets memory stream.
        /// </summary>
        /// <value>The memory stream.</value>
        public MemoryStream FileStream { get; set; }

        /// <summary>
        /// Gets or sets file name.
        /// </summary>
        /// <value>The file name.</value>
        public string FileName { get; set; }
    }
}