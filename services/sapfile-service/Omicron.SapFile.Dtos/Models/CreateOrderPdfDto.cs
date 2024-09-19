// <summary>
// <copyright file="CreateOrderPdfDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapFile.Dtos.Models
{

    /// <summary>
    /// Class to generate Pdfs.
    /// </summary>
    public class CreateOrderPdfDto
	{
        /// <summary>
        /// Gets or sets OrderId.
        /// </summary>
        /// <value>The OrderId.</value>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets ClientType.
        /// </summary>
        /// <value>The ClientType.</value>
        public string ClientType { get; set; }
    }
}

