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
    public class GroupByTests : TestBase
    {
        public GroupByTests(ITestOutputHelper output, DbContextFixture fixture)
            : base(output, fixture)
        {
        }

        [Fact]
        public async Task GroupBy_SingleColumn()
        {
            // Arrange
            var context = await dbFixture.CreateContextAsync();
            var publisher = new Publisher { Name = "ABC Press" + DbContextFixture.NewUniqueIndex };
            var authors = Enumerable.Range(1, 20).Select(i => new Author
            {
                FirstName = "William" + i % 5,
                LastName = "Shakespeare",
                Books = new[] {
                    new Book { Title = "Adventures 1", Isbn = "ISBN:" + DbContextFixture.NewUniqueIndex, Publisher = publisher },
                    new Book { Title = "Adventures 2", Isbn = "ISBN:" + DbContextFixture.NewUniqueIndex, Publisher = publisher }
                }
            }).ToArray();

            context.Authors.AddRange(authors);
            await context.SaveChangesAsync();

            // Act
            var groups = context.Authors.GroupBy(a => a.FirstName).Select(g => new { Name = g.Key, Count = g.Count() });
            output.WriteLine($"SQL: {groups.ToQueryString()}");

            // Assert
            Assert.Equal(5, groups.Count());
        }

        [Fact]
        public async Task GroupBy_MultipleColumn()
        {
            // Arrange
            var context = await dbFixture.CreateContextAsync();
            var publisher = new Publisher { Name = "ABC Press" + DbContextFixture.NewUniqueIndex };
            var authors = Enumerable.Range(1, 20).Select(i => new Author
            {
                FirstName = "William" + i % 5,
                LastName = "Shakespeare",
                Books = new[] {
                    new Book { Title = "Adventures 1", Isbn = "ISBN:" + DbContextFixture.NewUniqueIndex, Publisher = publisher },
                    new Book { Title = "Adventures 2", Isbn = "ISBN:" + DbContextFixture.NewUniqueIndex, Publisher = publisher }
                }
            }).ToArray();

            context.Authors.AddRange(authors);
            await context.SaveChangesAsync();

            // Act
            var groups = context.Authors.GroupBy(a => new { a.FirstName, a.LastName }).Select(g => new { Name = g.Key.FirstName + " " + g.Key.LastName, Count = g.Count() });
            output.WriteLine($"SQL: {groups.ToQueryString()}");

            // Assert
            Assert.Equal(5, groups.Count());
        }
    }

}
