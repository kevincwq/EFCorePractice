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
    public class DiscriminatorTests : TestBase
    {
        public DiscriminatorTests(ITestOutputHelper output, DbContextFixture fixture)
            : base(output, fixture)
        {
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
            var tvContract = new TvContract
            {
                PackageType = PackageType.M,
                Charge = 1.05M,
                Months = 12,
                StartDate = DateTime.UtcNow.Date
            };
            context.AddRange(mobileContract, broadbandContract, tvContract);
            await context.SaveChangesAsync();

            // Assert
            Assert.Equal("Mobile", context.Entry(mobileContract).Property("ContractType").CurrentValue);
            Assert.Equal("Broadband", context.Entry(broadbandContract).Property("ContractType").CurrentValue);
            Assert.Equal("Tv", context.Entry(tvContract).Property("ContractType").CurrentValue);
        }
    }
}
