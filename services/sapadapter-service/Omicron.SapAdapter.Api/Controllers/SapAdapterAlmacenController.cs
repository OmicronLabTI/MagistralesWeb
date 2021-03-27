// <summary>
// <copyright file="SapAdapterAlmacenController.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using Omicron.SapAdapter.Dtos.Models;
    using Omicron.SapAdapter.Facade.Sap;
    using Omicron.SapAdapter.Resources.Extensions;
    using Serilog;

    /// <summary>
    /// Class SapAdapterController Controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SapAdapterAlmacenController : ControllerBase
    {
        private readonly ISapFacade sapFacade;

        private readonly ISapAlmacenFacade sapAlmacenFacade;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapAdapterAlmacenController"/> class.
        /// </summary>
        /// <param name="sapFacade">the sap facade.</param>
        /// <param name="logger">the logger factory.</param>
        /// <param name="sapAlmacenFacade">the sap almacen.</param>
        public SapAdapterAlmacenController(ISapFacade sapFacade, ILogger logger, ISapAlmacenFacade sapAlmacenFacade)
        {
            this.sapFacade = sapFacade ?? throw new ArgumentNullException(nameof(sapFacade));
            this.sapAlmacenFacade = sapAlmacenFacade ?? throw new ArgumentNullException(nameof(sapAlmacenFacade));
            this.logger = logger;
        }

        /// <summary>
        /// Gets the orders.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>the data.</returns>
        [Route("/almacen/orders")]
        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] Dictionary<string, string> parameters)
        {
            var response = await this.sapAlmacenFacade.GetOrders(parameters);
            return this.Ok(response);
        }

        /// <summary>
        /// Gets the orders.
        /// </summary>
        /// <param name="type">The type of scanned item.</param>
        /// <param name="code">The code scanned.</param>
        /// <returns>the data.</returns>
        [Route("/scanner/{type}/{code}")]
        [HttpGet]
        public async Task<IActionResult> GetScannedOrder(string type, string code)
        {
            var response = await this.sapAlmacenFacade.GetScannedData(type, code);
            return this.Ok(response);
        }

        /// <summary>
        /// Gets the orders.
        /// </summary>
        /// <param name="saleorderid">The type of scanned item.</param>
        /// <returns>the data.</returns>
        [Route("/complete/detail/{saleorderid}")]
        [HttpGet]
        public async Task<IActionResult> GetCompleteDetail(int saleorderid)
        {
            var response = await this.sapAlmacenFacade.GetCompleteDetail(saleorderid);
            return this.Ok(response);
        }

        /// <summary>
        /// Gets the orders.
        /// </summary>
        /// <param name="ordersId">The type of scanned item.</param>
        /// <returns>the data.</returns>
        [Route("/delivery/orderids")]
        [HttpPost]
        public async Task<IActionResult> GetDeliveryBySaleOrderId(List<int> ordersId)
        {
            var response = await this.sapAlmacenFacade.GetDeliveryBySaleOrderId(ordersId);
            return this.Ok(response);
        }

        /// <summary>
        /// Gets the orders.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>the data.</returns>
        [Route("/delivery/orders")]
        [HttpGet]
        public async Task<IActionResult> GetOrdersDelivery([FromQuery] Dictionary<string, string> parameters)
        {
            var response = await this.sapAlmacenFacade.GetDelivery(parameters);
            return this.Ok(response);
        }

        /// <summary>
        /// Gets the orders.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>the data.</returns>
        [Route("/invoice/orders")]
        [HttpGet]
        public async Task<IActionResult> GetInvoice([FromQuery] Dictionary<string, string> parameters)
        {
            var response = await this.sapAlmacenFacade.GetInvoice(parameters);
            return this.Ok(response);
        }

        /// <summary>
        /// Gets the orders.
        /// </summary>
        /// <param name="invoiceid">The parameters.</param>
        /// <returns>the data.</returns>
        [Route("/invoice/{invoiceid}/products")]
        [HttpGet]
        public async Task<IActionResult> GetInvoiceProducts(int invoiceid)
        {
            var response = await this.sapAlmacenFacade.GetInvoiceProducts(invoiceid);
            return this.Ok(response);
        }

        /// <summary>
        /// Gets the orders.
        /// </summary>
        /// <param name="dataToLook">The parameters.</param>
        /// <returns>the data.</returns>
        [Route("/invoice/package/header")]
        [HttpPost]
        public async Task<IActionResult> GetInvoiceHeader(InvoicePackageSapLookDto dataToLook)
        {
            var response = await this.sapAlmacenFacade.GetInvoiceHeader(dataToLook);
            return this.Ok(response);
        }

        /// <summary>
        /// Gets the delivery and invoice id by sale order..
        /// </summary>
        /// <param name="saleids">The sales id separated by commas.</param>
        /// <returns>the data.</returns>
        [Route("/sapids/saleorders")]
        [HttpPost]
        public async Task<IActionResult> GetSapIds(List<int> saleids)
        {
            var response = await this.sapAlmacenFacade.GetSapIds(saleids);
            return this.Ok(response);
        }

        /// <summary>
        /// Gets the orders.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>the data.</returns>
        [Route("/almacen/graph")]
        [HttpGet]
        public async Task<IActionResult> AlmacenGraphCount([FromQuery] Dictionary<string, string> parameters)
        {
            var response = await this.sapAlmacenFacade.AlmacenGraphCount(parameters);
            return this.Ok(response);
        }

        /// <summary>
        /// Gets the orders.
        /// </summary>
        /// <returns>the data.</returns>
        [Route("/delivery/parties")]
        [HttpGet]
        public async Task<IActionResult> GetDeliveryParties()
        {
            var response = await this.sapAlmacenFacade.GetDeliveryParties();
            return this.Ok(response);
        }

        /// <summary>
        /// Gets the delivery and invoice id by sale order..
        /// </summary>
        /// <param name="saleids">The sales id separated by commas.</param>
        /// <returns>the data.</returns>
        [Route("/getdeliveries")]
        [HttpPost]
        public async Task<IActionResult> GetDeliveries(List<int> saleids)
        {
            var response = await this.sapAlmacenFacade.GetDeliveries(saleids);
            return this.Ok(response);
        }

        /// <summary>
        /// Gets the orders.
        /// </summary>
        /// <returns>the data.</returns>
        [Route("/cancelled/invoices")]
        [HttpGet]
        public async Task<IActionResult> GetCancelledInvoices()
        {
            var response = await this.sapAlmacenFacade.GetCancelledInvoices();
            return this.Ok(response);
        }

        /// <summary>
        /// Gets the orders.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>the data.</returns>
        [Route("/advance/lookup")]
        [HttpGet]
        public async Task<IActionResult> AdvanceLookUp([FromQuery] Dictionary<string, string> parameters)
        {
            var response = await this.sapAlmacenFacade.AdvanceLookUp(parameters);
            return this.Ok(response);
        }
    }
}
