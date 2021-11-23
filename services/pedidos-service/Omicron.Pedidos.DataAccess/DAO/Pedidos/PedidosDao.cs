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

        public async Task<IEnumerable<UserOrderModel>> GetUserOrderByUserIdAndStatus(List<string> listIds, List<string> status)
        {
            return await this.databaseContext.UserOrderModel.Where(x => listIds.Contains(x.Userid) && status.Contains(x.Status)).ToListAsync();
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

        public async Task<IEnumerable<UserOrderModel>> GetUserOrderForDelivery(List<string> listStatus)
        {
            return await this.databaseContext.UserOrderModel.Where(x => listStatus.Contains(x.Status) && x.DeliveryId != 0 && !string.IsNullOrEmpty(x.Productionorderid)).ToListAsync();
        }

        /// <summary>
        /// Returns the user order by user id.
        /// </summary>
        /// <param name="fechaInicio">The init date.</param>
        /// <param name="fechaFin">the end date.</param>
        /// <returns>the data.</returns>
        public async Task<IEnumerable<UserOrderModel>> GetUserOrderByFechaFin(DateTime fechaInicio, DateTime fechaFin)
        {
            return await this.databaseContext.UserOrderModel.Where(x => x.FinishDate != null && x.FinishDate >= fechaInicio && x.FinishDate <= fechaFin).ToListAsync();
        }

        /// <summary>
        /// Returns the user order by user id.
        /// </summary>
        /// <param name="fechaInicio">The init date.</param>
        /// <param name="fechaFin">the end date.</param>
        /// <returns>the data.</returns>
        public async Task<IEnumerable<UserOrderModel>> GetUserOrderByFechaClose(DateTime fechaInicio, DateTime fechaFin)
        {
            return await this.databaseContext.UserOrderModel.Where(x => x.CloseDate != null && x.CloseDate >= fechaInicio && x.CloseDate <= fechaFin).ToListAsync();            
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
        public async Task<List<UserOrderModel>> GetSaleOrderForAlmacen(string status, DateTime dateToLook, List<string> statusPending, string secondStatus)
        {
            var listStatustoLook = new List<string> { status, secondStatus };
            var orders = await this.databaseContext.UserOrderModel.Where(x => x.CloseDate != null && x.CloseDate >= dateToLook && x.FinishedLabel == 1 && listStatustoLook.Contains(x.Status)).ToListAsync();

            var idsSaleFinalized = orders.Where(x => x.IsSalesOrder && x.Status.Equals(status)).Select(y => y.Salesorderid).ToList();
            var orderstoReturn = await this.databaseContext.UserOrderModel.Where(x => idsSaleFinalized.Contains(x.Salesorderid)).ToListAsync();

            var maquilaOrders = await this.databaseContext.UserOrderModel.Where(x => x.TypeOrder == "MQ" && x.FinishedLabel == 1 && x.Status == status).ToListAsync();
            orderstoReturn.AddRange(maquilaOrders);

            var possiblePending = orders.Where(x => x.IsProductionOrder).Select(y => y.Salesorderid).Distinct().ToList();
            var isPending = possiblePending.Where(x => !idsSaleFinalized.Any(y => y == x)).ToList();
            var pendingOrders = await this.databaseContext.UserOrderModel.Where(x => isPending.Contains(x.Salesorderid) && x.Status != "Cancelado").ToListAsync();

            pendingOrders.GroupBy(x => x.Salesorderid).ToList().ForEach(y =>
            {
                var orders = y.Where(z => z.IsProductionOrder).ToList();
                var productionStatus = y.Where(z => z.IsProductionOrder && (z.Status == status || z.Status == secondStatus)).ToList();
                if (productionStatus.Any() && 
                    productionStatus.All(z => z.FinishedLabel == 1) && 
                    orders.All(z => statusPending.Contains(z.Status) && 
                    !orders.All(z => z.Status == secondStatus)))
                {
                    orderstoReturn.AddRange(y);
                }
            });
            
            return orderstoReturn;
        }

        /// <inheritdoc/>
        public async Task<List<UserOrderModel>> GetOrderForAlmacenToIgnore(string status, DateTime dateToLook)
        {
            return await this.databaseContext.UserOrderModel.Where(x => string.IsNullOrEmpty(x.Productionorderid) && 
            (x.FinalizedDate == null || x.FinalizedDate >= dateToLook) &&
            (x.Status != status || x.FinishedLabel != 1)).ToListAsync();
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
        public async Task<List<ProductionRemisionQrModel>> GetQrRemisionRouteByDelivery(List<int> delivery)
        {
            return await this.databaseContext.ProductionRemisionQrModel.Where(x => delivery.Contains(x.RemisionId)).ToListAsync();
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
            return await this.databaseContext.UserOrderModel.Where(x => x.StatusAlmacen == statusForSale || x.StatusAlmacen == statusForOrder).ToListAsync();
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

        /// <inheritdoc/>
        public async Task<IEnumerable<UserOrderModel>> GetUserOrderByDeliveryId(List<int> deliveryIds)
        {
            return await this.databaseContext.UserOrderModel.Where(x => deliveryIds.Contains(x.DeliveryId)).ToListAsync();
        }

        /// <summary>
        /// Gets the fields with the dates.
        /// </summary>
        /// <param name="productId">Te product id.</param>
        /// <param name="name">Te product id.</param>
        /// <returns>Related lists.</returns>
        public async Task<List<CustomComponentListModel>> GetCustomComponentListByProductAndName(string productId, string name)
        {
            return await this.databaseContext.CustomComponentLists.Where(x => x.ProductId.Equals(productId) && x.Name.Equals(name) ).ToListAsync();
        }

        /// <summary>
        /// Delete components of custom list.
        /// </summary>
        /// <param name="components">Components of custom list to insert.</param>
        /// <returns>Operation result.</returns> 
        public async Task<bool> DeleteComponentsOfCustomList(List<ComponentCustomComponentListModel> components)
        {
            this.databaseContext.ComponentsCustomComponentLists.RemoveRange(components);
            await ((DatabaseContext)this.databaseContext).SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Delete custom component list.
        /// </summary>
        /// <param name="customComponentList">Custom list to insert.</param>
        /// <returns>Operation result</returns>
        public async Task<bool> DeleteCustomComponentList(CustomComponentListModel customComponentList)
        {
            this.databaseContext.CustomComponentLists.Remove(customComponentList);
            await ((DatabaseContext)this.databaseContext).SaveChangesAsync();
            return true;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<UserOrderModel>> GetUserOrderByPlanningDate(DateTime fechaInicio, DateTime fechaFin)
        {
            return await this.databaseContext.UserOrderModel.Where(x => x.PlanningDate != null && x.PlanningDate >= fechaInicio && x.PlanningDate <= fechaFin).ToListAsync();
        }
    }
}

