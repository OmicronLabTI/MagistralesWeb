// <summary>
// <copyright file="ServiceConstants.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Services.Constants
{
    using System.Collections.Generic;

    /// <summary>
    /// The constants classs.
    /// </summary>
    public static class ServiceConstants
    {
        /// <summary>
        /// the filter for orders.
        /// </summary>
        public const string DocNum = "docNum";

        /// <summary>
        /// the filter for orders.
        /// </summary>
        public const string FechaInicio = "fini";

        /// <summary>
        /// the filter for orders.
        /// </summary>
        public const string FechaFin = "ffin";

        /// <summary>
        /// the filter for orders.
        /// </summary>
        public const string Status = "status";

        /// <summary>
        /// the filter for orders.
        /// </summary>
        public const string Qfb = "qfb";

        /// <summary>
        /// const for offset.
        /// </summary>
        public const string Offset = "offset";

        /// <summary>
        /// Const for the limit.
        /// </summary>
        public const string Limit = "limit";

        /// <summary>
        /// the filter for orders.
        /// </summary>
        public const string ItemCode = "code";

        /// <summary>
        /// the abierto status.
        /// </summary>
        public const string Abierto = "Abierto";

        /// <summary>
        /// value for chips.
        /// </summary>
        public const string Chips = "chips";

        /// <summary>
        /// values cuando no hay chips.
        /// </summary>
        public const string NoChipsError = "No se encontraron filtros";

        /// <summary>
        /// get the chips.
        /// </summary>
        public const string ChipSeparator = ",";

        /// <summary>
        /// the proceso status.
        /// </summary>
        public const string Proceso = "Proceso";

        /// <summary>
        /// en proceso to show.
        /// </summary>
        public const string EnProceso = "En proceso";

        /// <summary>
        /// en proceso to show.
        /// </summary>
        public const string Terminado = "Terminado";

        /// <summary>
        /// Gets the status of the order.
        /// </summary>
        /// <value>
        /// the status.
        /// </value>
        public static Dictionary<string, string> DictStatus { get; } = new Dictionary<string, string>
        {
            { "P", "Planificado" },
            { "L", "Cerrado" },
            { "C", "Cancelado" },
            { "R", "Liberado" },
        };

        /// <summary>
        /// Gets the status of the order.
        /// </summary>
        /// <value>
        /// the status.
        /// </value>
        public static Dictionary<string, string> DictStatusType { get; } = new Dictionary<string, string>
        {
            { "S", "Estandar" },
        };

        /// <summary>
        /// Gets the status of the order.
        /// </summary>
        /// <value>
        /// the status.
        /// </value>
        public static Dictionary<string, string> DictStatusOrigin { get; } = new Dictionary<string, string>
        {
            { "M", "Manual" },
        };

        /// <summary>
        /// Gets the status of the order.
        /// </summary>
        /// <value>
        /// the status.
        /// </value>
        public static Dictionary<string, string> DictUrlEncode { get; } = new Dictionary<string, string>
        {
            { "%C3%9C", "Ü" },
            { "%C3%BC", "ü" },
        };
    }
}
