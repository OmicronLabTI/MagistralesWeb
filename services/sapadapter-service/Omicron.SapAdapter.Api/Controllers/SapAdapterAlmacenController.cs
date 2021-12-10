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
    using Omicron.SapAdapter.Dtos.Models;
    using Omicron.SapAdapter.Facade.Sap;

    /// <summary>
    /// Class SapAdapterController Controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SapAdapterAlmacenController : ControllerBase
    {
        private readonly ISapAlmacenFacade sapAlmacenFacade;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapAdapterAlmacenController"/> class.
        /// </summary>
        /// <param name="sapAlmacenFacade">the sap almacen.</param>
        public SapAdapterAlmacenController(ISapAlmacenFacade sapAlmacenFacade)
        {
            this.sapAlmacenFacade = sapAlmacenFacade ?? throw new ArgumentNullException(nameof(sapAlmacenFacade));
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
        /// <param name="orderId">The parameters.</param>
        /// <returns>the data.</returns>
        [Route("/almacen/orders/{orderId}/detail")]
        [HttpGet]
        public async Task<IActionResult> GetOrdersDetails(int orderId)
        {
            var response = await this.sapAlmacenFacade.GetOrdersDetails(orderId);
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
        /// <param name="salesOrdersIds">The sales Orders Ids.</param>
        /// <returns>the data.</returns>
        [Route("/orders/model")]
        [HttpPost]
        public async Task<IActionResult> GetOrdersByIds(List<int> salesOrdersIds)
        {
            var response = await this.sapAlmacenFacade.GetOrdersByIds(salesOrdersIds);
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
        /// <param name="deliveryId">The parameters.</param>
        /// <returns>the data.</returns>
        [Route("/delivery/orders/{deliveryId}/details")]
        [HttpGet]
        public async Task<IActionResult> GetOrdersDeliveryDetail(int deliveryId)
        {
            var response = await this.sapAlmacenFacade.GetOrdersDeliveryDetail(deliveryId);
            return this.Ok(response);
        }

        /// <summary>
        /// Gets the orders.
        /// </summary>
        /// <param name="saleId">The parameters.</param>
        /// <returns>the data.</returns>
        [Route("/delivery/{saleId}/products")]
        [HttpGet]
        public async Task<IActionResult> GetProductsDelivery(string saleId)
        {
            var response = await this.sapAlmacenFacade.GetProductsDelivery(saleId);
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
        /// <param name="invoice">The invoice to look for.</param>
        /// <returns>the data.</returns>
        [Route("/invoice/orders/{invoice}/detail")]
        [HttpGet]
        public async Task<IActionResult> GetInvoiceDetail([FromRoute] int invoice)
        {
            var response = await this.sapAlmacenFacade.GetInvoiceDetail(invoice);
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
        /// <param name="days">the days.</param>
        /// <returns>the data.</returns>
        [Route("/cancelled/invoices/{days}")]
        [HttpGet]
        public async Task<IActionResult> GetCancelledInvoices(int days)
        {
            var response = await this.sapAlmacenFacade.GetCancelledInvoices(days);
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

        /// <summary>
        /// Gets the orders.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>the data.</returns>
        [Route("/almacen/orders/doctor")]
        [HttpGet]
        public async Task<IActionResult> SearchAlmacenOrdersByDoctor([FromQuery] Dictionary<string, string> parameters)
        {
            var response = await this.sapAlmacenFacade.SearchAlmacenOrdersByDoctor(parameters);
            return this.Ok(response);
        }

        /// <summary>
        /// Gets the delivery and invoice id by sale order..
        /// </summary>
        /// <param name="details">The sales id separated by commas.</param>
        /// <returns>the data.</returns>
        [Route("/almacen/orders/doctor/detail")]
        [HttpPost]
        public async Task<IActionResult> SearchAlmacenOrdersDetailsByDoctor(DoctorOrdersSearchDeatilDto details)
        {
            var response = await this.sapAlmacenFacade.SearchAlmacenOrdersDetailsByDoctor(details);
            return this.Ok(response);
        }

        /// <summary>
        /// Gets the orders.
        /// </summary>
        /// <param name="saleorderid">The type of scanned item.</param>
        /// <returns>the data.</returns>
        [Route("/almacen/Orderdetail/{saleorderid}")]
        [HttpGet]
        public async Task<IActionResult> GetOrderdetail(int saleorderid)
        {
            var response = await this.sapAlmacenFacade.GetOrderdetail(saleorderid);
            return this.Ok(response);
        }
    }
}
