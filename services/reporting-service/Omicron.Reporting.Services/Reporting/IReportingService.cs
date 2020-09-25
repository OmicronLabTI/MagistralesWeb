// <summary>
// <copyright file="IReportingService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Reporting.Services
{
    using System.Threading.Tasks;
    using Omicron.Reporting.Entities.Model;

    /// <summary>
    /// Contract for request service.
    /// </summary>
    public interface IReportingService
    {
        /// <summary>
        /// Create raw material request.
        /// </summary>
        /// <param name="request">Requests data.</param>
        /// <param name="preview">Flag for preview file.</param>
        /// <returns>Report file stream.</returns>
        FileResultModel CreateRawMaterialRequestPdf(RawMaterialRequestModel request, bool preview);

        /// <summary>
        /// Submit raw material request.
        /// </summary>
        /// <param name="request">Requests data.</param>
        /// <returns>Operation result.</returns>
        Task<ResultModel> SubmitRawMaterialRequestPdf(RawMaterialRequestModel request);
    }
}
