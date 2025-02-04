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
            if (dateTime.Kind == DateTimeKind.Unspecified && IsInvalidTime(MexicoTimeZone, dateTime))
            {
                dateTime = dateTime.AddMinutes(60);
            }

            return dateTime.Kind switch
            {
                DateTimeKind.Unspecified => DateTime.SpecifyKind(TimeZoneInfo.ConvertTimeToUtc(dateTime, MexicoTimeZone), DateTimeKind.Utc),
                DateTimeKind.Local => dateTime.ToUniversalTime(),
                _ => dateTime // Ya es UTC
            };
        }

        private static bool IsInvalidTime(TimeZoneInfo timeZone, DateTime dateTime)
        {
            // Obtener las reglas de ajuste (Daylight Saving Time)
            var adjustmentRules = timeZone.GetAdjustmentRules();

            foreach (var adjustmentRule in adjustmentRules)
            {
                // Verificar si la fecha está dentro del periodo de ajuste (DST)
                if (adjustmentRule.DateStart <= dateTime.Date && adjustmentRule.DateEnd >= dateTime.Date)
                {
                    // Obtener el periodo en el que el cambio de hora ocurre
                    var transitionStart = adjustmentRule.DaylightTransitionStart;
                    var transitionEnd = adjustmentRule.DaylightTransitionEnd;

                    // Calcular la fecha de inicio y fin del cambio de hora
                    DateTime startTransition = GetTransitionDate(dateTime.Year, transitionStart);
                    DateTime endTransition = GetTransitionDate(dateTime.Year, transitionEnd);

                    // Verificar si el tiempo cae dentro del periodo inválido
                    if (dateTime >= startTransition && dateTime < startTransition.Add(adjustmentRule.DaylightDelta))
                    {
                        return true; // Tiempo inválido
                    }
                }
            }

            return false; // Tiempo válido
        }

        private static DateTime GetTransitionDate(int year, TimeZoneInfo.TransitionTime transition)
        {
            DateTime timeOfTransition = new DateTime(year, transition.Month, 1).AddHours(transition.TimeOfDay.Hour);

            // Encontrar el día de la transición (ejemplo: primer domingo)
            int dayOfWeek = (int)transition.DayOfWeek;
            int currentDayOfWeek = (int)timeOfTransition.DayOfWeek;
            int deltaDays = (dayOfWeek - currentDayOfWeek + 7) % 7;

            // Ajustar al primer, segundo, etc., domingo de ese mes
            return timeOfTransition.AddDays(deltaDays + (7 * (transition.Week - 1)));
        }
    }
}
