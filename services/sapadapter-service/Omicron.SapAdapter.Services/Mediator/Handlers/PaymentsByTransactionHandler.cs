// <summary>
// <copyright file="PaymentsByTransactionHandler.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Services.Mediator.Handlers
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Omicron.SapAdapter.Dtos.DxpModels;
    using Omicron.SapAdapter.Services.Mediator.Commands;
    using Omicron.SapAdapter.Services.ProccessPayments;
    using Omicron.SapAdapter.Services.Utils;

    /// <summary>
    /// PaymentsByTransactionHandler class.
    /// </summary>
    public class PaymentsByTransactionHandler : IRequestHandler<PaymentsByTransactionCommand, List<PaymentsDto>>
    {
        private readonly IProccessPayments proccessPayments;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentsByTransactionHandler"/> class.
        /// </summary>
        /// <param name="proccessPayments">the proccess payments.</param>
        public PaymentsByTransactionHandler(IProccessPayments proccessPayments)
        {
            this.proccessPayments = proccessPayments.ThrowIfNull(nameof(proccessPayments));
        }

        /// <inheritdoc/>
        public async Task<List<PaymentsDto>> Handle(PaymentsByTransactionCommand request, CancellationToken cancellationToken)
            => await ServiceShared.GetPaymentsByTransactionsIds(this.proccessPayments, request.TransactionsIds);
    }
}
