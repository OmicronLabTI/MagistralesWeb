// <summary>
// <copyright file="IDatabaseContext.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Entities.Context
{
    using Microsoft.EntityFrameworkCore;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Entities.Model.Db;

    /// <summary>
    /// Interface IDataBaseContext.
    /// </summary>
    public interface IDatabaseContext
    {
        /// <summary>
        /// Gets or sets CatUser.
        /// </summary>
        /// <value>
        /// Object UserModel CatUser.
        /// </value>
        DbSet<UserModel> CatUser { get; set; }

        /// <summary>
        /// Gets or sets CatUser.
        /// </summary>
        /// <value>
        /// Object UserModel CatUser.
        /// </value>
        DbSet<UserOrderModel> UserOrderModel { get; set; }

        /// <summary>
        /// Gets or sets CatUser.
        /// </summary>
        /// <value>
        /// Object UserModel CatUser.
        /// </value>
        DbSet<OrderLogModel> OrderLogModel { get; set; }

        /// <summary>
        /// Gets or sets UserOrderSignature.
        /// </summary>
        /// <value>
        /// Object UserModel UserOrderSignature.
        /// </value>
        DbSet<UserOrderSignatureModel> UserOrderSignatureModel { get; set; }

        /// <summary>
        /// Gets or sets custom component lists.
        /// </summary>
        /// <value>
        /// Object custom lists.
        /// </value>
        DbSet<CustomComponentListModel> CustomComponentLists { get; set; }

        /// <summary>
        /// Gets or sets components of custom lists .
        /// </summary>
        /// <value>
        /// Object componets of custom lists.
        /// </value>
        DbSet<ComponentCustomComponentListModel> ComponentsCustomComponentLists { get; set; }

        /// <summary>
        /// Gets or sets components of custom lists .
        /// </summary>
        /// <value>
        /// Object componets of custom lists.
        /// </value>
        DbSet<ParametersModel> ParametersModel { get; set; }

        /// <summary>
        /// Gets or sets components of custom lists .
        /// </summary>
        /// <value>
        /// Object componets of custom lists.
        /// </value>
        DbSet<ProductionOrderQr> ProductionOrderQr { get; set; }
    }
}
