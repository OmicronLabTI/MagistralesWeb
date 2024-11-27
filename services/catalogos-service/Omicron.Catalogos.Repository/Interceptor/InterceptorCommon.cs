// <summary>
// <copyright file="InterceptorCommon.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.Entities.Interceptor
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
                DateTimeKind.Unspecified => DateTime.SpecifyKind(TimeZoneInfo.ConvertTimeToUtc(dateTime, MexicoTimeZone), DateTimeKind.Utc),
                DateTimeKind.Local => dateTime.ToUniversalTime(),
                _ => dateTime // Ya es UTC
            };
        }
    }
}
