// <summary>
// <copyright file="SapFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapFile.Facade.Sap
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using Omicron.SapFile.Dtos.Models;
    using Omicron.SapFile.Entities.Models;
    using Omicron.SapFile.Services.Prescription;
    using Omicron.SapFile.Services.SapFile;

    public class SapFacade : ISapFacade
    {
        private readonly IMapper mapper;

        private readonly ISapFileService sapFileService;

        private readonly IPrescriptionService prescriptionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapFacade"/> class.
        /// </summary>        
        /// <param name="mapper"></param>
        /// <param name="prescriptionService">Prescription Service.</param>
        public SapFacade(IMapper mapper, ISapFileService sapFileService, IPrescriptionService prescriptionService)
        {
            this.mapper = mapper;
            this.sapFileService = sapFileService;
            this.prescriptionService = prescriptionService;
        }

        /// <summary>
        /// Generates the pdfs.
        /// </summary>
        /// <param name="listGeneratePdf"></param>
        /// <returns>the data.</returns>
        public async Task<ResultDto> CreatePdfs(List<FinalizaGeneratePdfDto> listGeneratePdf)
        {
            return this.mapper.Map<ResultDto>(await this.sapFileService.CreatePdfs(this.mapper.Map<List<FinalizaGeneratePdfModel>>(listGeneratePdf)));
        }

        /// <summary>
        /// Created the pdf.
        /// </summary>
        /// <param name="orders">the orders.</param>
        /// <returns>the data.</returns>
        public async Task<ResultDto> CreateSaleOrderPdf(List<CreateOrderPdfDto> orders)
        {
            return this.mapper.Map<ResultDto>(await this.sapFileService.CreateSaleOrderPdf(orders));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> CreatePdfByType(string type, List<int> invoiceIds)
        {
            return this.mapper.Map<ResultDto>(await this.sapFileService.CreatePdfByType(type, invoiceIds));
        }

        /// <summary>
        /// Deletes the files.
        /// </summary>
        /// <returns>the data.</returns>
        public async Task<ResultDto> DeleteFiles()
        {
            return this.mapper.Map<ResultDto>(await this.sapFileService.DeleteFiles());
        }

        /// <summary>
        /// Save recipe to server.
        /// </summary>
        /// <param name="prescriptionUrls">Prescription urls to save.</param>
        /// <returns>Result Dto.</returns>
        public async Task<ResultDto> SavePresciptionToServer(List<PrescriptionServerRequestDto> prescriptionUrls)
        {
            return this.mapper.Map<ResultDto>(await this.prescriptionService.SavePresciptionToServer(prescriptionUrls));
        }
    }
}
