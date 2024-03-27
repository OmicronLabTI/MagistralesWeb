// <summary>
// <copyright file="BaseTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.SapServiceLayerAdapter.Test
{
    /// <summary>
    /// Class Base Test.
    /// </summary>
    public abstract class BaseTest
    {
        /// <summary>
        /// Returns the ressult model.
        /// </summary>
        /// <param name="code">Code.</param>
        /// <param name="success">Is success.</param>
        /// <param name="response">the object for response.</param>
        /// <param name="userError">User error.</param>
        /// <param name="exceptionMessage">Exception Message.</param>
        /// <param name="comments">the comments.</param>
        /// <returns>the data.</returns>
        public ResultModel GetGenericResponseModel(
            int code,
            bool success,
            object response,
            string userError = null,
            string exceptionMessage = null,
            object comments = null)
            => new ()
            {
                Code = code,
                Success = success,
                Response = JsonConvert.SerializeObject(response),
                UserError = userError,
                ExceptionMessage = exceptionMessage,
                Comments = comments,
            };

        /// <summary>
        /// Get OrderDto.
        /// </summary>
        /// <returns>The OrderDto.</returns>
        public IEnumerable<OrderDto> GetOrdersDtol()
            => new List<OrderDto>()
            {
                new () { DocumentEntry = 1 },
            };
    }
}
