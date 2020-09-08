// <summary>
// <copyright file="UserWithOrderCountModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Usuarios.Entities.Model
{
    /// <summary>
    /// class to return all the users with the count of orders.
    /// </summary>
    public class UserWithOrderCountModel
    {
        /// <summary>
        /// Gets or sets Username.
        /// </summary>
        /// <value>
        /// Username.
        /// </value>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets Username.
        /// </summary>
        /// <value>
        /// Username.
        /// </value>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets Username.
        /// </summary>
        /// <value>
        /// Username.
        /// </value>
        public int CountTotalFabOrders { get; set; }

        /// <summary>
        /// Gets or sets Username.
        /// </summary>
        /// <value>
        /// Username.
        /// </value>
        public int CountTotalOrders { get; set; }

        /// <summary>
        /// Gets or sets Username.
        /// </summary>
        /// <value>
        /// Username.
        /// </value>
        public int CountTotalPieces { get; set; }

        /// <summary>
        /// Gets or sets Username.
        /// </summary>
        /// <value>
        /// Username.
        /// </value>
        public int Asignable { get; set; }
    }
}
