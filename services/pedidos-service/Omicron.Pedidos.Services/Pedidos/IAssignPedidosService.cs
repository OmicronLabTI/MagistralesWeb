// <summary>
// <copyright file="IAssignPedidosService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.Pedidos
{
    using System.Threading.Tasks;
    using Omicron.Pedidos.Entities.Model;

    /// <summary>
    /// the Assign interface.
    /// </summary>
    public interface IAssignPedidosService
    {
        /// <summary>
        /// Assign the orders.
        /// </summary>
        /// <param name="manualAssign">the manual assign.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> AssignOrder(ManualAssignModel manualAssign);

        /// <summary>
        /// Makes the automatic assign.
        /// </summary>
        /// <param name="assignModel">the assign model.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> AutomaticAssign(AutomaticAssingModel assignModel);

        /// <summary>
        /// Reassign the ordr to a user.
        /// </summary>
        /// <param name="manualAssign">the objecto to assign.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> ReassignOrder(ManualAssignModel manualAssign);
    }
}
