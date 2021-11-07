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
    public class ShadowPropertyTests : IClassFixture<DbContextFixture>
    {
        DbContextFixture dbFixture;
        private readonly ITestOutputHelper output;

        public ShadowPropertyTests(ITestOutputHelper output, DbContextFixture fixture)
        {
            this.output = output;
            this.dbFixture = fixture;
        }

        [Fact]
        public async Task ShadowProperty_Query()
        {
            // Arrange
            var context = await dbFixture.CreateContextAsync();
            var contact = new Contact
            {
                FirstName = "William",
                LastName = "Shakespeare",
                Email = "a_b@c.com"
            };
            var utc = DateTime.UtcNow;

            context.Add(contact);
            await context.SaveChangesAsync();

            // set shadow property value
            context.Entry(contact).Property("LastUpdated").CurrentValue = utc;
            await context.SaveChangesAsync();

            // Act
            var query = context.Contacts.Where(c => EF.Property<DateTime>(c, "LastUpdated") == utc);
            output.WriteLine($"SQL: {query.ToQueryString()}");
            var saved = query.Single();

            // Assert
            Assert.Equal(utc, context.Entry(saved).Property("LastUpdated").CurrentValue);
            Assert.Equal(contact.FirstName, saved.FirstName);
        }
    }
}
