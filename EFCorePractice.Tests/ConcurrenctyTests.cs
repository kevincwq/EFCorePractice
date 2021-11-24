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
    public class ConcurrenctyTests : TestBase
    {
        public ConcurrenctyTests(ITestOutputHelper output, DbContextFixture fixture)
            :base(output, fixture)
        {
        }

        [Fact]
        public async Task Concurrencty_Update_ThrowException()
        {
            // Arrange
            var context_a = await dbFixture.CreateContextAsync();
            var context_b = await dbFixture.CreateContextAsync();
            var contact_a = new Contact
            {
                FirstName = "William",
                LastName = "Shakespeare",
                Email = "a_b@c.com"
            };
            context_a.Add(contact_a);
            await context_a.SaveChangesAsync();

            // Act
            var contact_b = context_b.Contacts.SingleOrDefault(c => c.Email == contact_a.Email);

            contact_a.FirstName = "William II";
            Assert.True(0 < await context_a.SaveChangesAsync());

            // TODO: SQLITE DOES NOT THROW EXCEPTION
            contact_b.FirstName = "William IIII";
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => context_b.SaveChangesAsync());
            // Assert.True(0 < await context_b.SaveChangesAsync());

            context_b.Entry(contact_b).Reload();
            contact_b.FirstName = "William III";
            Assert.True(0 < await context_b.SaveChangesAsync());

            // Assert
            context_a.Entry(contact_a).Reload();
            Assert.Equal(contact_b.FirstName, contact_a.FirstName);
        }
    }
}
