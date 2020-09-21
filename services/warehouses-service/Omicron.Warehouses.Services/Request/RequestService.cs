// <summary>
// <copyright file="RequestService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Warehouses.Services.Request
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Omicron.Warehouses.DataAccess.DAO.Request;
    using Omicron.Warehouses.Entities.Model;
    using Omicron.Warehouses.Services.Clients;
    using Omicron.Warehouses.Services.Constants;
    using Omicron.Warehouses.Services.Utils;

    /// <summary>
    /// Implementations for request service.
    /// </summary>
    public class RequestService : IRequestService
    {
        private readonly IRequestDao requestDao;
        private readonly IUsersService usersService;
        private readonly ISapAdapterService sapAdapterService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestService"/> class.
        /// </summary>
        /// <param name="requestDao">request dao.</param>
        /// <param name="usersService">users service.</param>
        /// <param name="sapAdapterService">sap adapter service.</param>
        public RequestService(IRequestDao requestDao, IUsersService usersService, ISapAdapterService sapAdapterService)
        {
            this.requestDao = requestDao;
            this.usersService = usersService;
            this.sapAdapterService = sapAdapterService;
        }

        /// <summary>
        /// Create raw material request.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="request">Requests data.</param>
        /// <returns>List with successfuly and failed creations.</returns>
        public async Task<ResultModel> CreateRawMaterialRequest(string userId, RawMaterialRequestModel request)
        {
            var user = (await this.usersService.GetUsersById(userId)).FirstOrDefault(x => x.Id.Equals(userId));
            if (user == null)
            {
                return ServiceUtils.CreateResult(false, 200, ErrorReasonConstants.UserNotExists, null, null);
            }

            request.SigningUserId = userId;
            request.SigningUserName = $"{user.FirstName} {user.LastName}";

            var results = new SuccessFailResults<object>();
            var valitateExistsResults = await this.ValidateExistingByProductionOrderIds(request.ProductionOrderIds);
            request.ProductionOrderIds = valitateExistsResults.Missing;

            await this.CreateRequest(userId, request);

            valitateExistsResults.Existing.ForEach(x => results.AddFailedResult(new { ProductionOrderId = x }, string.Format(ErrorReasonConstants.ReasonRawMaterialRequestAlreadyExists, x)));
            valitateExistsResults.Missing.ForEach(x => results.AddSuccesResult(new { ProductionOrderId = x }));

            return ServiceUtils.CreateResult(true, 200, null, results, null);
        }

        /// <summary>
        /// Get a raw material request for production order id.
        /// </summary>
        /// <param name="productionOrderIds">The production order id.</param>
        /// <returns>Missing and existing lists.</returns>
        public async Task<(List<int> Missing, List<int> Existing)> ValidateExistingByProductionOrderIds(List<int> productionOrderIds)
        {
            var relatedRequest = await this.requestDao.GetRawMaterialRequestOrdersByProductionOrderIds(productionOrderIds.ToArray());
            return (
                productionOrderIds.Where(x => !relatedRequest.Any(r => r.ProductionOrderId.Equals(x))).ToList(),
                productionOrderIds.Where(x => relatedRequest.Any(r => r.ProductionOrderId.Equals(x))).ToList());
        }

        /// <summary>
        /// Update raw material request.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="request">Requests data.</param>
        /// <returns>List with successfuly and failed updates.</returns>
        public Task<ResultModel> UpdateRawMaterialRequest(string userId, RawMaterialRequestModel request)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create raw material request.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="request">Request data.</param>
        /// <returns>
        /// Return tuple with creation result.
        /// Item1 = Operation status.
        /// Item2 = Created request.
        /// Item3 = Error message.
        /// </returns>
        public async Task<RawMaterialRequestModel> CreateRequest(string userId, RawMaterialRequestModel request)
        {
            request.Id = 0;
            request.CreationUserId = userId;
            request.CreationDate = DateTime.Now.ToString(DateConstants.LargeFormat);
            await this.requestDao.InsertRawMaterialRequest(request);
            await this.requestDao.InsertOrdersOfRawMaterialRequest(request.ProductionOrderIds.Select(x => new RawMaterialRequestOrderModel { RequestId = request.Id, ProductionOrderId = x }).ToList());
            await this.CreateDetail(request.Id, request.OrderedProducts);
            return request;
        }

        /// <summary>
        /// Create detail of raw material request.
        /// </summary>
        /// <param name="requestId">The rquest id.</param>
        /// <param name="detail">Request detail.</param>
        /// <returns>New request detail.</returns>
        public async Task<List<RawMaterialRequestDetailModel>> CreateDetail(int requestId, List<RawMaterialRequestDetailModel> detail)
        {
            detail.ForEach(x => x.RequestId = requestId);
            await this.requestDao.InsertDetailsOfRawMaterialRequest(detail);
            return detail;
        }

        /// <summary>
        /// Get a raw material request for production order id.
        /// </summary>
        /// <param name="productionOrderIds">The production order ids.</param>
        /// <returns>Raw material request.</returns>
        public async Task<List<RawMaterialRequestModel>> GetRawMaterialRequestByProductionOrder(params int[] productionOrderIds)
        {
            return await this.requestDao.GetRawMaterialRequestByProductionOrderIds(productionOrderIds);
        }

        /// <summary>
        /// Get a raw material request for production order id.
        /// </summary>
        /// <param name="productionOrderId">The production order id.</param>
        /// <returns>Raw material request.</returns>
        public async Task<ResultModel> GetRawMaterialRequestByProductionOrderId(int productionOrderId)
        {
            var request = await this.GetRawMaterialRequestByProductionOrder(productionOrderId);
            return ServiceUtils.CreateResult(true, 200, null, request.FirstOrDefault(), null);
        }

        /// <summary>
        /// Get a raw material pre-request.
        /// </summary>
        /// <param name="salesOrders">the sales order ids.</param>
        /// <param name="productionOrders">the production order ids.</param>
        /// <returns>The material pre-request.</returns>
        public async Task<ResultModel> GetRawMaterialPreRequest(List<int> salesOrders, List<int> productionOrders)
        {
            var existingProductionOrders = await this.sapAdapterService.GetProductionOrdersByCriterial(salesOrders, productionOrders);
            existingProductionOrders = existingProductionOrders.Where(x => x.Status.ToLower().Equals(ServiceConstants.ProductionOrderPlannedStatus.ToLower())).ToList();

            var allComponentsInPO = new List<ProductionOrderComponentModel>();

            existingProductionOrders.ForEach(x => allComponentsInPO.AddRange(x.Details));

            var preRequest = new RawMaterialRequestModel
            {
                ProductionOrderIds = existingProductionOrders.Select(x => x.ProductionOrderId).ToList(),
                OrderedProducts = this.CreateRequestDetail(allComponentsInPO),
            };

            return ServiceUtils.CreateResult(true, 200, null, preRequest, null);
        }

        /// <summary>
        /// Create request detail from production order components.
        /// </summary>
        /// <param name="allComponentsInPO">Production order components.</param>
        /// <returns>Request detail.</returns>
        private List<RawMaterialRequestDetailModel> CreateRequestDetail(List<ProductionOrderComponentModel> allComponentsInPO)
        {
            var results = new List<RawMaterialRequestDetailModel>();

            foreach (var itemGroup in allComponentsInPO.GroupBy(x => x.ProductId))
            {
                var firstItem = itemGroup.FirstOrDefault();

                // TODO: Calcular la cantidad requerida.
                decimal requiredQuantity = 0M;

                results.Add(new RawMaterialRequestDetailModel
                {
                    ProductId = itemGroup.Key,
                    Description = firstItem.Description,
                    RequestQuantity = requiredQuantity,
                    Unit = firstItem.Unit,
                });
            }

            return results;
        }
    }
}
