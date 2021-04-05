// <summary>
// <copyright file="AdvanceLokkServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Test.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Moq;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using Omicron.LeadToCash.Resources.Exceptions;
    using Omicron.SapAdapter.DataAccess.DAO.Sap;
    using Omicron.SapAdapter.Entities.Context;
    using Omicron.SapAdapter.Entities.Model;
    using Omicron.SapAdapter.Entities.Model.BusinessModels;
    using Omicron.SapAdapter.Services.Constants;
    using Omicron.SapAdapter.Services.Pedidos;
    using Omicron.SapAdapter.Services.Redis;
    using Omicron.SapAdapter.Services.Sap;
    using Omicron.SapAdapter.Services.User;
    using Omicron.SapAdapter.Services.Utils;
    using Serilog;

    /// <summary>
    /// Class UsersServiceTest.
    /// </summary>
    [TestFixture]
    public class AdvanceLokkServiceTest : BaseTest
    {
        private IAdvanceLookService advanceLookService;

        private ISapDao sapDao;

        private ISapService sapService;

        private DatabaseContext context;

        /// <summary>
        /// The set up.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TemporalAdvancedLook")
                .Options;

            this.context = new DatabaseContext(options);
            this.context.AsesorModel.Add(this.GetAsesorModel());
            this.context.DetallePedido.AddRange(this.GetDetallePedido());
            this.context.OrdenFabricacionModel.AddRange(this.GetOrdenFabricacionModel());
            this.context.OrderModel.AddRange(this.GetOrderModel());
            this.context.ProductoModel.AddRange(this.GetProductoModel());
            this.context.Users.AddRange(this.GetSapUsers());
            this.context.DetalleFormulaModel.AddRange(this.GetDetalleFormula());
            this.context.ItemWarehouseModel.AddRange(this.GetItemWareHouse());
            this.context.Batches.AddRange(this.GetBatches());
            this.context.BatchesQuantity.AddRange(this.GetBatchesQuantity());
            this.context.BatchTransacitions.AddRange(this.GetBatchTransacitions());
            this.context.BatchesTransactionQtyModel.AddRange(this.GetBatchesTransactionQtyModel());
            this.context.AttachmentModel.AddRange(this.GetAttachmentModel());

            this.context.SaveChanges();
            var mockPedidoService = new Mock<IPedidosService>();
            var mockUserService = new Mock<IUsersService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockRedis = new Mock<IRedisService>();

            mockConfiguration.SetupGet(x => x[It.Is<string>(s => s == "SapOmicron:BatchCodes:prefix")]).Returns("L-");
            mockConfiguration.SetupGet(x => x[It.Is<string>(s => s == "SapOmicron:BatchCodes:numberPositions")]).Returns("7");
            mockConfiguration.SetupGet(x => x[It.Is<string>(s => s == "OmicronRecipeAddress")]).Returns("http://localhost:5002/");

            mockPedidoService
                .Setup(m => m.GetUserPedidos(It.IsAny<List<int>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultGetUserPedidos()));

            mockPedidoService
                .Setup(m => m.GetPedidosService(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultDtoGetPedidosService()));

            mockUserService
                .Setup(m => m.GetUsersById(It.IsAny<List<string>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultDtoGetUsersById()));

            var mockLog = new Mock<ILogger>();

            mockLog
                .Setup(m => m.Information(It.IsAny<string>()));

            mockRedis
                .Setup(m => m.WriteToRedis(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()));

            this.sapDao = new SapDao(this.context, mockLog.Object);
            IGetProductionOrderUtils getProdUtils = new GetProductionOrderUtils(this.sapDao, mockLog.Object);
            this.sapService = new SapService(this.sapDao, mockPedidoService.Object, mockUserService.Object, mockConfiguration.Object, mockLog.Object, getProdUtils, mockRedis.Object);
        }

        /// <summary>
        /// gets the orders test.
        /// </summary>
        /// <returns>the orders.</returns>
        [Test]
        public async Task GetOrdersFechaIni()
        {
            // arrange
            var dates = DateTime.Now.ToString("dd/MM/yyyy");
            var dateFinal = DateTime.Now.AddDays(2).ToString("dd/MM/yyyy");
            var dicParams = new Dictionary<string, string>
            {
                { ServiceConstants.FechaInicio, string.Format("{0}-{1}", dates, dateFinal) },
            };

            // act
            var result = await this.sapService.GetOrders(dicParams);

            Assert.IsNotNull(result);
        }
    }
}
