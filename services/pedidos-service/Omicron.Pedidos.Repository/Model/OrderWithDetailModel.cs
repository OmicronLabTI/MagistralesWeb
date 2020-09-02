// <summary>
// <copyright file="OrderWithDetailModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Entities.Model
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// object with order and detail.
    /// </summary>
    public class OrderWithDetailModel
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public OrderModel Order { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public List<CompleteDetailOrderModel> Detalle { get; set; }

        /// <summary>
        /// Conver instance to list of user order models.
        /// </summary>
        /// <returns>User order models.</returns>
        public List<UserOrderModel> ToUserOrderModels()
        {
            var userOrderModels = new List<UserOrderModel>();
            userOrderModels.Add(new UserOrderModel
            {
                Salesorderid = this.Order.DocNum.ToString(),
            });

            userOrderModels = this.Detalle.Select(x => new UserOrderModel
            {
                Salesorderid = this.Order.DocNum.ToString(),
                Productionorderid = x.OrdenFabricacionId.ToString(),
            }).ToList();
            return userOrderModels;
        }
    }
}
