// <summary>
// <copyright file="ISapAdapterService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.Services.SapAdapter
{
    /// <summary>
    /// Interface sap adapter.
    /// </summary>
    public interface ISapAdapterService
    {
        /// <summary>
        /// Asynchronously sends a POST request to the SAP adapter service.
        /// </summary>
        /// <param name="data"> Data to send in the POST request. </param>
        /// <param name="route"> SAP adapter service endpoint URL. </param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation, containing a <see cref="ResultDto"/> with the response data from the SAP adapter.</returns>
        Task<ResultDto> Post(object data, string route);
    }
}
