// <summary>
// <copyright file="MockResponse.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Usuarios.Test
{
    using System.Net;

    /// <summary>
    /// Abstract Class Base Test.
    /// </summary>
    public class MockResponse
    {
        /// <summary>
        /// Gets or sets the json.
        /// </summary>
        /// <value>The json.</value>
        public string Json { get; set; }

        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        /// <value>The status code.</value>
        public HttpStatusCode StatusCode { get; set; }
    }
}
