// <summary>
// <copyright file="InvoiceDeliverModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Entities.Model.AlmacenModels
{
    /// <summary>
    /// Class for the invoice deliver model.
    /// </summary>
    public class InvoiceDeliverModel
    {
        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public int PackageNumber { get; set; }

        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public string Client { get; set; }

        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public string Doctor { get; set; }

        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public string Comments { get; set; }

        /// <summary>
        /// Gets or sets FirstName.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets FirstName.
        /// </summary>
        /// <value>
        /// String FirstName.
        /// </value>
        public bool NeedsDelivery { get; set; }

        /// <summary>
        /// Gets or sets the card code.
        /// </summary>
        /// <value>Card code.</value>
        public string BetweenStreets { get; set; }

        /// <summary>
        /// Gets or sets the doctor degree type.
        /// </summary>
        /// <value>Degree Type.</value>
        public string References { get; set; }

        /// <summary>
        /// Gets or sets the doctor degree type.
        /// </summary>
        /// <value>Degree Type.</value>
        public string Telephone { get; set; }

        /// <summary>
        /// Gets or sets the doctor degree type.
        /// </summary>
        /// <value>Degree Type.</value>
        public string ResponsibleDoctor { get; set; }

        /// <summary>
        /// Gets or sets the establishment name.
        /// </summary>
        /// <value>Establishment name.</value>
        public string EstablishmentName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets.
        /// </summary>
        /// <value>
        /// Bool is production order.
        public string DestinyEmail { get; set; }

        /// <summary>
        /// Gets or sets a value sales person email.
        /// </summary>
        /// <value>
        /// Bool is production order.
        public string SalesPersonEmail { get; set; }

        /// <summary>
        /// Gets or sets a value person names.
        /// </summary>
        /// <value>
        /// Bool is production order.
        public string SalesPrsonName { get; set; }

        /// <summary>
        /// Gets or sets a value person names.
        /// </summary>
        /// <value>
        /// Bool is production order.
        public string SalesOrders { get; set; }

        /// <summary>
        /// Gets or sets a value person names.
        /// </summary>
        /// <value>
        /// Bool is production order.
        public string Contact { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        /// <value>
        /// ItemCode.
        /// </value>
        public string DeliveryComments { get; set; }

        /// <summary>
        /// Gets or sets.
        /// </summary>
        /// <value>
        /// ItemCode.
        /// </value>
        public string DeliverySuggestedTime { get; set; }
    }
}
