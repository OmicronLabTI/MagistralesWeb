// <summary>
// <copyright file="RawMaterialRequestDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Reporting.Dtos.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// Class for request raw materia.
    /// </summary>
    public class RawMaterialRequestDto
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        /// <value>
        /// Int Id.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the production order ids.
        /// </summary>
        /// <value>
        /// Id production ids.
        /// </value>
        public List<int> ProductionOrderIds { get; set; }

        /// <summary>
        /// Gets or sets signature in byte[] representation.
        /// </summary>
        /// <value>
        /// Byte[] signature.
        /// </value>
        public string Signature { get; set; }

        /// <summary>
        /// Gets or sets signing user name.
        /// </summary>
        /// <value>
        /// String user name.
        /// </value>
        public string SigningUserName { get; set; }

        /// <summary>
        /// Gets or sets signing user id.
        /// </summary>
        /// <value>
        /// String user id.
        /// </value>
        public string SigningUserId { get; set; }

        /// <summary>
        /// Gets or sets observations.
        /// </summary>
        /// <value>
        /// String observations.
        /// </value>
        public string Observations { get; set; }

        /// <summary>
        /// Gets or sets ordered products.
        /// </summary>
        /// <value>
        /// List products.
        /// </value>
        public List<RawMaterialRequestDetailDto> OrderedProducts { get; set; }

        /// <summary>
        /// Gets or sets creation user id.
        /// </summary>
        /// <value>
        /// Int user id.
        /// </value>
        public string CreationUserId { get; set; }

        /// <summary>
        /// Gets or sets creation date.
        /// </summary>
        /// <value>
        /// String creation date.
        /// </value>
        public string CreationDate { get; set; }
    }
}
