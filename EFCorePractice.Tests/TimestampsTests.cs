using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace EFCorePractice.Tests
{
    public class TimestampsTests : TestBase
    {
        public TimestampsTests(ITestOutputHelper output, DbContextFixture fixture)
            : base(output, fixture)
        {
        }

        [Fact]
        public async Task CreatedUtc_UpdatedUtc_HaveCorrectValues()
        {
            // Arrange
            var utc = DateTime.UtcNow;
            var context = await dbFixture.CreateContextAsync();
            var author = new Author { FirstName = "William", LastName = "Shakespeare" };
            context.Add(author);
            await context.SaveChangesAsync();

            // Act
            var createdUtc = author.CreatedUtc;
            var updatedUtc = author.UpdatedUtc;
            author.FirstName = "Bill";
            await context.SaveChangesAsync();

            // Assert
            var saved = await context.Authors.AsNoTracking().SingleAsync(a => a.Id == author.Id);
            Assert.True(utc < createdUtc);
            Assert.True(utc < updatedUtc);
            Assert.Equal(createdUtc, saved.CreatedUtc);
            Assert.True(updatedUtc < saved.UpdatedUtc);
        }
    }
}
