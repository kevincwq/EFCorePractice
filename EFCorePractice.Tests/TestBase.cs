using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace EFCorePractice.Tests
{
    [Collection(nameof(DbContextCollection))]
    public abstract class TestBase : IClassFixture<DbContextFixture>
    {
        protected readonly DbContextFixture dbFixture;
        protected readonly ITestOutputHelper output;

        public TestBase(ITestOutputHelper output, DbContextFixture fixture)
        {
            this.output = output;
            this.dbFixture = fixture;
        }
    }
}
