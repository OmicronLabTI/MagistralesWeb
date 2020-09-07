// <summary>
// <copyright file="RequestService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Pedidos.Services.Request
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.Pedidos.DataAccess.DAO.Request;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Entities.Model.Db;
    using Omicron.Pedidos.Resources.Extensions;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.Utils;

    /// <summary>
    /// Implementations for request service.
    /// </summary>
    public class RequestService : IRequestService
    {
        private readonly IRequestDao requestDao;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestService"/> class.
        /// </summary>
        /// <param name="requestDao">request dao.</param>
        public RequestService(IRequestDao requestDao)
        {
            this.requestDao = requestDao;
        }

        /// <summary>
        /// Create raw material request.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="requests">Requests data.</param>
        /// <returns>List with successfuly and failed creations.</returns>
        public async Task<ResultModel> CreateRawMaterialRequest(string userId, List<RawMaterialRequestModel> requests)
        {
            var results = new SuccessFailResults<RawMaterialRequestModel>();
            foreach (var request in requests)
            {
                var creationResult = await this.CreateRequest(userId, request);
                if (creationResult.Item1)
                {
                    results.AddSuccesResult(creationResult.Item2);
                }
                else
                {
                    results.AddFailedResult(request, creationResult.Item3);
                }
            }

            return ServiceUtils.CreateResult(true, 200, null, results, null);
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
        public async Task<(bool, RawMaterialRequestModel, string)> CreateRequest(string userId, RawMaterialRequestModel request)
        {
            if ((await this.GetRawMaterialRequestByProductionOrder(request.ProductionOrderId)) == null)
            {
                request.Id = 0;
                request.CreationUserId = userId;
                request.CreationDate = DateTime.Now.FormatedLargeDate();

                await this.requestDao.InsertRawMaterialRequest(request);
                await this.CreateDetail(request.Id, request.OrderedProducts);
                return (true, request, null);
            }

            var message = string.Format(ServiceConstants.ReasonRawMaterialRequestAlreadyExists, request.ProductionOrderId);
            return (false, null, message);
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
        /// <param name="productionOrderId">The production order id.</param>
        /// <returns>Raw material request.</returns>
        public async Task<RawMaterialRequestModel> GetRawMaterialRequestByProductionOrder(int productionOrderId)
        {
            return await this.requestDao.GetRawMaterialRequestByProductionOrderId(productionOrderId);
        }

        /// <summary>
        /// Get a raw material request for production order id.
        /// </summary>
        /// <param name="productionOrderId">The production order id.</param>
        /// <returns>Raw material request.</returns>
        public async Task<ResultModel> GetRawMaterialRequestByProductionOrderId(int productionOrderId)
        {
            var request = await this.GetRawMaterialRequestByProductionOrder(productionOrderId);
            return ServiceUtils.CreateResult(true, 200, null, request, null);
        }
    }
}
