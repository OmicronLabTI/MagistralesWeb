// <summary>
// <copyright file="ICatalogDao.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.DataAccess.DAO.Catalog
{
    using Omicron.Catalogos.Entities.Model;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// The catalog Dao
    /// </summary>
    public interface ICatalogDao
    {
        /// <summary>
        /// Get all the roles
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<IEnumerable<RoleModel>> GetAllRoles();

        /// <summary>
        /// Looks the values by field.
        /// </summary>
        /// <param name="fields">the data to look.</param>
        /// <returns>the data to return.</returns>
        Task<IEnumerable<ParametersModel>> GetParamsByField(List<string> fields);

        /// <summary>
        /// Method to obtain warehouses.
        /// </summary>
        /// <param name="warehouses"> warehouses to search. </param>
        /// <returns> boolean indicating whether the insert was successful. </returns>
        Task<List<WarehouseModel>> GetWarehouses(List<string> warehouses);

        /// <summary>
        /// Method to insert new warehouses.
        /// </summary>
        /// <param name="warehouses"> new warehouses to be inserted. </param>
        /// <returns> boolean indicating whether the insert was successful. </returns>
        Task<bool> InsertWarehouses(List<WarehouseModel> warehouses);

        /// <summary>
        /// Method to obtain warehouses.
        /// </summary>
        /// <param name="warehouses"> warehouses to search. </param>
        /// <returns> boolean indicating whether the insert was successful. </returns>
         Task<List<WarehouseModel>> GetActiveWarehouses();

        /// <summary>
        /// Method to insert new sortingroute.
        /// </summary>
        /// <param name="sortingroute"> new sortingroute to be inserted. </param>
        /// <returns> boolean indicating whether the insert was successful. </returns>
        Task<bool> InsertSortingRoute(List<ConfigRoutesModel> sortingroute);

        /// <summary>
        /// Method to obtain classifications.
        /// </summary>
        /// <param name="classifications"> classifications to search. </param>
        /// <returns> boolean indicating whether the insert was successful. </returns>
        Task<List<ConfigRoutesModel>> GetSortingRoutes(List<string> classifications);

        /// <summary>
        /// Gets the configuration route used for processing or classification logic.
        /// </summary>
        /// <returns> A <see cref="Task{TResult}"/> representing the result of the asynchronous operation. </returns>
        Task<List<ConfigRoutesModel>> GetConfigurationRoute();

        /// <summary>
        /// GetConfigRoutesModel.
        /// </summary>
        /// <returns> boolean indicating whether the insert was successful. </returns>
        Task<List<ConfigRoutesModel>> GetConfigRoutesModel();

        /// <summary>
        /// GetActiveClassificationColorsByRoutes.
        /// </summary>
        /// <returns>Active classification colors by routes.</returns>
        Task<List<ConfigRoutesModel>> GetActiveClassificationColorsByRoutes(List<string> routes);
        /// Method to insert new producttypecolors.
        /// </summary>
        /// <param name="producttypecolors"> new producttypecolors to be inserted. </param>
        /// <returns> boolean indicating whether the insert was successful. </returns>
        Task<bool> InsertProductTypecolors(List<ProductTypeColorsModel> producttypecolors);

        /// <summary>
        /// Method to update new producttypecolors.
        /// </summary>
        /// <param name="producttypecolors"> new producttypecolors to be inserted. </param>
        /// <returns> boolean indicating whether the insert was successful. </returns>
        Task<bool> UpdateProductTypecolors(List<ProductTypeColorsModel> producttypecolors);

        /// <summary>
        /// Method to get new temaIds.
        /// </summary>
        /// <param name="temaIds"> new producttypecolors to be inserted. </param>
        /// <returns> boolean indicating whether the insert was successful. </returns>
        Task<List<string>> GetExistingTemaIds(List<string> temaIds);
    }
}
