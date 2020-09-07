// <summary>
// <copyright file="SuccessFailResults.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Axity.Warehouses.Entities.Model
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Dynamic;
    using System.Linq;
    using Axity.Warehouses.Resources.Comparers;

    /// <summary>
    /// Response with success and fail results.
    /// </summary>
    /// <typeparam name="T">Class type.</typeparam>
    public class SuccessFailResults<T>
        where T : class
    {
        /// <summary>
        /// Gets success items on response.
        /// </summary>
        /// <value>
        /// Success values.
        /// </value>
        public List<object> Success { get; private set; } = new List<object>();

        /// <summary>
        /// Gets failed items on response.
        /// </summary>
        /// <value>
        /// Failed values.
        /// </value>
        public List<object> Failed { get; private set; } = new List<object>();

        /// <summary>
        /// Add success result.
        /// </summary>
        /// <typeparam name="T">Type of result.</typeparam>
        /// <param name="item">Result.</param>
        public void AddSuccesResult<T>(T item)
        {
            this.Success.Add(item);
        }

        /// <summary>
        /// Add failed result.
        /// </summary>
        /// <typeparam name="T">Type of result.</typeparam>
        /// <param name="item">Result.</param>
        /// <param name="reason">Failed reason.</param>
        public void AddFailedResult<T>(T item, string reason)
        {
            IDictionary<string, object> failedItem = new ExpandoObject();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(item.GetType()))
            {
                var propName = property.Name;
                propName = char.ToLower(propName[0]) + propName.Substring(1);
                failedItem.Add(propName, property.GetValue(item));
            }

            failedItem["reason"] = reason;
            this.Failed.Add(failedItem);
        }

        /// <summary>
        /// Apply distinc.
        /// </summary>
        /// <returns>Distinct results.</returns>
        public SuccessFailResults<T> DistinctResults()
        {
            this.Success = this.Success.Distinct().ToList();
            this.Failed = this.Failed.Distinct(ExpandoObjectComparer.Default()).ToList();
            return this;
        }
    }
}
