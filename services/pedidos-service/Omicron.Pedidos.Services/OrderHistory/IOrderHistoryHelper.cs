// <summary>
// <copyright file="IOrderHistoryHelper.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.OrderHistory
{
    using System.Threading.Tasks;
    using Omicron.Pedidos.Services.MediatR.Commands;

    /// <summary>
    /// IOrderHistoryHelper.
    /// </summary>
    public interface IOrderHistoryHelper
    {
        /// <summary>
        /// SaveHistoryOrdersFab.
        /// </summary>
        /// <param name="detailOrderId">Child order number.</param>
        /// <param name="request">Parent request.</param>
        /// <returns>the return.</returns>
        Task SaveHistoryOrdersFab(int detailOrderId, CreateChildOrdersSapCommand request);
    }
}
