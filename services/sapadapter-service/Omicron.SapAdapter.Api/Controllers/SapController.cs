// <summary>
// <copyright file="SapController.cs" company="Axity">
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
    /// Class User Controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SapController : ControllerBase
    {
        private readonly ISapFacade sapFacade;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapController"/> class.
        /// </summary>
        /// <param name="sapFacade">the sap facade.</param>
        /// <param name="logger">the logger factory.</param>
        public SapController(ISapFacade sapFacade, ILogger logger)
        {
            this.sapFacade = sapFacade ?? throw new ArgumentNullException(nameof(sapFacade));
            this.logger = logger;
        }

        /// <summary>
        /// Method to get all orders.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>List of orders.</returns>
        [Route("/orders")]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] Dictionary<string, string> parameters)
        {
            var response = await this.sapFacade.GetOrders(parameters);
            return this.Ok(response);
        }

        /// <summary>
        /// Gets the detail of the order.
        /// </summary>
        /// <param name="docEntry">the detail id.</param>
        /// <returns>the details.</returns>
        [Route("/detail/{docEntry}")]
        [HttpGet]
        public async Task<IActionResult> GetDetails(string docEntry)
        {
            var result = await this.sapFacade.GetDetallePedidos(docEntry);
            return this.Ok(result);
        }

        /// <summary>
        /// Method to get all orders.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>List of orders.</returns>
        [Route("/orders/details")]
        [HttpGet]
        public async Task<IActionResult> GetDetails([FromQuery] Dictionary<string, string> parameters)
        {
            var response = await this.sapFacade.GetDetails(parameters);
            return this.Ok(response);
        }

        /// <summary>
        /// Obtiene las formulas de la orden de fabricacion.
        /// </summary>
        /// <param name="ordenId">the order id.</param>
        /// <returns>the object.</returns>
        [Route("/formula/{ordenId}")]
        [HttpGet]
        public async Task<IActionResult> GetOrderFormula(int ordenId)
        {
            var result = await this.sapFacade.GetOrderFormula(new List<int> { ordenId }, true, true);
            return this.Ok(result);
        }

        /// <summary>
        /// Obtiene las formulas de la orden de fabricacion.
        /// </summary>
        /// <param name="ordenId">the order id.</param>
        /// <returns>the object.</returns>
        [Route("/qfb/formula")]
        [HttpPost]
        public async Task<IActionResult> GetOrderQfbFormula(List<int> ordenId)
        {
            var result = await this.sapFacade.GetOrderFormula(ordenId, false, false);
            return this.Ok(result);
        }

        /// <summary>
        /// Get fabrication orders by criterial.
        /// </summary>
        /// <param name="salesOrders">the sales order ids.</param>
        /// <param name="productionOrders">the production order ids.</param>
        /// <param name="components">Flag for get components.</param>
        /// <returns>the object.</returns>
        [Route("/fabOrder")]
        [HttpGet]
        public async Task<IActionResult> GetFabricationOrdersByCriterial(
            [FromQuery]string salesOrders,
            [FromQuery] string productionOrders,
            [FromQuery] bool components)
        {
            var salesOrdersIds = (salesOrders ?? string.Empty).ToIntList();
            var productionOrdersIds = (productionOrders ?? string.Empty).ToIntList();

            var result = await this.sapFacade.GetFabricationOrdersByCriterial(salesOrdersIds, productionOrdersIds, components);
            return this.Ok(result);
        }

        /// <summary>
        /// Obtiene las ordenes de fabricacion con su pedido.
        /// </summary>
        /// <param name="pedidosId">the ids.</param>
        /// <returns>the result.</returns>
        [Route("/getDetails")]
        [HttpPost]
        public async Task<IActionResult> GetOrderWithDetails(List<int> pedidosId)
        {
            var result = await this.sapFacade.GetPedidoWithDetail(pedidosId);
            return this.Ok(result);
        }

        /// <summary>
        /// Obtiene la orden de fabricacion en base al orderitem-productid para el insert de ordenes de fabricacion.
        /// </summary>
        /// <param name="pedidosId">the ids.</param>
        /// <returns>the result.</returns>
        [Route("/getProductionOrderItem")]
        [HttpPost]
        public async Task<IActionResult> GetProdOrderByOrderItem(List<string> pedidosId)
        {
            var result = await this.sapFacade.GetProdOrderByOrderItem(pedidosId);
            return this.Ok(result);
        }

        /// <summary>
        /// Obtiene las formulas de la orden de fabricacion.
        /// </summary>
        /// <param name="parameters">the order id.</param>
        /// <returns>the object.</returns>
        [Route("/componentes")]
        [HttpGet]
        public async Task<IActionResult> GetComponentes([FromQuery] Dictionary<string, string> parameters)
        {
            this.logger.Information($"Se buscara {JsonConvert.SerializeObject(parameters)}");
            var result = await this.sapFacade.GetComponents(parameters);
            return this.Ok(result);
        }

        /// <summary>
        /// Obtiene las formulas de la orden de fabricacion.
        /// </summary>
        /// <param name="ordenId">the order id.</param>
        /// <returns>the object.</returns>
        [Route("/componentes/lotes/{ordenId}")]
        [HttpGet]
        public async Task<IActionResult> GetBatchesComponents(int ordenId)
        {
            var result = await this.sapFacade.GetBatchesComponents(ordenId);
            return this.Ok(result);
        }

        /// <summary>
        /// Get last id of isolated production order created.
        /// </summary>
        /// <param name="productId">the product id.</param>
        /// <param name="uniqueId">the unique record id.</param>
        /// <returns>the data.</returns>
        [Route("/fabOrder/isolated/last")]
        [HttpGet]
        public async Task<IActionResult> GetlLastIsolatedProductionOrderId([FromQuery] string productId, [FromQuery] string uniqueId)
        {
            var result = await this.sapFacade.GetlLastIsolatedProductionOrderId(productId, uniqueId);
            return this.Ok(result);
        }

        /// <summary>
        /// Get new batch code.
        /// </summary>
        /// <param name="productCode">the product code.</param>
        /// <returns>the data.</returns>
        [Route("/batchcode/next")]
        [HttpGet]
        public async Task<IActionResult> GetNextBatchCode([FromQuery] string productCode)
        {
            var result = await this.sapFacade.GetNextBatchCode(productCode);
            return this.Ok(result);
        }

        /// <summary>
        /// Validate if batch code exists.
        /// </summary>
        /// <param name="productCode">the product code.</param>
        /// <param name="batchCode">batchCode.</param>
        /// <returns>the data.</returns>
        [Route("/batchcode/exists")]
        [HttpGet]
        public async Task<IActionResult> ValidateIfExistsBatchCodeByItemCode([FromQuery] string productCode, [FromQuery] string batchCode)
        {
            var result = await this.sapFacade.ValidateIfExistsBatchCodeByItemCode(productCode, batchCode);
            return this.Ok(result);
        }

        /// <summary>
        /// Get the orders by the filters.
        /// </summary>
        /// <param name="orderFabDto">The orderFabDto.</param>
        /// <returns>the data.</returns>
        [Route("/fabOrder/filters")]
        [HttpPost]
        public async Task<IActionResult> GetFabOrders(GetOrderFabDto orderFabDto)
        {
            var result = await this.sapFacade.GetFabOrders(orderFabDto);
            return this.Ok(result);
        }

        /// <summary>
        /// Get products management by batches with criterials.
        /// </summary>
        /// <param name="parameters">the filters.</param>
        /// <returns>the data.</returns>
        [HttpGet]
        [Route("/products")]
        public async Task<IActionResult> GetProductsManagmentByBatch([FromQuery] Dictionary<string, string> parameters)
        {
            var result = await this.sapFacade.GetProductsManagmentByBatch(parameters.DecodeQueryString());
            return this.Ok(result);
        }

        /// <summary>
        /// Get the orders by the orderid.
        /// </summary>
        /// <param name="lisOrders">The orderFabDto.</param>
        /// <returns>the data.</returns>
        [Route("/fabOrderId")]
        [HttpPost]
        public async Task<IActionResult> GetFabOrdersById(List<int> lisOrders)
        {
            var result = await this.sapFacade.GetFabOrdersById(lisOrders);
            return this.Ok(result);
        }

        /// <summary>
        /// Gets the list of recipes.
        /// </summary>
        /// <param name="orderId">the order id.</param>
        /// <returns>the data.</returns>
        [Route("/recipe/{orderId}")]
        [HttpGet]
        public async Task<IActionResult> GetRecipe(int orderId)
        {
            var result = await this.sapFacade.GetRecipe(orderId);
            return this.Ok(result);
        }

        /// <summary>
        /// Gets the recipes by all orders id.
        /// </summary>
        /// <param name="ordersId">the order ids.</param>
        /// <returns>the data.</returns>
        [Route("/recipes/orders")]
        [HttpPost]
        public async Task<IActionResult> GetRecipes(List<int> ordersId)
        {
            var result = await this.sapFacade.GetRecipes(ordersId);
            return this.Ok(result);
        }

        /// <summary>
        /// Gets the recipes by all orders id.
        /// </summary>
        /// <param name="orderId">the order ids.</param>
        /// <returns>the data.</returns>
        [Route("/validate/order/{orderId}")]
        [HttpGet]
        public async Task<IActionResult> ValidateOrder(int orderId)
        {
            var result = await this.sapFacade.ValidateOrder(orderId);
            return this.Ok(result);
        }

        /// <summary>
        /// Makes the ping.
        /// </summary>
        /// <returns>return the pong.</returns>
        [Route("/ping")]
        [HttpGet]
        public IActionResult Ping()
        {
            return this.Ok("Pong");
        }
    }
}
