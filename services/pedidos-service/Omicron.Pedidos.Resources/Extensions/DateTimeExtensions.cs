// <summary>
// <copyright file="DateTimeExtensions.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Pedidos.Resources.Extensions
{
    using System;

    /// <summary>
    /// Extensions for datetime object.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Create formated date time.
        /// </summary>
        /// <param name="dateTime">Date time instance.</param>
        /// <returns>Formated string date.</returns>
        public static string FormatedDate(this DateTime dateTime)
        {
            return DateTime.Now.ToString("dd/MM/yyyy");
        }

        /// <summary>
        /// Create formated date time.
        /// </summary>
        /// <param name="dateTime">Date time instance.</param>
        /// <returns>Formated string date.</returns>
        public static string FormatedLargeDate(this DateTime dateTime)
        {
            return DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
        }
    }
}
