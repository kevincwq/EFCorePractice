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
    public class NoTrackingTests : IClassFixture<DbContextFixture>
    {
        DbContextFixture dbFixture;
        private readonly ITestOutputHelper output;

        public NoTrackingTests(ITestOutputHelper output, DbContextFixture fixture)
        {
            this.output = output;
            this.dbFixture = fixture;
        }

        [Fact]
        public async Task NoTracking_SingleQuery()
        {
            // Arrange
            var context = await dbFixture.CreateContextAsync();
            var publisher = new Publisher { Name = "ABC Press", Address = new Address { City = "Beijing" } };
            var author = new Author
            {
                FirstName = "William",
                LastName = "Shakespeare",
                Books = new[] {
                    new Book { Title = "Adventures 1", Isbn = "1234", Publisher = publisher },
                    new Book { Title = "Adventures 2", Isbn = "5678", Publisher = publisher }
                },
                Address = new Address { City = "London" },
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
            var publisher = new Publisher { Name = "ABC Press", Address = new Address { City = "Beijing" } };
            var author = new Author
            {
                FirstName = "William",
                LastName = "Shakespeare",
                Books = new[] {
                    new Book { Title = "Adventures 1", Isbn = "1234", Publisher = publisher },
                    new Book { Title = "Adventures 2", Isbn = "5678", Publisher = publisher }
                },
                Address = new Address { City = "London" },
                Biography = new AuthorBiography { Biography = "Something cool", DateOfBirth = new DateTime(1920, 1, 2), PlaceOfBirth = "Unknown", Nationality = "Q" }
            };

            context.Add(author);
            await context.SaveChangesAsync();

            // Act
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            var saved = context.Authors.Where(a => a.FirstName == "William").Single();
            saved = context.Authors.Where(a => a.LastName == "Shakespeare").Single();

            // Assert
            Assert.Equal(author.FirstName, saved.FirstName);
            Assert.Equal(EntityState.Detached, context.Entry(saved).State);
        }
    }
}
