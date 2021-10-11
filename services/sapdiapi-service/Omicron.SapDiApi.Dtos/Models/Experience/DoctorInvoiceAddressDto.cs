// <summary>
// <copyright file="DoctorInvoiceAddressDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapDiApi.Dtos.Models.Experience
{
    /// <summary>
    /// class for invoice address.
    /// </summary>
    public class DoctorInvoiceAddressDto
    {
        /// <summary>
        /// Gets or sets the doctor id.
        /// </summary>
        /// <value>Doctor Id.</value>
        public string DoctorId { get; set; }

        /// <summary>
        /// Gets or sets the doctor id.
        /// </summary>
        /// <value>Doctor Id.</value>
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets the zipcode.
        /// </summary>
        /// <value>Zip code.</value>
        public string NickName { get; set; }

        /// <summary>
        /// Gets or sets the zipcode.
        /// </summary>
        /// <value>Zip code.</value>
        public string BussinessName { get; set; }

        /// <summary>
        /// Gets or sets the zipcode.
        /// </summary>
        /// <value>Zip code.</value>
        public string Rfc { get; set; }

        /// <summary>
        /// Gets or sets the zipcode.
        /// </summary>
        /// <value>Zip code.</value>
        public string ZipCode { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>Zip code.</value>
        public string Email { get; set; }
    }
}
