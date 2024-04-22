// <summary>
// <copyright file="BatchNumberDetailDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.BatchNumberDetails
{
    /// <summary>
    /// the class BatchNumberDetailDto.
    /// </summary>
    public class BatchNumberDetailDto
    {
        /// <summary>
        /// Gets or sets the ItemCode.
        /// </summary>
        /// <value>ItemCode.</value>
        [JsonProperty("ItemCode")]
        public string ItemCode { get; set; }

        /// <summary>
        /// Gets or sets the ItemDescription.
        /// </summary>
        /// <value>ItemDescription.</value>
        [JsonProperty("ItemDescription")]
        public string ItemDescription { get; set; }

        /// <summary>
        /// Gets or sets the Status.
        /// </summary>
        /// <value>Status.</value>
        [JsonProperty("Status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the Batch.
        /// </summary>
        /// <value>Batch.</value>
        [JsonProperty("Batch")]
        public string Batch { get; set; }

        /// <summary>
        /// Gets or sets the AdmissionDate.
        /// </summary>
        /// <value>AdmissionDate.</value>
        [JsonProperty("AdmissionDate")]
        public string AdmissionDate { get; set; }

        /// <summary>
        /// Gets or sets the Certificate.
        /// </summary>
        /// <value>Certificate.</value>
        [JsonProperty("U_Certificado")]
        public string Certificate { get; set; }

        /// <summary>
        /// Gets or sets the DocEntry.
        /// </summary>
        /// <value>DocEntry.</value>
        [JsonProperty("DocEntry")]
        public int DocEntry { get; set; }
    }
}