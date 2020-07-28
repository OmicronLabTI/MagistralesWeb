// <summary>
// <copyright file="ServiceConstants.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Usuarios.Services.Constants
{
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
        public const string IncorrectPassword = "La contraseña es incorrecta, intenta de nuevo";

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
    }
}
