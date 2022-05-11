// <summary>
// <copyright file="CommonCall.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Reporting.Services.Utils
{
    /// <summary>
    /// common call services.
    /// </summary>
    public static class CommonCall
    {
        /// <summary>
        /// Calculate value from validation.
        /// </summary>
        /// <typeparam name="T">The Type T.</typeparam>
        /// <param name="validation">Validation.</param>
        /// <param name="value">True value.</param>
        /// <param name="defaultValue">False value.</param>
        /// <returns>Result.</returns>
        public static T CalculateTernary<T>(bool validation, T value, T defaultValue)
        {
            return validation ? value : defaultValue;
        }
    }
}
