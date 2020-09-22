// <summary>
// <copyright file="IReportingFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Reporting.Facade.Request
{
    using Omicron.Reporting.Dtos.Model;

    /// <summary>
    /// interfaces for the reporting facade.
    /// </summary>
    public interface IReportingFacade
    {
        /// <summary>
        /// Create raw material request.
        /// </summary>
        /// <param name="request">Requests data.</param>
        /// <param name="preview">Flag for preview file.</param>
        /// <returns>Report file stream.</returns>
        public FileResultDto CreateRawMaterialRequestPdf(RawMaterialRequestDto request, bool preview);
    }
}
