// <summary>
// <copyright file="ErrorReasonConstants.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Warehouses.Services.Constants
{
    /// <summary>
    /// the class for constatns.
    /// </summary>
    public static class ErrorReasonConstants
    {
        /// <summary>
        /// Reason raw material request already exists.
        /// </summary>
        public const string ReasonRawMaterialRequestAlreadyExists = "La solicitud de materia prima para la orden de fabricación {0} ya existe.";

        /// <summary>
        /// Reason raw material request not exists.
        /// </summary>
        public const string ReasonRawMaterialRequestNotExists = "La solicitud de materia prima para la orden de fabricación {0} no existe.";
    }
}
