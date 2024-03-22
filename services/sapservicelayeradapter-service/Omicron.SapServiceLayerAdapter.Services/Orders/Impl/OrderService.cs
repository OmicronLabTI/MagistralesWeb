// <summary>
// <copyright file="OrdersService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Services.Orders.Impl
{
    /// <summary>
    /// Class Orders Service.
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly IServiceLayerClient serviceLayerClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderService"/> class.
        /// </summary>
        /// <param name="serviceLayerClient">Service layer client.</param>
        public OrderService(IServiceLayerClient serviceLayerClient)
        {
            this.serviceLayerClient = serviceLayerClient.ThrowIfNull(nameof(serviceLayerClient));
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetLastGeneratedOrder()
        {
            var result = await this.serviceLayerClient.GetAsync(ServiceQuerysConstants.QryGetLastGeneratedOrder);

            if (!result.Success)
            {
                result.Response = JsonConvert.DeserializeObject<ServiceLayerErrorResponseDto>(result.Response.ToString());
                return result;
            }

            var response = JsonConvert.DeserializeObject<ServiceLayerResponseDto>(result.Response.ToString());
            var order = JsonConvert.DeserializeObject<List<OrderDto>>(response.Value.ToString());
            return ResponseUtils.CreateResult(
                result.Success,
                result.Code,
                result.UserError,
                order,
                result.ExceptionMessage,
                result.Comments?.ToString());
        }
    }
}
