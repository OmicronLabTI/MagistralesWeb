// <summary>
// <copyright file="DoctorAddressDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.Doctor
{
    /// <summary>
    /// Class DoctorAddressDto.
    /// </summary>
    public class DoctorAddressDto
    {
        /// <summary>
        /// Gets or sets AddressName.
        /// </summary>
        /// <value>The AddressName.</value>
        [JsonProperty("AddressName")]
        public string AddressName { get; set; }

        /// <summary>
        /// Gets or sets Street.
        /// </summary>
        /// <value>The Street.</value>
        [JsonProperty("Street")]
        public string Street { get; set; }

        /// <summary>
        /// Gets or sets Block.
        /// </summary>
        /// <value>The Block.</value>
        [JsonProperty("Block")]
        public string Block { get; set; }

        /// <summary>
        /// Gets or sets ZipCode.
        /// </summary>
        /// <value>The ZipCode.</value>
        [JsonProperty("ZipCode")]
        public string ZipCode { get; set; }

        /// <summary>
        /// Gets or sets City.
        /// </summary>
        /// <value>The City.</value>
        [JsonProperty("City")]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets Country.
        /// </summary>
        /// <value>The Country.</value>
        [JsonProperty("Country")]
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets State.
        /// </summary>
        /// <value>The State.</value>
        [JsonProperty("State")]
        public string State { get; set; }

        /// <summary>
        /// Gets or sets FederalTaxID.
        /// </summary>
        /// <value>The FederalTaxID.</value>
        [JsonProperty("FederalTaxID")]
        public string FederalTaxID { get; set; }

        /// <summary>
        /// Gets or sets TaxCode.
        /// </summary>
        /// <value>The TaxCode.</value>
        [JsonProperty("TaxCode")]
        public string TaxCode { get; set; }

        /// <summary>
        /// Gets or sets AddressName2.
        /// </summary>
        /// <value>The AddressName2.</value>
        [JsonProperty("AddressName2")]
        public string AddressName2 { get; set; }

        /// <summary>
        /// Gets or sets AddressName3.
        /// </summary>
        /// <value>The AddressName3.</value>
        [JsonProperty("AddressName3")]
        public string AddressName3 { get; set; }

        /// <summary>
        /// Gets or sets StreetNo.
        /// </summary>
        /// <value>The StreetNo.</value>
        [JsonProperty("StreetNo")]
        public string StreetNo { get; set; }

        /// <summary>
        /// Gets or sets BPCode.
        /// </summary>
        /// <value>The BPCode.</value>
        [JsonProperty("BPCode")]
        public string BPCode { get; set; }

        /// <summary>
        /// Gets or sets GlobalLocationNumber.
        /// </summary>
        /// <value>The GlobalLocationNumber.</value>
        [JsonProperty("GlobalLocationNumber")]
        public string GlobalLocationNumber { get; set; }

        /// <summary>
        /// Gets or sets U_RFC.
        /// </summary>
        /// <value>The RFC.</value>
        [JsonProperty("U_RFC")]
        public string RFC { get; set; }

        /// <summary>
        /// Gets or sets Advisor.
        /// </summary>
        /// <value>The Advisor.</value>
        [JsonProperty("U_ASESOR")]
        public string Advisor { get; set; }

        /// <summary>
        /// Gets or sets Reason.
        /// </summary>
        /// <value>The Reason.</value>
        [JsonProperty("U_RAZON")]
        public string Reason { get; set; }

        /// <summary>
        /// Gets or sets AddressType.
        /// </summary>
        /// <value>The AddressType.</value>
        [JsonProperty("AddressType")]
        public string AddressType { get; set; }
    }
}