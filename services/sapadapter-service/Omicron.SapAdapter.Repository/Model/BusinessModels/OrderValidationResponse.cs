// <summary>
// <copyright file="OrderValidationResponse.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Entities.Model.BusinessModels
{
    using System.Collections.Generic;

    /// <summary>
    /// class to send the validation responses.
    /// </summary>
    public class OrderValidationResponse
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public List<string> ListItems { get; set;  }
    }
}
