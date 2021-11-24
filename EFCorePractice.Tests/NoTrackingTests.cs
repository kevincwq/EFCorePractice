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
    public class NoTrackingTests : TestBase
    {
        public NoTrackingTests(ITestOutputHelper output, DbContextFixture fixture)
            : base(output, fixture)
        {
        }

        [Fact]
        public async Task NoTracking_SingleQuery()
        {
            // Arrange
            var context = await dbFixture.CreateContextAsync();
            var publisher = new Publisher { Name = "ABC Press" + DbContextFixture.NewUniqueIndex, Address = new Address { Street = "12 Yonghegong St", City = "Dongcheng", StateOrProvince = "Beijing", Country = "China", } };
            var author = new Author
            {
                FirstName = "William",
                LastName = "Shakespeare",
                Books = new[] {
                    new Book { Title = "Adventures 1", Isbn = "ISBN:" + DbContextFixture.NewUniqueIndex, Publisher = publisher },
                    new Book { Title = "Adventures 2", Isbn = "5678", Publisher = publisher }
                },
                Address = new Address { Street = "Cromwell Rd", City = "South Kensington", StateOrProvince = "London SW7 5BD", Country = "United Kingdom" },
                Biography = new AuthorBiography { Biography = "Something cool", DateOfBirth = new DateTime(1920, 1, 2), PlaceOfBirth = "Unknown", Nationality = "Q" }
            };

            context.Add(author);
            await context.SaveChangesAsync();

            // Act
            var query = context.Authors.Where(a => a.FirstName == "William").AsNoTracking();
            output.WriteLine($"SQL: {query.ToQueryString()}");
            var saved = query.Single();

            // Assert
            Assert.Equal(author.FirstName, saved.FirstName);
            Assert.Equal(EntityState.Detached, context.Entry(saved).State);
        }


        [Fact]
        public async Task NoTracking_AllQuery()
        {
            // Arrange
            var context = await dbFixture.CreateContextAsync();
            var publisher = new Publisher { Name = "ABC Press" + DbContextFixture.NewUniqueIndex, Address = new Address { Street = "12 Yonghegong St", City = "Dongcheng", StateOrProvince = "Beijing", Country = "China", } };
            var author = new Author
            {
                FirstName = "William" + DbContextFixture.NewUniqueIndex,
                LastName = "Shakespeare" + DbContextFixture.NewUniqueIndex,
                Books = new[] {
                    new Book { Title = "Adventures 1", Isbn = "ISBN:" + DbContextFixture.NewUniqueIndex, Publisher = publisher },
                    new Book { Title = "Adventures 2", Isbn = "ISBN:" + DbContextFixture.NewUniqueIndex, Publisher = publisher }
                },
                Address = new Address { Street = "Cromwell Rd", City = "South Kensington", StateOrProvince = "London SW7 5BD", Country = "United Kingdom" },
                Biography = new AuthorBiography { Biography = "Something cool", DateOfBirth = new DateTime(1920, 1, 2), PlaceOfBirth = "Unknown", Nationality = "Q" }
            };

            context.Add(author);
            await context.SaveChangesAsync();

            // Act
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            var saved = context.Authors.Where(a => a.FirstName == author.FirstName).Single();
            saved = context.Authors.Where(a => a.LastName == author.LastName).Single();

            // Assert
            Assert.Equal(author.FirstName, saved.FirstName);
            Assert.Equal(EntityState.Detached, context.Entry(saved).State);
        }
    }
}
