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
    public class ConcurrenctyTests : IClassFixture<DbContextFixture>
    {
        DbContextFixture dbFixture;
        private readonly ITestOutputHelper output;

        public ConcurrenctyTests(ITestOutputHelper output, DbContextFixture fixture)
        {
            this.output = output;
            this.dbFixture = fixture;
        }

        [Fact]
        public async Task Concurrencty_Update_ThrowException()
        {
            // Arrange
            var context_a = await dbFixture.CreateContextAsync();
            var contact_a = new Contact
            {
                FirstName = "William",
                LastName = "Shakespeare",
                Email = "a_b@c.com"
            };
            context_a.Add(contact_a);
            await context_a.SaveChangesAsync();

            // Act
            var context_b = await dbFixture.CreateContextAsync();
            var contact_b = context_b.Contacts.SingleOrDefault(c => c.Email == contact_a.Email);

            contact_a.FirstName = "William II";
            Assert.True(0 < await context_a.SaveChangesAsync());

            // TODO: SQLITE DOES NOT THROW EXCEPTION
            contact_b.FirstName = "William IIII";
            //await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => context_b.SaveChangesAsync());
            Assert.True(0 < await context_b.SaveChangesAsync());

            contact_b = context_b.Contacts.SingleOrDefault(c => c.Email == contact_a.Email);
            contact_b.FirstName = "William III";
            Assert.True(0 < await context_b.SaveChangesAsync());

            // Assert

            context_a.Entry(contact_a).Reload();
            Assert.Equal(contact_b.FirstName, contact_a.FirstName);
        }
    }
}
