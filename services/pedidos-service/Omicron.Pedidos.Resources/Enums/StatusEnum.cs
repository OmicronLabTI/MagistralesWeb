// <summary>
// <copyright file="StatusEnum.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Resources.Enums
{
    /// <summary>
    /// SignatureTypes.
    /// </summary>
    public enum StatusEnum
    {
        /// <summary>
        /// Represents a logisticts signature.
        /// </summary>
        Abierto = 0,

        /// <summary>
        /// Represents a logisticts signature.
        /// </summary>
        Planificado = 1,

        /// <summary>
        /// Represents a logisticts signature.
        /// </summary>
        Asignado = 2,

        /// <summary>
        /// Represents a logisticts signature.
        /// </summary>
        Reasignado = 3,

        /// <summary>
        /// Represents a logisticts signature.
        /// </summary>
        Pendiente = 4,

        /// <summary>
        /// Represents a logisticts signature.
        /// </summary>
        Proceso = 5,

        /// <summary>
        /// Represents a logisticts signature.
        /// </summary>
        Terminado = 6,

        /// <summary>
        /// Represents a logisticts signature.
        /// </summary>
        Finalizado = 7,

        /// <summary>
        /// Represents a logisticts signature.
        /// </summary>
        Almacenado = 7,

        /// <summary>
        /// Represents a logisticts signature.
        /// </summary>
        Entregado = 7,

        /// <summary>
        /// Represents a logisticts signature.
        /// </summary>
        Cancelado = 8,
    }
}
