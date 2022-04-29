// <summary>
// <copyright file="ParametersCardInvoice.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Entities.Model.AlmacenModels
{
    using System;
    using System.Collections.Generic;
    using Omicron.SapAdapter.Dtos.DxpModels;
    using Omicron.SapAdapter.Entities.Model.BusinessModels;
    using Omicron.SapAdapter.Entities.Model.DbModels;
    using Omicron.SapAdapter.Entities.Model.JoinsModels;

    /// <summary>
    /// class for the advance look up cards.
    /// </summary>
    public class ParametersCardInvoice
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public List<int> ListDeliveryId { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public List<UserOrderModel> UserOrders { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public List<LineProductsModel> LineProducts { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public List<DeliveryDetailModel> DeliveryDetailModels { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public List<InvoiceHeaderModel> InvoiceHeadersToLook { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public List<InvoiceDetailModel> InvoiceDetailsToLook { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public List<string> LocalNeighbors { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>s
        /// <value>The code.</value>
        public List<PaymentsDto> Payments { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>s
        /// <value>The code.</value>
        public Tuple<int, string> LookUpTuple { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public List<DoctorDeliveryAddressModel> DeliveryAddress { get; set; }
    }
}
