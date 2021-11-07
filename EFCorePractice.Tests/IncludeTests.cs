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
    public class IncludeTests : IClassFixture<DbContextFixture>
    {
        DbContextFixture dbFixture;
        private readonly ITestOutputHelper output;

        public IncludeTests(ITestOutputHelper output, DbContextFixture fixture)
        {
            this.output = output;
            this.dbFixture = fixture;
        }

        [Fact]
        public async Task Include_All()
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
            var query = context.Authors.Where(a => a.FirstName == "William").Include(a => a.Books).ThenInclude(b => b.Publisher).ThenInclude(p => p.Address).Include(a => a.Address).Include(a => a.Biography);
            output.WriteLine($"SQL: {query.ToQueryString()}");
            var saved = query.Single();

            // Assert
            Assert.Equal(author.FirstName, saved.FirstName);
            Assert.Equal(author.Books.Count(), saved.Books.Count());
            Assert.Equal(author.Address.City, saved.Address.City);
            Assert.Equal(author.Biography.DateOfBirth, saved.Biography.DateOfBirth);
            Assert.True(saved.Books.All(b => b.Publisher.Name == publisher.Name));
        }
    }
}
