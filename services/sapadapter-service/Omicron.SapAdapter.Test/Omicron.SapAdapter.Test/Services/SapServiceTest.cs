// <summary>
// <copyright file="SapServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Test.Services
{
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using NUnit.Framework;
    using Omicron.SapAdapter.DataAccess.DAO.Sap;
    using Omicron.SapAdapter.Entities.Context;
    using Omicron.SapAdapter.Services.Sap;

    /// <summary>
    /// class for the test.
    /// </summary>
    [TestFixture]
    public class SapServiceTest
    {
        private ISapService sapService;

        private ISapDao sapDao;

        private DatabaseContext context;

        /// <summary>
        /// The set up.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "Temporal")
                .Options;

            this.context = new DatabaseContext(options);
            this.context.OrderModel.AddRange(this.GetListRoles());
            this.context.SaveChanges();

            this.sapDao = new SapDao(this.context);
            this.sapService = new SapService(this.sapDao);
        }

        [Test]
        public async Task GetOrders()
        {
            var result = await this.sapService.GetOrders();

            Assert.IsNotNull(result);
        }
    }
}
