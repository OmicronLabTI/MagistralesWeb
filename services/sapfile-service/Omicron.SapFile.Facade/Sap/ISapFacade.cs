// <summary>
// <copyright file="ResultDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapFile.Facade.Sap
{
    using Omicron.SapFile.Dtos.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ISapFacade
    {
        /// <summary>
        /// Generates the pdfs.
        /// </summary>
        /// <param name="listGeneratePdf"></param>
        /// <returns>the data.</returns>
        Task<ResultDto> CreatePdfs(List<FinalizaGeneratePdfDto> listGeneratePdf);
    }
}
