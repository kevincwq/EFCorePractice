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
    public class IncludeTests : TestBase
    {
        public IncludeTests(ITestOutputHelper output, DbContextFixture fixture)
            : base(output, fixture)
        {
        }

        [Fact]
        public async Task Include_All()
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
                    new Book { Title = "Adventures 2", Isbn = "ISBN:" + DbContextFixture.NewUniqueIndex, Publisher = publisher }
                },
                Address = new Address { Street = "Cromwell Rd", City = "South Kensington", StateOrProvince = "London SW7 5BD", Country = "United Kingdom" },
                Biography = new AuthorBiography { Biography = "Something cool", DateOfBirth = new DateTime(1920, 1, 2), PlaceOfBirth = "Unknown", Nationality = "Q" }
            };

            context.Add(author);
            await context.SaveChangesAsync();

            // Act
            var query = context.Authors.AsNoTracking().Where(a => a.Id == author.Id).Include(a => a.Books).ThenInclude(b => b.Publisher).ThenInclude(p => p.Address).Include(a => a.Address).Include(a => a.Biography);
            output.WriteLine($"SQL: {query.ToQueryString()}");
            var saved = query.Single();

            // Assert
            Assert.Equal(author.FirstName, saved.FirstName);
            Assert.Equal(author.Books.Count(), saved.Books.Count());
            Assert.Equal(author.Address.City, saved.Address.City);
            Assert.Equal(author.Biography.DateOfBirth, saved.Biography.DateOfBirth);
            Assert.True(saved.Books.All(b => b.Publisher.Name == publisher.Name));
        }


        [Fact]
        public async Task Include_NonEntityType()
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
                    new Book { Title = "Adventures 2", Isbn = "ISBN:" + DbContextFixture.NewUniqueIndex, Publisher = publisher }
                },
                Address = new Address { Street = "Cromwell Rd", City = "South Kensington", StateOrProvince = "London SW7 5BD", Country = "United Kingdom" },
                Biography = new AuthorBiography { Biography = "Something cool", DateOfBirth = new DateTime(1920, 1, 2), PlaceOfBirth = "Unknown", Nationality = "Q" }
            };

            context.Add(author);
            await context.SaveChangesAsync();

            // Act
            var query = context.Authors.Where(a => a.FirstName == "William").Include(a => a.Books).ThenInclude(b => b.Publisher).ThenInclude(p => p.Address).Include(a => a.Address).Include(a => a.Biography).Select(a => new { a.FirstName, Count = a.Books.Count(), a.Address.City, a.Biography.DateOfBirth, PubName = a.Books.First().Publisher.Name });
            output.WriteLine($"SQL: {query.ToQueryString()}");
            var saved = query.Single();

            // Assert
            Assert.Equal(author.FirstName, saved.FirstName);
            Assert.Equal(author.Books.Count(), saved.Count);
            Assert.Equal(author.Address.City, saved.City);
            Assert.Equal(author.Biography.DateOfBirth, saved.DateOfBirth);
            Assert.Equal(publisher.Name, saved.PubName);
        }
    }
}
