// <summary>
// <copyright file="ServiceEnums.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Entities.Enums
{
    /// <summary>
    /// the enums for the services.
    /// </summary>
    public static class ServiceEnums
    {
        /// <summary>
        /// Enum for the status.
        /// </summary>
        public enum Status
        {
            /// <summary>
            /// Gets or sets status Asignaodo.
            /// </summary>
            /// <value>The code.</value>
            Asignado = 1,

            /// <summary>
            /// Gets or sets status en proceso.
            /// </summary>
            /// <value>The code.</value>
            Proceso = 2,

            /// <summary>
            /// Gets or sets status pendiente.
            /// </summary>
            /// <value>The code.</value>
            Pendiente = 3,

            /// <summary>
            /// Gets or sets status terminado.
            /// </summary>
            /// <value>The code.</value>
            Terminado = 4,

            /// <summary>
            /// Gets or sets stauts reasignado.
            /// </summary>
            /// <value>The code.</value>
            Reasignado = 5,
        }
    }
}
