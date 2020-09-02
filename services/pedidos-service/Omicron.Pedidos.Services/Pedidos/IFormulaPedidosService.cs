// <summary>
// <copyright file="IFormulaPedidosService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Pedidos.Services.Pedidos
{
    using System.Threading.Tasks;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Entities.Model.Db;

    /// <summary>
    /// Contract for formulas.
    /// </summary>
    public interface IFormulaPedidosService
    {
        /// <summary>
        /// Create custom component list.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="customList">The custom list.</param>
        /// <returns>New custom list.</returns>
        Task<ResultModel> CreateCustomComponentList(string userId, CustomComponentListModel customList);
    }
}
