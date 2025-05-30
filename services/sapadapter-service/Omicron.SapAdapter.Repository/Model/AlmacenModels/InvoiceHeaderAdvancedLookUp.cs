// <summary>
// <copyright file="InvoiceHeaderAdvancedLookUp.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Entities.Model.AlmacenModels
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Gets the almacen invoice header.
    /// </summary>
    public class InvoiceHeaderAdvancedLookUp
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public int Invoice { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string ProductType { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string Doctor { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string Client { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public DateTime InvoiceDocDate { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public int TotalDeliveries { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public int TotalProducts { get; set; }

    /// <summary>
        /// Gets or sets TotalPieces.
        /// </summary>
        /// <value>
        /// Int TotalPieces.
        /// </value>
        public int TotalPackedProducts { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public int DocEntry { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public int DeliverId { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public int SalesOrder { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string StatusDelivery { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public DateTime? DataCheckin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public bool IsLookUpInvoices { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string DeliveredBy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string DeliveryGuyName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string ReasonNotDelivered { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string TrakingNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string DeliveryCompany { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string CodeClient { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public bool IsRefactura { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string TypeOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public bool IsPackage { get; set; }

        /// <summary>
        /// Gets or sets the establishment name.
        /// </summary>
        /// <value>Establishment name.</value>
        public string EtablishmentName { get; set; }

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
        public string DoctorPhone { get; set; }

        /// <summary>
        /// Gets or sets the doctor degree type.
        /// </summary>
        /// <value>Degree Type.</value>
        public string IsDeliveredInOffice { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public List<BoxModel> Boxes { get; set; }

        /// <summary>
        /// Gets or sets the type Order.
        /// </summary>
        /// <value>
        /// type order.
        public string Packer { get; set; }

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

        /// <summary>
        /// Gets or sets OrderList.
        /// </summary>
        /// <value>
        /// String OrderList.
        /// </value>
        public string OrderList { get; set; }

        /// <summary>
        /// Gets or sets RemissionList.
        /// </summary>
        /// <value>
        /// String RemissionList.
        /// </value>
        public string RemissionList { get; set; }

        /// <summary>
        /// Gets or sets TotalPieces.
        /// </summary>
        /// <value>
        /// Int TotalPieces.
        /// </value>
        public int TotalPieces { get; set; }

        /// <summary>
        /// Gets or sets TotalPieces.
        /// </summary>
        /// <value>
        /// Int TotalPieces.
        /// </value>
        public int TotalPackedPieces { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets is doctor direction.
        /// </summary>
        /// <value>
        /// ItemCode.
        /// </value>
        public bool IsDoctorDirection { get; set; }

       /// <summary>
        /// Gets or sets TotalpackedDeliveries.
        /// </summary>
        /// <value>
        /// Int TotalpackedDeliveries.
        /// </value>
        public int TotalPackedDeliveries { get; set; }
    }
}
