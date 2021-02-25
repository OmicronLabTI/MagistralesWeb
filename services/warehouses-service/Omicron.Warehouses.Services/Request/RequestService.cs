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
        private readonly IReportingService reportingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestService"/> class.
        /// </summary>
        /// <param name="requestDao">request dao.</param>
        /// <param name="usersService">users service.</param>
        /// <param name="sapAdapterService">sap adapter service.</param>
        /// <param name="reportingService">reporting service.</param>
        public RequestService(IRequestDao requestDao, IUsersService usersService, ISapAdapterService sapAdapterService, IReportingService reportingService)
        {
            this.requestDao = requestDao;
            this.usersService = usersService;
            this.sapAdapterService = sapAdapterService;
            this.reportingService = reportingService;
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

            (List<int> Missing, List<int> Existing) valitateExistsResults;
            List<int> exiting = new List<int> { };
            List<int> missing = new List<int> { };

            if (request.ProductionOrderIds.Any())
            {
                valitateExistsResults = await this.ValidateExistingByProductionOrderIds(request.ProductionOrderIds);
                exiting.AddRange(valitateExistsResults.Existing);
                missing.AddRange(valitateExistsResults.Missing);
                request.ProductionOrderIds = valitateExistsResults.Missing;
                if (!request.ProductionOrderIds.Any())
                {
                    valitateExistsResults.Existing.ForEach(x => results.AddFailedResult(new { ProductionOrderId = x }, string.Format(ErrorReasonConstants.ReasonRawMaterialRequestAlreadyExists, x)));
                    return ServiceUtils.CreateResult(true, 200, null, results, null);
                }
            }

            await this.InsertRequest(userId, request);

            var mailResult = await this.reportingService.SubmitRequest(request);
            if (!mailResult)
            {
                return ServiceUtils.CreateResult(true, 200, ErrorReasonConstants.ErrorToSubmitFile, null, null);
            }

            await this.InsertRequestDetail(request);

            if (request.ProductionOrderIds.Any())
            {
                exiting.ForEach(x => results.AddFailedResult(new { ProductionOrderId = x }, string.Format(ErrorReasonConstants.ReasonRawMaterialRequestAlreadyExists, x)));
                missing.ForEach(x => results.AddSuccesResult(new { ProductionOrderId = x }));
                return ServiceUtils.CreateResult(true, 200, null, results, null);
            }

            results.AddSuccesResult(new { ProductionOrderId = 0 });
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
        /// Return inserted request.
        /// </returns>
        public async Task<RawMaterialRequestModel> InsertRequest(string userId, RawMaterialRequestModel request)
        {
            request.Id = 0;
            request.CreationUserId = userId;
            request.CreationDate = DateTime.Now.ToString(DateConstants.LargeFormat);
            await this.requestDao.InsertRawMaterialRequest(request);
            return request;
        }

        /// <summary>
        /// Insert raw material request details.
        /// </summary>
        /// <param name="request">Request data.</param>
        /// <returns>
        /// Return updated request.
        /// </returns>
        public async Task<RawMaterialRequestModel> InsertRequestDetail(RawMaterialRequestModel request)
        {
            if (request.ProductionOrderIds.Any())
            {
                await this.requestDao.InsertOrdersOfRawMaterialRequest(request.ProductionOrderIds.Select(x => new RawMaterialRequestOrderModel { RequestId = request.Id, ProductionOrderId = x }).ToList());
            }

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

            var valitateExistingRequestsResults = await this.ValidateExistingByProductionOrderIds(existingProductionOrders.Select(x => x.ProductionOrderId).ToList());

            var allComponentsInPO = new List<ProductionOrderComponentModel>();
            existingProductionOrders.Where(x => valitateExistingRequestsResults.Missing.Any(y => y.Equals(x.ProductionOrderId))).ToList().ForEach(x => allComponentsInPO.AddRange(x.Details));

            var preRequest = new RawMaterialRequestModel
            {
                FailedProductionOrderIds = valitateExistingRequestsResults.Existing,
                ProductionOrderIds = valitateExistingRequestsResults.Missing,
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
            allComponentsInPO = this.RemoveItemPresentation(allComponentsInPO);

            foreach (var group in allComponentsInPO.GroupBy(x => new { x.ProductId, x.Warehouse, }))
            {
                var firstItem = group.FirstOrDefault();
                results.Add(new RawMaterialRequestDetailModel
                {
                    ProductId = group.Key.ProductId,
                    Description = firstItem.Description,
                    RequestQuantity = this.CalculateRequiredQuantity(group),
                    Unit = firstItem.Unit,
                });
            }

            results = results.Where(x => x.RequestQuantity < 0).ToList();
            results.ForEach(x => x.RequestQuantity = Math.Abs(x.RequestQuantity));

            return results;
        }

        /// <summary>
        /// Create request detail from production order components.
        /// </summary>
        /// <param name="allComponentsInPO">Production order components.</param>
        /// <returns>Request detail.</returns>
        private List<ProductionOrderComponentModel> RemoveItemPresentation(List<ProductionOrderComponentModel> allComponentsInPO)
        {
            allComponentsInPO.ForEach(x =>
            {
                x.ProductId = x.ProductId.Split("   ").First();
            });

            return allComponentsInPO;
        }

        /// <summary>
        /// Calculate required quantity.
        /// </summary>
        /// <param name="group">Item group.</param>
        /// <returns>Required cuantity.</returns>
        private decimal CalculateRequiredQuantity(IGrouping<object, ProductionOrderComponentModel> group)
        {
            var firstItem = group.FirstOrDefault();
            var sumRequiredQuantities = group.Sum(x => x.RequiredQuantity);

            return (firstItem.WarehouseQuantity < 0) ? -sumRequiredQuantities : firstItem.WarehouseQuantity - sumRequiredQuantities;
        }
    }
}
