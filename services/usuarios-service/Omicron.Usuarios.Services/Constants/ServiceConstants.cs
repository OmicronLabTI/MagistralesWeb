// <summary>
// <copyright file="ServiceConstants.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Usuarios.Services.Constants
{
    using System.Collections.Generic;

    /// <summary>
    /// Class for constants.
    /// </summary>
    public static class ServiceConstants
    {
        /// <summary>
        /// the logic error.
        /// </summary>
        public const int LogicError = 300;

        /// <summary>
        /// The status ok.
        /// </summary>
        public const int StatusOk = 200;

        /// <summary>
        /// if the user doesnt exist.
        /// </summary>
        public const string UserDontExist = "El usuario no existe, favor de comunicarse con el administrador";

        /// <summary>
        /// If the password is incorrect.
        /// </summary>
        public const string IncorrectPass = "La contraseña es incorrecta, intenta de nuevo";

        /// <summary>
        /// Text when the username already exists.
        /// </summary>
        public const string UserAlreadyExist = "El usuario ya existe, favor de elegir otro.";

        /// <summary>
        /// Error while saving the user.
        /// </summary>
        public const string ErrorWhileInsertingUser = "Error al guardar usuario.";

        /// <summary>
        /// const for offset.
        /// </summary>
        public const string Offset = "offset";

        /// <summary>
        /// Const for the limit.
        /// </summary>
        public const string Limit = "limit";

        /// <summary>
        /// Const for the user.
        /// </summary>
        public const string UserName = "user";

        /// <summary>
        /// the first name.
        /// </summary>
        public const string FirstName = "fname";

        /// <summary>
        /// the last name.
        /// </summary>
        public const string LastName = "lname";

        /// <summary>
        /// the role.
        /// </summary>
        public const string Role = "role";

        /// <summary>
        /// the assignable value.
        /// </summary>
        public const string Assignable = "assignable";

        /// <summary>
        /// the status.
        /// </summary>
        public const string Status = "status";

        /// <summary>
        /// the qfb role.
        /// </summary>
        public const int RoleQfb = 2;

        /// <summary>
        /// gets the roles from catalogservice.
        /// </summary>
        public const string QfbOrders = "qfbOrders";

        /// <summary>
        /// Get fab orders.
        /// </summary>
        public const string GetFabOrders = "fabOrderId";

        /// <summary>
        /// the qfb role.
        /// </summary>
        public const string Qfb = "qfb";

        /// <summary>
        /// the qfb classification.
        /// </summary>
        public const string TypeQfb = "typeQfb";

        /// <summary>
        /// Gets list of thw status for the orders.
        /// </summary>
        /// <value>
        /// List of thw status for the orders.
        /// </value>
        public static List<string> ListStatusOrdenes { get; } = new List<string>
        {
            "Asignado",
            "Proceso",
            "Pendiente",
            "Reasignado",
            "Terminado",
        };
    }
}
