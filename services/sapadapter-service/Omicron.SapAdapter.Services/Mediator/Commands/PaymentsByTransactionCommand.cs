// <summary>
// <copyright file="PaymentsByTransactionCommand.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Services.Mediator.Commands
{
    using System.Collections.Generic;
    using MediatR;
    using Omicron.SapAdapter.Dtos.DxpModels;
    using Omicron.SapAdapter.Services.Utils;

    /// <summary>
    /// PaymentsByTransactionCommand class.
    /// </summary>
    public class PaymentsByTransactionCommand : IRequest<List<PaymentsDto>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentsByTransactionCommand"/> class.
        /// </summary>
        /// <param name="transactionsIds">Transactions Ids.</param>
        public PaymentsByTransactionCommand(List<string> transactionsIds)
        {
            this.TransactionsIds = transactionsIds.ThrowIfNull(nameof(transactionsIds));
        }

        /// <summary>
        /// Gets or sets TransactionsIds.
        /// </summary>
        /// <value>
        /// <see cref="List{string}"/> TransactionsIds.
        /// </value>
        public List<string> TransactionsIds { get; set; }
    }
}
