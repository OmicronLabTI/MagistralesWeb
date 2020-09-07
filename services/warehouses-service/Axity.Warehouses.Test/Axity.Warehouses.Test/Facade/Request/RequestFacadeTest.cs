// <summary>
// <copyright file="RequestFacadeTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Warehouses.Test.Facade.Request
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using AutoMapper;
    using Axity.Warehouses.Dtos.Model;
    using Axity.Warehouses.Entities.Model;
    using Axity.Warehouses.Facade.Request;
    using Axity.Warehouses.Services.Mapping;
    using Axity.Warehouses.Test;
    using Moq;
    using NUnit.Framework;
    using Omicron.Warehouses.Services.Request;

    /// <summary>
    /// Class UsersServiceTest.
    /// </summary>
    [TestFixture]
    public class RequestFacadeTest : BaseTest
    {
        private RequestFacade requestFacade;

        /// <summary>
        /// The init.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            var mapper = mapperConfiguration.CreateMapper();

            var resultDto = AutoFixtureProvider.Create<ResultModel>();
            resultDto.Success = true;

            var mockRequestService = new Mock<IRequestService>();
            mockRequestService.SetReturnsDefault(Task.FromResult(resultDto));

            this.requestFacade = new RequestFacade(mockRequestService.Object, mapper);
        }

        /// <summary>
        /// the processOrders.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task CreateRawMaterialRequest()
        {
            // arrange
            var requests = AutoFixtureProvider.Create<List<RawMaterialRequestDto>>();
            var imageData = File.ReadAllText("SignatureBase64.txt");
            requests.ForEach(x => x.Signature = imageData);

            // act
            var response = await this.requestFacade.CreateRawMaterialRequest("userId", requests);

            // arrange
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }

        /// <summary>
        /// the processOrders.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task UpdateRawMaterialRequest()
        {
            // arrange
            var requests = AutoFixtureProvider.Create<List<RawMaterialRequestDto>>();
            var imageData = File.ReadAllText("SignatureBase64.txt");
            requests.ForEach(x => x.Signature = imageData);

            // act
            var response = await this.requestFacade.UpdateRawMaterialRequest("userId", requests);

            // arrange
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }
    }
}
