// <summary>
// <copyright file="CreateSaleOrderDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Common.DTOs.Orders
{
    /// <summary>
    /// class for sale orde model.
    /// </summary>
    public class CreateSaleOrderDto
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string PrescriptionUrl { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public List<ShoppingCartItemDto> Items { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string ShippinAddress { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string BillingAddress { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string CardCode { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the transaction record in the payment table (id field).
        /// </summary>
        /// <value>The code.</value>
        public string TransactionId { get; set; }

        /// <summary>
        /// Gets or sets the patient name.
        /// </summary>
        /// <value>The code.</value>
        public string PatientName { get; set; }

        /// <summary>
        /// Gets or sets the bool value id name is printed.
        /// </summary>
        /// <value>The code.</value>
        public int IsNamePrinted { get; set; }

        /// <summary>
        /// Gets or sets the bool value id name is printed.
        /// </summary>
        /// <value>The code.</value>
        public string ShippingCost { get; set; }

        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public string CfdiValue { get; set; }

        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public string PaymentMethodSapCode { get; set; }

        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public string WayToPaySapCode { get; set; }

        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public decimal DiscountSpecial { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets FirstName.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public bool IsPackage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets FirstName.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public string UserRfc { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets FirstName.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public string ProfecionalLicense { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets FirstName.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public bool IsSample { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the order is omigenomics.
        /// </summary>
        /// <value>Omigenomics value.</value>
        public bool? IsOmigenomicsOrder { get; set; }

        /// <summary>
        /// Gets or sets the adviser id.
        /// </summary>
        /// <value>IdDoctor.</value>
        public int? SlpCode { get; set; }

        /// <summary>
        /// Gets or sets the adviser id.
        /// </summary>
        /// <value>IdDoctor.</value>
        public int? EmployeeId { get; set; }
    }
}
