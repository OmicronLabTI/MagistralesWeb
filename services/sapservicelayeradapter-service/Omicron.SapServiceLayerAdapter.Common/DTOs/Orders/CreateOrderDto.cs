// <summary>
// <copyright file="CreateOrderDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.Orders
{
    /// <summary>
    /// The class for the OrderDto.
    /// </summary>
    public class CreateOrderDto : BaseOrderDto
    {
        /// <summary>
        /// Gets or sets the DocumentDate.
        /// </summary>
        /// <value>Document Date.</value>
        [JsonProperty("DocDate")]
        public DateTime DocumentDate { get; set; }

        /// <summary>
        /// Gets or sets the Card Code.
        /// </summary>
        /// <value>CardCode.</value>
        [JsonProperty("CardCode")]
        public string CardCode { get; set; }

        /// <summary>
        /// Gets or sets the Reference Number.
        /// </summary>
        /// <value>Reference Number.</value>
        [JsonProperty("NumAtCard")]
        public string ReferenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the Due Date.
        /// </summary>
        /// <value>Due Date.</value>
        [JsonProperty("DocDueDate")]
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Gets or sets if is omigenomics.
        /// </summary>
        /// <value>Is Omigenomics.</value>
        [JsonProperty("U_Omigenomicstp")]
        public string IsSecondary { get; set; }

        /// <summary>
        /// Gets or sets the Order Lines.
        /// </summary>
        /// <value>Order Lines.</value>
        [JsonProperty("DocumentLines")]
        public List<CreateOrderLineDto> OrderLines { get; set; }

        /// <summary>
        /// Gets or sets the ShipToCode.
        /// </summary>
        /// <value>Ship To Code.</value>
        [JsonProperty("ShipToCode")]
        public string ShippingCode { get; set; }

        /// <summary>
        /// Gets or sets the Sales Person Code.
        /// </summary>
        /// <value>Sales Person Code.</value>
        [JsonProperty("SalesPersonCode")]
        public int? SalesPersonCode { get; set; }

        /// <summary>
        /// Gets or sets the Owner code.
        /// </summary>
        /// <value>Documents Owner.</value>
        [JsonProperty("DocumentsOwner")]
        public int? DocumentsOwner { get; set; }

        /// <summary>
        /// Gets or sets the Owner code.
        /// </summary>
        /// <value>Documents Owner.</value>
        [JsonProperty("ContactPersonCode")]
        public int? ContactPersonCode { get; set; }

        /// <summary>
        /// Gets or sets the Client Type Order.
        /// </summary>
        /// <value>Client Type Order.</value>
        [JsonProperty("U_Paciente")]
        public string ClientTypeOrder { get; set; }

        /// <summary>
        /// Gets or sets the OrderComments.
        /// </summary>
        /// <value>Order Comments.</value>
        [JsonProperty("Comments")]
        public string OrderComments { get; set; }

        /// <summary>
        /// Gets or sets if is omigenomics.
        /// </summary>
        /// <value> Is Omigenomics. </value>
        [JsonProperty("U_catagomigenomics")]
        public string IsOmigenomics { get; set; }

        /// <summary>
        /// Gets or sets the OrderComments.
        /// </summary>
        /// <value>Order Comments.</value>
        [JsonProperty("U_TipoPedido")]
        public string TipoPedido { get; set; }
    }
}
