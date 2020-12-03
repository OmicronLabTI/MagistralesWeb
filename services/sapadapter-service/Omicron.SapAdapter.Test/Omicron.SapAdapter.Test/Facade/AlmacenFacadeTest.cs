// <summary>
// <copyright file="AlmacenFacadeTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Test.Facade
{
    using AutoMapper;
    using Moq;
    using NUnit.Framework;
    using Omicron.SapAdapter.Entities.Model;
    using Omicron.SapAdapter.Facade.Sap;
    using Omicron.SapAdapter.Services.Sap;
    using System.Threading.Tasks;

    /// <summary>
    /// Class for the QR test.
    /// </summary>
    [TestFixture]
    public class AlmacenFacadeTest : BaseTest
    {
        private SapAlmacenFacade almacenFacade;

        /// <summary>
        /// The init.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            var mapper = mapperConfiguration.CreateMapper();

            var response = new ResultModel
            {
                Success = true,
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = string.Empty,
                UserError = string.Empty,
            };

            var mockService = new Mock<ISapAlmacenService>();

            mockService
                .Setup(m => m.GetOrders()))
                .Returns(Task.FromResult(response));

            this.almacenFacade = new SapAlmacenFacade(mapper, mockService.Object);
        }

        /// <summary>
        /// Test the get orders.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetPedidos()
        {
            var response = await this.almacenFacade.GetOrders();

            Assert.IsNotNull(response);
        }
    }
}
