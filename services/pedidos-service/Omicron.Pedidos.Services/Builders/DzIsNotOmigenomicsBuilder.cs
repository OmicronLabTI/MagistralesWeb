// <summary>
// <copyright file="DzIsNotOmigenomicsBuilder.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.Builders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Newtonsoft.Json;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Resources.Exceptions;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.Utils;

    /// <summary>
    /// DzIsNotOmigenomicsBuilder class.
    /// </summary>
    public class DzIsNotOmigenomicsBuilder
    {
        private readonly List<OrderWithDetailModel> ordersSap;
        private readonly List<UserModel> usersDZ;
        private readonly List<OrderWithDetailModel> ordersDzIsNotOmi;

        /// <summary>
        ///  Initializes a new instance of the <see cref="DzIsNotOmigenomicsBuilder"/> class.
        /// </summary>
        /// <param name="orders">Result model order SAP.</param>
        /// <param name="users">User Orders.</param>
        /// <param name="ordersSap">Orders Sap.</param>
        public DzIsNotOmigenomicsBuilder(ResultModel orders, List<UserModel> users, List<OrderWithDetailModel> ordersSap)
        {
            this.ordersSap = ordersSap.ThrowIfNull(nameof(ordersSap));
            this.ordersDzIsNotOmi = JsonConvert.DeserializeObject<List<OrderWithDetailModel>>(
                JsonConvert.SerializeObject(orders.ThrowIfNull(nameof(orders)).Response))
                .FindAll(x => x.Detalle.Any(AsignarLogic.IsDzAndIsNotOmigenomics))
                .OrderBy(o => o.Order.PedidoId).ToList();
            this.usersDZ = users.ThrowIfNull(nameof(users))
                .FindAll(x => x.Classification.ToUpper().Equals(ServiceConstants.UserClassificationDZ))
                .OrderBy(user => user.Piezas).ToList();
        }

        /// <summary>
        /// Method to Assign Orders with products dz is not omigenomecs to users with classification DZ.
        /// </summary>
        /// <returns>Orders with products dz is not omigenomecs to users with classification DZ relation.</returns>
        public List<RelationUserDZAndOrdersDZModel> AssignOrdersToUsersDZIsNotOmi()
        {
            this.SetOrdersDzAndIsNotOmigenomics();
            var userCount = this.usersDZ.Any() ? this.usersDZ.Count : 1;
            var iterations = (int)Math.Ceiling((decimal)this.ordersDzIsNotOmi.Count / userCount);
            return Enumerable.Range(0, iterations)
                .SelectMany(index =>
                    this.ordersDzIsNotOmi
                    .Skip(index == 0 ? index : userCount * index)
                    .Take(userCount)
                    .Select((order, indexInt) =>
                    new RelationUserDZAndOrdersDZModel
                    {
                        UserId = this.usersDZ[indexInt].Id,
                        Order = order,
                    })).ToList();
        }

        private void SetOrdersDzAndIsNotOmigenomics()
        {
            this.ordersDzIsNotOmi.ForEach(orderDzIsNotOmi => orderDzIsNotOmi.Detalle = orderDzIsNotOmi.Detalle.Where(AsignarLogic.IsDzAndIsNotOmigenomics).ToList());
            this.ordersDzIsNotOmi.RemoveAll(order => !order.Detalle.Any());

            this.ordersSap.ForEach(orderSap => orderSap.Detalle.RemoveAll(AsignarLogic.IsDzAndIsNotOmigenomics));
            this.ordersSap.RemoveAll(order => !order.Detalle.Any());

            if (this.ordersDzIsNotOmi.Any() && !this.usersDZ.Any())
            {
                throw new CustomServiceException(ServiceConstants.ErrorUsersDZAutomatico, HttpStatusCode.BadRequest);
            }
        }
    }
}
