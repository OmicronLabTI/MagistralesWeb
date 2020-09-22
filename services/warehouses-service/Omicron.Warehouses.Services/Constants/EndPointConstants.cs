// <summary>
// <copyright file="EndPointConstants.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Warehouses.Services.Constants
{
    /// <summary>
    /// the class for constatns.
    /// </summary>
    public static class EndPointConstants
    {
        /// <summary>
        /// Endpoint to get production orders with formula form sap dapter service.
        /// </summary>
        public const string SapAdapterGetProductionOrders = "fabOrder";

        /// <summary>
        /// Endpoint to get users by id from users service.
        /// </summary>
        public const string UsersGetUsersById = "getUsersById";

        /// <summary>
        /// Endpoint to submit raw material request.
        /// </summary>
        public const string SubmitRawMaterialRequest = "submit/request/rawmaterial/pdf";
    }
}