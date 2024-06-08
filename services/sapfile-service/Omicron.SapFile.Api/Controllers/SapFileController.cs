// ------------------------------------------------------------------------------------------------
// <copyright file="SapFileController.cs" company="Ann Inc">
//  Copyright (c) 2017 Ann Inc, All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace Omicron.SapFile.Api.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Omicron.SapFile.Dtos.Models;
    using Omicron.SapFile.Facade.Sap;
    
    /// <summary>
    /// Layout Element controller for any layout element which belongs to a specific layout
    /// </summary>
    [RoutePrefix("SapFile")]
    public class SapFileController : BaseController
    {
        /// <summary>
        /// Gets or sets the sa.
        /// </summary>
        private readonly ISapFacade sapFacade;

        /// <summary>
        /// /// Initializes a new instance of the <see cref="SapFileController"/> class.
        /// </summary>
        /// <param name="sapFacade">the facade.</param>
        public SapFileController(ISapFacade sapFacade)
        {
            this.sapFacade = sapFacade;
        }

        /// <summary>
        /// Method to send to generate the pdfs.
        /// </summary>
        /// <param name="listToGenerate">the list to generate.</param>
        /// <returns>the data.</returns>
        [HttpPost]
        [Route("create/pdf")]
        public async Task<IHttpActionResult> CreatePdfs(List<FinalizaGeneratePdfDto> listToGenerate)
        {
            var response = await this.sapFacade.CreatePdfs(listToGenerate);
            return this.Ok(response);
        }

        /// <summary>
        /// Method to create the sale order pdf.
        /// </summary>
        /// <param name="ordersId">the order ids.</param>
        /// <returns>the data.</returns>
        [HttpPost]
        [Route("create/sale/pdf")]
        public async Task<IHttpActionResult> CreateSaleOrderPdf(List<int> ordersId)
        {
            var response = await this.sapFacade.CreateSaleOrderPdf(ordersId);
            return this.Ok(response);
        }

        /// <summary>
        /// Method to create the sale order pdf.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="ordersId">the order ids.</param>
        /// <returns>the data.</returns>
        [HttpPost]
        [Route("create/{type}/pdfs")]
        public async Task<IHttpActionResult> CreateInvoicePdf(string type, List<int> ordersId)
        {
            var response = await this.sapFacade.CreatePdfByType(type, ordersId);
            return this.Ok(response);
        }

        /// <summary>
        /// Deletes the files.
        /// </summary>
        /// <returns>the data.</returns>
        [HttpPost]
        [Route("delete/files")]
        public async Task<IHttpActionResult> DeleteFiles()
        {
            var response = await this.sapFacade.DeleteFiles();
            return this.Ok(response);
        }

        /// <summary>
        /// the ping pong.
        /// </summary>
        /// <returns>rturn pong.</returns>
        [HttpGet]
        [Route("ping")]
        public IHttpActionResult Get()
        {
            return this.Ok("pong");
        }
    }
}