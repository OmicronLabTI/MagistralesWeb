// <summary>
// <copyright file="DictionaryExtensions.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Resources.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Net;

    /// <summary>
    /// Dictionary extensions.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Decode a query string parameters.
        /// </summary>
        /// <param name="parameters">Encode parameters.</param>
        /// <returns>Decoded parameters.</returns>
        public static Dictionary<string, string> DecodeQueryString(this Dictionary<string, string> parameters)
        {
            var newParameters = new Dictionary<string, string>();
            foreach (var item in parameters)
            {
                newParameters.Add(item.Key, WebUtility.UrlDecode(item.Value));
            }

            return newParameters;
        }

        /// <summary>
        /// Get value.
        /// </summary>
        /// <typeparam name="T">Key type.</typeparam>
        /// <typeparam name="TU">Value type.</typeparam>
        /// <typeparam name="TV">New value type.</typeparam>
        /// <param name="dictionary">Dictionary with values.</param>
        /// <param name="key">Property with value.</param>
        /// <returns>Converted value.</returns>
        public static TV Get<T, TU, TV>(this IDictionary<T, TU> dictionary, T key)
            where TV : IConvertible
        {
            Type type = typeof(TV);

            if (!dictionary.Keys.Any(x => x.Equals(key)))
            {
                throw new KeyNotFoundException(key.ToString());
            }

            object value = dictionary[key];
            TypeConverter converter = TypeDescriptor.GetConverter(type);

            if (type.IsNumericType() && (value == null || string.IsNullOrEmpty(value.ToString().Trim())))
            {
                value = 0;
            }

            var stringValue = value != null ? value.ToString().Trim() : string.Empty;

            return (TV)converter.ConvertFromString(stringValue);
        }

        /// <summary>
        /// Get value.
        /// </summary>
        /// <typeparam name="T">Key type.</typeparam>
        /// <typeparam name="TU">Value type.</typeparam>
        /// <typeparam name="TV">New value type.</typeparam>
        /// <param name="dictionary">Dictionary with values.</param>
        /// <param name="key">Property with value.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <param name="value">Property with new value.</param>
        /// <returns>Converted value.</returns>
        public static bool TryGet<T, TU, TV>(this IDictionary<T, TU> dictionary, T key, TV defaultValue, out TV value)
            where TV : IConvertible
        {
            try
            {
                value = dictionary.Get<T, TU, TV>(key);
            }
            catch (Exception ex)
            {
                value = defaultValue;
            }

            return true;
        }
    }
}
