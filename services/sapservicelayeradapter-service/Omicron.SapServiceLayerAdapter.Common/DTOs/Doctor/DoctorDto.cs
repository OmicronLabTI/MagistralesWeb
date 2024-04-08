// <summary>
// <copyright file="DoctorDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.Doctor
{
    /// <summary>
    /// Class DoctorDto.
    /// </summary>
    public class DoctorDto
    {
        /// <summary>
        /// Gets or sets DoctorId.
        /// </summary>
        /// <value>The DoctorId.</value>
        [JsonProperty("CardCode")]
        public string DoctorId { get; set; }

        // <summary>
        /// Gets or sets PriceListNum.
        /// </summary>
        /// <value>The PriceListNum.</value>
        [JsonProperty("PriceListNum")]
        public int ListNum { get; set; }

        /// <summary>
        /// Gets or sets CardName.
        /// </summary>
        /// <value>The CardName.</value>
        [JsonProperty("CardName")]
        public string CardName { get; set; }

        /// <summary>
        /// Gets or sets CardType.
        /// </summary>
        /// <value>The CardType.</value>
        [JsonProperty("CardType")]
        public string CardType { get; set; }

        /// <summary>
        /// Gets or sets Address.
        /// </summary>
        /// <value>The Address.</value>
        [JsonProperty("Address")]
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets ZipCode.
        /// </summary>
        /// <value>The ZipCode.</value>
        [JsonProperty("ZipCode")]
        public string ZipCode { get; set; }

        /// <summary>
        /// Gets or sets MailAddress.
        /// </summary>
        /// <value>The MailAddress.</value>
        [JsonProperty("MailAddress")]
        public string MailAddress { get; set; }

        /// <summary>
        /// Gets or sets MailZipCode.
        /// </summary>
        /// <value>The MailZipCode.</value>
        [JsonProperty("MailZipCode")]
        public string MailZipCode { get; set; }

        /// <summary>
        /// Gets or sets Phone1.
        /// </summary>
        /// <value>The Phone1.</value>
        [JsonProperty("Phone1")]
        public string Phone1 { get; set; }

        /// <summary>
        /// Gets or sets Phone2.
        /// </summary>
        /// <value>The Phone2.</value>
        [JsonProperty("Phone2")]
        public string Phone2 { get; set; }

        /// <summary>
        /// Gets or sets ContactPerson.
        /// </summary>
        /// <value>The ContactPerson.</value>
        [JsonProperty("ContactPerson")]
        public string ContactPerson { get; set; }

        /// <summary>
        /// Gets or sets Notes.
        /// </summary>
        /// <value>The Notes.</value>
        [JsonProperty("Notes")]
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets TaxID.
        /// </summary>
        /// <value>The TaxID.</value>
        [JsonProperty("FederalTaxID")]
        public string TaxID { get; set; }

        /// <summary>
        /// Gets or sets SalesPersonCode.
        /// </summary>
        /// <value>The SalesPersonCode.</value>
        [JsonProperty("SalesPersonCode")]
        public string SalesPersonCode { get; set; }

        /// <summary>
        /// Gets or sets Currency.
        /// </summary>
        /// <value>The Currency.</value>
        [JsonProperty("Currency")]
        public string Currency { get; set; }

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
        /// Gets or sets MailCity.
        /// </summary>
        /// <value>The MailCity.</value>
        [JsonProperty("MailCity")]
        public string MailCity { get; set; }

        /// <summary>
        /// Gets or sets MailCountry.
        /// </summary>
        /// <value>The MailCountry.</value>
        [JsonProperty("MailCountry")]
        public string MailCountry { get; set; }

        /// <summary>
        /// Gets or sets EmailAddress.
        /// </summary>
        /// <value>The EmailAddress.</value>
        [JsonProperty("EmailAddress")]
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets CardForeignName.
        /// </summary>
        /// <value>The CardForeignName.</value>
        [JsonProperty("CardForeignName")]
        public string CardForeignName { get; set; }

        /// <summary>
        /// Gets or sets Block.
        /// </summary>
        /// <value>The Block.</value>
        [JsonProperty("Block")]
        public string Block { get; set; }

        /// <summary>
        /// Gets or sets BillToState.
        /// </summary>
        /// <value>The BillToState.</value>
        [JsonProperty("BillToState")]
        public string BillToState { get; set; }

        /// <summary>
        /// Gets or sets ShipToState.
        /// </summary>
        /// <value>The ShipToState.</value>
        [JsonProperty("ShipToState")]
        public string ShipToState { get; set; }

        /// <summary>
        /// Gets or sets ShipToDefault.
        /// </summary>
        /// <value>The ShipToDefault.</value>
        [JsonProperty("ShipToDefault")]
        public string ShipToDefault { get; set; }

        /// <summary>
        /// Gets or sets ShipToDefault.
        /// </summary>
        /// <value>The ShipToDefault.</value>
        [JsonProperty("Valid")]
        public string Valid { get; set; }

        /// <summary>
        /// Gets or sets Addresses.
        /// </summary>
        /// <value>The Addresses.</value>
        [JsonProperty("BPAddresses")]
        public List<DoctorAddressDto> Addresses { get; set; }

        /// <summary>
        /// Gets or sets ContactEmployees.
        /// </summary>
        /// <value>The ContactEmployees.</value>
        [JsonProperty("ContactEmployees")]
        public List<DoctorEmployeeDto> ContactEmployees { get; set; }
    }
}