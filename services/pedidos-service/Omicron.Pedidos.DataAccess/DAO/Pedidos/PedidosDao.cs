// <summary>
// <copyright file="PedidosDao.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.DataAccess.DAO.Pedidos
{
    using Microsoft.EntityFrameworkCore;
    using Omicron.Pedidos.Entities.Context;
    using Omicron.Pedidos.Entities.Model;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Security.Cryptography.X509Certificates;
    using Omicron.Pedidos.Entities.Model.Db;

    /// <summary>
    /// dao for pedidos
    /// </summary>
    public class PedidosDao : IPedidosDao
    {
        private readonly IDatabaseContext databaseContext;

        private readonly string CloseDate = "CloseDate";

        /// <summary>
        /// Initializes a new instance of the <see cref="PedidosDao"/> class.
        /// </summary>
        /// <param name="databaseContext">DataBase Context</param>
        public PedidosDao(IDatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
        }

        /// <inheritdoc/>
        public async Task<bool> InsertUserOrder(List<UserOrderModel> userorder)
        {
            this.databaseContext.UserOrderModel.AddRange(userorder);
            await ((DatabaseContext)this.databaseContext).SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Method for add registry to DB.
        /// </summary>
        /// <param name="orderLog">UserOrder Dto.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<bool> InsertOrderLog(List<OrderLogModel> orderLog)
        {
            this.databaseContext.OrderLogModel.AddRange(orderLog);
            await ((DatabaseContext)this.databaseContext).SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// the list ids.
        /// </summary>
        /// <param name="listIDs">the list ids.</param>
        /// <returns>the data.</returns>
        public async Task<IEnumerable<UserOrderModel>> GetUserOrderBySaleOrder(List<string> listIDs)
        {
            return await this.databaseContext.UserOrderModel.Where(x => listIDs.Contains(x.Salesorderid)).ToListAsync();
        }

        /// <summary>
        /// Returns the user orders by SalesOrder (Pedido)
        /// </summary>
        /// <param name="listIDs">the list ids.</param>
        /// <returns>the data.</returns>
        public async Task<IEnumerable<UserOrderModel>> GetUserOrderByProducionOrder(List<string> listIDs)
        {
            return await this.databaseContext.UserOrderModel.Where(x => listIDs.Contains(x.Productionorderid)).ToListAsync();
        }

        /// <summary>
        /// Returns the user order by user id.
        /// </summary>
        /// <param name="listIds">the list of users.</param>
        /// <returns>the data.</returns>
        public async Task<IEnumerable<UserOrderModel>> GetUserOrderByUserId(List<string> listIds)
        {
            return await this.databaseContext.UserOrderModel.Where(x => listIds.Contains(x.Userid)).ToListAsync();
        }

        /// <summary>
        /// Returns the user order by user id.
        /// </summary>
        /// <param name="listStatus">the list of users.</param>
        /// <returns>the data.</returns>
        public async Task<IEnumerable<UserOrderModel>> GetUserOrderByStatus(List<string> listStatus)
        {
            return await this.databaseContext.UserOrderModel.Where(x => listStatus.Contains(x.Status)).ToListAsync();
        }

        /// <summary>
        /// Returns the user order by user id.
        /// </summary>
        /// <param name="fechaInicio">The init date.</param>
        /// <param name="fechaFin">the end date.</param>
        /// <returns>the data.</returns>
        public async Task<IEnumerable<UserOrderModel>> GetUserOrderByFechaFin(DateTime fechaInicio, DateTime fechaFin)
        {
            var orderByFinishDate = await this.databaseContext.UserOrderModel.Where(x => !string.IsNullOrEmpty(x.FinishDate)).ToListAsync();
            return this.GetDataByDateConvert(orderByFinishDate, fechaInicio, fechaFin, "finishDate");
        }

        /// <summary>
        /// Returns the user order by user id.
        /// </summary>
        /// <param name="fechaInicio">The init date.</param>
        /// <param name="fechaFin">the end date.</param>
        /// <returns>the data.</returns>
        public async Task<IEnumerable<UserOrderModel>> GetUserOrderByFechaClose(DateTime fechaInicio, DateTime fechaFin)
        {
            var orderByFinishDate = await this.databaseContext.UserOrderModel.Where(x => !string.IsNullOrEmpty(x.CloseDate)).ToListAsync();
            return this.GetDataByDateConvert(orderByFinishDate, fechaInicio, fechaFin, this.CloseDate);
        }

        /// <summary>
        /// Updates the entries.
        /// </summary>
        /// <param name="userOrderModels">the user model.</param>
        /// <returns>the data.</returns>
        public async Task<bool> UpdateUserOrders(List<UserOrderModel> userOrderModels)
        {
            this.databaseContext.UserOrderModel.UpdateRange(userOrderModels);
            await ((DatabaseContext)this.databaseContext).SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Method for add order signatures.
        /// </summary>
        /// <param name="orderSignature">Order signatures to add.</param>
        /// <returns>Operation result</returns>
        public async Task<bool> InsertOrderSignatures(UserOrderSignatureModel orderSignature)
        {
            this.databaseContext.UserOrderSignatureModel.AddRange(orderSignature);
            await ((DatabaseContext)this.databaseContext).SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Method for add order signatures.
        /// </summary>
        /// <param name="orderSignature">Order signatures to add.</param>
        /// <returns>Operation result</returns>
        public async Task<bool> InsertOrderSignatures(List<UserOrderSignatureModel> orderSignature)
        {
            this.databaseContext.UserOrderSignatureModel.AddRange(orderSignature);
            await ((DatabaseContext)this.databaseContext).SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Method for save order signatures.
        /// </summary>
        /// <param name="orderSignature">Order signatures to save.</param>
        /// <returns>Operation result</returns>
        public async Task<bool> SaveOrderSignatures(UserOrderSignatureModel orderSignature)
        {
            this.databaseContext.UserOrderSignatureModel.UpdateRange(orderSignature);
            await ((DatabaseContext)this.databaseContext).SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Method for save order signatures.
        /// </summary>
        /// <param name="orderSignature">Order signatures to save.</param>
        /// <returns>Operation result</returns>
        public async Task<bool> SaveOrderSignatures(List<UserOrderSignatureModel> orderSignature)
        {
            this.databaseContext.UserOrderSignatureModel.UpdateRange(orderSignature);
            await((DatabaseContext)this.databaseContext).SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Get order signature by user order id.
        /// </summary>
        /// <param name="userOrderId">User order to find.</param>
        /// <returns>Operation result</returns>
        public async Task<UserOrderSignatureModel> GetSignaturesByUserOrderId(int userOrderId)
        {
            return await this.databaseContext.UserOrderSignatureModel.FirstOrDefaultAsync(x => x.UserOrderId.Equals(userOrderId));
        }

        /// <summary>
        /// Get order signature by user order id.
        /// </summary>
        /// <param name="userOrderId">User order to find.</param>
        /// <returns>Operation result</returns>
        public async Task<IEnumerable<UserOrderSignatureModel>> GetSignaturesByUserOrderId(List<int> userOrderId)
        {
            return await this.databaseContext.UserOrderSignatureModel.Where(x => userOrderId.Contains(x.UserOrderId)).ToListAsync();
        }

        /// <summary>
        /// Insert new custom component list.
        /// </summary>
        /// <param name="customComponentList">Custom list to insert.</param>
        /// <returns>Operation result</returns>
        public async Task<bool> InsertCustomComponentList(CustomComponentListModel customComponentList)
        {
            this.databaseContext.CustomComponentLists.Add(customComponentList);
            await ((DatabaseContext)this.databaseContext).SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Insert new components of custom list.
        /// </summary>
        /// <param name="components">Components of custom list to insert.</param>
        /// <returns>Operation result.</returns>
        public async Task<bool> InsertComponentsOfCustomList(List<ComponentCustomComponentListModel> components)
        {
            this.databaseContext.ComponentsCustomComponentLists.AddRange(components);
            await ((DatabaseContext)this.databaseContext).SaveChangesAsync();
            return true;
        }


        /// <summary>
        /// Get all custom component lists for product id.
        /// </summary>
        /// <param name="productId">Te product id.</param>
        /// <returns>Related lists.</returns>
        public async Task<List<CustomComponentListModel>> GetCustomComponentListByProduct(string productId)
        {
            return await this.databaseContext.CustomComponentLists.Where(x => x.ProductId.Equals(productId)).ToListAsync();
        }


        /// <summary>
        /// Get all component for custom list id.
        /// </summary>
        /// <param name="customListIds">Te custom list ids.</param>
        /// <returns>Related components.</returns>
        public async Task<List<ComponentCustomComponentListModel>> GetComponentsByCustomListId(List<int> customListIds)
        {
            return await this.databaseContext.ComponentsCustomComponentLists.Where(x => customListIds.Contains(x.CustomListId)).ToListAsync();
        }

        /// <summary>
        /// Gets the data by field.
        /// </summary>
        /// <param name="fieldName">The field name.</param>
        /// <returns>the data.</returns>
        public async Task<List<ParametersModel>> GetParamsByFieldContains(string fieldName)
        {
            return await this.databaseContext.ParametersModel.Where(x => x.Field.Contains(fieldName)).ToListAsync();
        }

        /// <summary>
        /// Gets the qr if exist in table.
        /// </summary>
        /// <param name="userOrderId">the orders ids.</param>
        /// <returns>the data.</returns>
        public async Task<List<ProductionOrderQr>> GetQrRoute(List<int> userOrderId)
        {
            return await this.databaseContext.ProductionOrderQr.Where(x => userOrderId.Contains(x.UserOrderId)).ToListAsync();
        }

        /// <summary>
        /// Gets the qr if exist in table.
        /// </summary>
        /// <param name="modelsToSave">the orders ids.</param>
        /// <returns>the data.</returns>
        public async Task<bool> InsertQrRoute(List<ProductionOrderQr> modelsToSave)
        {
            this.databaseContext.ProductionOrderQr.AddRange(modelsToSave);
            await((DatabaseContext)this.databaseContext).SaveChangesAsync();
            return true;
        }

        /// <inheritdoc/>
        public async Task<List<UserOrderModel>> GetSaleOrderForAlmacen(string status, DateTime dateToLook, List<string> statusPending)
        {
            var orders = await this.databaseContext.UserOrderModel.Where(x => x.FinalizedDate != null && x.FinalizedDate >= dateToLook).ToListAsync();

            var idsSaleFinalized = orders.Where(x => x.IsSalesOrder && x.Status.Equals(status) && x.FinishedLabel == 1).Select(y => y.Salesorderid).ToList();
            var orderstoReturn = orders.Where(x => idsSaleFinalized.Contains(x.Salesorderid)).ToList();

            var possiblePending = orders.Where(x => x.IsProductionOrder && x.Status.Equals(status) && x.FinishedLabel == 1).Select(y => y.Salesorderid).Distinct().ToList();
            var isPending = possiblePending.Where(x => !idsSaleFinalized.Any(y => y == x)).ToList();
            var pendingOrders = await this.databaseContext.UserOrderModel.Where(x => isPending.Contains(x.Salesorderid)).ToListAsync();

            pendingOrders.GroupBy(x => x.Salesorderid).ToList().ForEach(y =>
            {
                var orders = y.Where(z => z.IsProductionOrder).ToList();
                if (y.Any(z => z.IsProductionOrder && z.Status == status && z.FinishedLabel == 1) && orders.All(z => statusPending.Contains(z.Status)))
                {
                    orderstoReturn.AddRange(pendingOrders);
                }
            });
            return orderstoReturn;
        }

        /// <inheritdoc/>
        public async Task<List<UserOrderModel>> GetOrderForAlmacenToIgnore(string status, DateTime dateToLook)
        {
            var orders = await this.databaseContext.UserOrderModel.Where(x => x.FinalizedDate == null || x.FinalizedDate >= dateToLook).ToListAsync();
            return orders.Where(x => x.IsSalesOrder && (x.Status != status || x.FinishedLabel != 1)).ToList();
        }

        /// <summary>
        /// GEts the orders by id.
        /// </summary>
        /// <param name="ordersId">th eorderd id.</param>
        /// <returns>the orders.</returns>
        public async Task<List<UserOrderModel>> GetUserOrdersById(List<int> ordersId)
        {
            return await this.databaseContext.UserOrderModel.Where(x => ordersId.Contains(x.Id)).ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<List<ProductionRemisionQrModel>> GetQrRemisionRouteBySaleOrder(List<int> saleOrder)
        {
            return await this.databaseContext.ProductionRemisionQrModel.Where(x => saleOrder.Contains(x.PedidoId)).ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<List<ProductionFacturaQrModel>> GetQrFacturaRouteByInvoice(List<int> invoiceId)
        {
            return await this.databaseContext.ProductionFacturaQrModel.Where(x => invoiceId.Contains(x.FacturaId)).ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> InsertQrRouteFactura(List<ProductionFacturaQrModel> modelsToSave)
        {
            this.databaseContext.ProductionFacturaQrModel.AddRange(modelsToSave);
            await ((DatabaseContext)this.databaseContext).SaveChangesAsync();
            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> InsertQrRouteRemision(List<ProductionRemisionQrModel> modelsToSave)
        {
            this.databaseContext.ProductionRemisionQrModel.AddRange(modelsToSave);
            await ((DatabaseContext)this.databaseContext).SaveChangesAsync();
            return true;
        }

        /// <inheritdoc/>
        public async Task<List<UserOrderModel>> GetUserOrdersForInvoice(string statusForSale, string statusForOrder)
        {
            var userOrders = await this.databaseContext.UserOrderModel.Where(x => x.Status == statusForSale).ToListAsync();
            var prodOrders = await this.databaseContext.UserOrderModel.Where(x => !string.IsNullOrEmpty(x.Productionorderid) && !string.IsNullOrEmpty(x.StatusAlmacen) && x.StatusAlmacen == statusForOrder).ToListAsync();
            userOrders.AddRange(prodOrders);
            return userOrders;
        }

        /// <inheritdoc/>
        public async Task<List<UserOrderModel>> GetUserOrdersByInvoiceId(List<int> invoiceId)
        {
            return await this.databaseContext.UserOrderModel.Where(x => invoiceId.Contains(x.InvoiceId)).ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<UserOrderModel>> GetUserOrderByStatusInvoice(List<string> listStatus)
        {
            return await this.databaseContext.UserOrderModel.Where(x => listStatus.Contains(x.StatusInvoice)).ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<UserOrderModel>> GetUserOrderByInvoiceType(List<string> types)
        {
            return await this.databaseContext.UserOrderModel.Where(x => types.Contains(x.InvoiceType)).ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<UserOrderModel>> GetUserOrderByFinalizeDate(DateTime init, DateTime endDate)
        {
            return await this.databaseContext.UserOrderModel.Where(x => x.FinalizedDate >= init && x.FinalizedDate <= endDate).ToListAsync();
        }

        /// <summary>
        /// Gets the fields with the dates.
        /// </summary>
        /// <param name="userOrders">the user orders.</param>
        /// <param name="fechaInicio">the init date.</param>
        /// <param name="fechaFin">the end date.</param>
        /// <param name="field">the field to look.</param>
        /// <returns>teh data.</returns>
        private IEnumerable<UserOrderModel> GetDataByDateConvert(List<UserOrderModel> userOrders, DateTime fechaInicio, DateTime fechaFin, string field)
        {
            var listToReturn = new List<UserOrderModel>();

            Parallel.ForEach(userOrders, user =>
            {
                var dateArray = field.Equals(this.CloseDate) ? user.CloseDate.Split("/") : user.FinishDate.Split("/");
                var finishDate = new DateTime(int.Parse(dateArray[2]), int.Parse(dateArray[1]), int.Parse(dateArray[0]));

                if (finishDate >= fechaInicio && finishDate <= fechaFin)
                {
                    lock (listToReturn)
                    {
                        listToReturn.Add(user);
                    }
                }
            });

            return listToReturn;
        }
    }
}

