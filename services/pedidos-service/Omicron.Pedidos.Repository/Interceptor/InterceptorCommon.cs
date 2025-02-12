// <summary>
// <copyright file="InterceptorCommon.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Entities.Interceptor
{
    using System;

    /// <summary>
    /// Interceptor common functions.
    /// </summary>
    public static class InterceptorCommon
    {
        private static readonly TimeZoneInfo MexicoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Mexico_City");

        /// <summary>
        /// Convert to UTC.
        /// </summary>
        /// <param name="dateTime">Date Time.</param>
        /// <returns>UtC DateTime.</returns>
        public static DateTime ConvertToUtc(DateTime dateTime)
        {
            return dateTime.Kind switch
            {
                DateTimeKind.Unspecified => HandleUnspecifiedDateTime(dateTime),
                DateTimeKind.Local => dateTime.ToUniversalTime(),
                DateTimeKind.Utc => dateTime, // Ya es UTC
                _ => HandleDateTimeWithOffset(dateTime) // No deberíamos llegar aquí, pero por seguridad
            };
        }

        private static DateTime HandleUnspecifiedDateTime(DateTime dateTime)
        {
            if (DateTimeOffset.TryParse(dateTime.ToString(), out DateTimeOffset dateTimeOffset))
            {
                return dateTimeOffset.UtcDateTime;
            }

            return DateTime.SpecifyKind(TimeZoneInfo.ConvertTimeToUtc(dateTime, MexicoTimeZone), DateTimeKind.Utc);
        }

        private static DateTime HandleDateTimeWithOffset(DateTime dateTime)
        {
            if (DateTimeOffset.TryParse(dateTime.ToString(), out DateTimeOffset dateTimeOffset))
            {
                return dateTimeOffset.UtcDateTime;
            }

            return dateTime;
        }
    }
}
