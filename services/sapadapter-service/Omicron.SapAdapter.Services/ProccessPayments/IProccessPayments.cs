// <summary>
// <copyright file="IProccessPayments.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Services.ProccessPayments
{
    using System.Threading.Tasks;
    using Omicron.SapAdapter.Dtos.Models;

    /// <summary>
    /// The proccess Payments service.
    /// </summary>
    public interface IProccessPayments
    {
        /// <summary>
        /// Gets the POST proccess payments.
        /// </summary>
        /// <param name="objectToSend">the object.</param>
        /// <param name="route">The route.</param>
        /// <returns>data.</returns>
        Task<ResultDto> PostProccessPayments(object objectToSend, string route);
    }
}
