using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace EFCorePractice.Tests
{
    public class DiscriminatorTests : IClassFixture<DbContextFixture>
    {
        DbContextFixture dbFixture;
        private readonly ITestOutputHelper output;

        public DiscriminatorTests(ITestOutputHelper output, DbContextFixture fixture)
        {
            this.output = output;
            this.dbFixture = fixture;
        }

        [Fact]
        public async Task Discriminator_Query()
        {
            // Arrange
            var context = await dbFixture.CreateContextAsync();

            // Act
            var mobileContract = new MobileContract
            {
                MobileNumber = "13032348711",
                Charge = 1.05M,
                Months = 12,
                StartDate = DateTime.UtcNow.Date
            };
            var broadbandContract = new BroadbandContract
            {
                DownloadSpeed = 100,
                Charge = 1.05M,
                Months = 12,
                StartDate = DateTime.UtcNow.Date
            };
            context.AddRange(mobileContract, broadbandContract);
            await context.SaveChangesAsync();

            // Assert
            Assert.Equal(1, context.Entry(mobileContract).Property("ContractType").CurrentValue);
            Assert.Equal(2, context.Entry(broadbandContract).Property("ContractType").CurrentValue);
        }
    }
}
