// <summary>
// <copyright file="IDoctorService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Services.Doctors
{
    using System.Threading.Tasks;
    using Omicron.SapAdapter.Dtos.Models;

    /// <summary>
    /// The proccess Payments service.
    /// </summary>
    public interface IDoctorService
    {
        /// <summary>
        /// Gets the POST proccess payments.
        /// </summary>
        /// <param name="objectToSend">the object.</param>
        /// <param name="route">The route.</param>
        /// <returns>data.</returns>
        Task<ResultDto> PostDoctors(object objectToSend, string route);
    }
}
