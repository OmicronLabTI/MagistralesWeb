// <copyright file="CreateiInvoiceNotification.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>

namespace Omicron.Invoice.Common.MediatR
{
    /// <summary>
    /// PushMsgProcessCreateiInvoiceNotification.
    /// </summary>
    public class CreateiInvoiceNotification : INotification
    {
        /// <summary>
        /// Gets or sets RequestBody.
        /// </summary>
        public CreateInvoiceDto RequestBody { get; set; }
    }
}
