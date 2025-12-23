// <summary>
// <copyright file="IProcessPaymentService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Services.ProcessPayment
{
    /// <summary>
    /// interface for process paymants.
    /// </summary>
    public interface IProcessPaymentService
    {
        /// <summary>
        /// The process payment post.
        /// </summary>
        /// <param name="dataToSend">the data.</param>
        /// <param name="route">the route.</param>
        /// <returns>the result.</returns>
        Task<ResultDto> PostProcessPayments(object dataToSend, string route);

        /// <summary>
        /// Makes a get to favorites service.
        /// </summary>
        /// <param name="route">the route to send.</param>
        /// <returns>the data.</returns>
        Task<ResultDto> GetProcessPayments(string route);
    }
}
