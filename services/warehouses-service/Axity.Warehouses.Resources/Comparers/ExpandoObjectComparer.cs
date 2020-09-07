// <summary>
// <copyright file="ExpandoObjectComparer.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Axity.Warehouses.Resources.Comparers
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;

    /// <summary>
    /// Custom exapndo object comparer.
    /// </summary>
    public class ExpandoObjectComparer : IEqualityComparer<object>
    {
        private ExpandoObjectComparer()
        {
        }

        /// <summary>
        /// Create default instance.
        /// </summary>
        /// <returns>Comparer instance-
        /// .</returns>
        public static ExpandoObjectComparer Default()
        {
            return new ExpandoObjectComparer();
        }

        /// <summary>
        /// Compare elements.
        /// </summary>
        /// <param name="x">First element.</param>
        /// <param name="y">Second element.</param>
        /// <returns>Comparision result.</returns>
        public bool Equals(object x, object y)
        {
            if (object.ReferenceEquals(x, y))
            {
                return true;
            }

            if (x.GetType().Equals(y.GetType()) && x.GetType().Equals(typeof(ExpandoObject)))
            {
                var xkeyValues = new Dictionary<string, object>(x as IDictionary<string, object>);
                var ykeyValues = new Dictionary<string, object>(y as IDictionary<string, object>);

                var xfieldsCount = xkeyValues.Count;
                var yfieldsCount = ykeyValues.Count;

                if (xfieldsCount != yfieldsCount)
                {
                    return false;
                }

                var missingKey = xkeyValues.Keys.FirstOrDefault(k => !ykeyValues.ContainsKey(k));
                if (missingKey != null)
                {
                    return false;
                }

                foreach (var keyValue in xkeyValues)
                {
                    var key = keyValue.Key;
                    var xvalueItem = keyValue.Value;
                    var yvalueItem = ykeyValues[key];

                    if (xvalueItem == null && yvalueItem != null)
                    {
                        return false;
                    }

                    if (xvalueItem != null && yvalueItem == null)
                    {
                        return false;
                    }

                    if (xvalueItem != null && !xvalueItem.Equals(yvalueItem))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Get hash code.
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <returns>Hash cpde.</returns>
        public int GetHashCode(object obj)
        {
            int hashCode = 0;

            Func<object, int> getHash = item =>
            {
                if (item == null)
                {
                    return 0;
                }

                return item.GetHashCode();
            };

            if (obj.GetType().Equals(typeof(ExpandoObject)))
            {
                unchecked
                {
                    var fieldValues = new Dictionary<string, object>(obj as IDictionary<string, object>);
                    fieldValues.Values.ToList().ForEach(v => hashCode ^= getHash(v));
                }

                return hashCode;
            }
            else
            {
                return obj.GetHashCode();
            }
        }
    }
}