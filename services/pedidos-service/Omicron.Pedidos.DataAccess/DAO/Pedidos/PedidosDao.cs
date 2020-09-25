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

