using System;
using System.Linq;
using Xunit;

namespace EFCorePractice.Tests
{
    public class DbContextTests : TestBase
    {
        [Fact]
        public async void AddingData()
        {
            // Arrange
            var context = await CreateSQLiteDbContextAsync();

            // Act
            // with type parameter
            var author1 = new Author { FirstName = "William", LastName = "Shakespeare" };
            context.Add<Author>(author1);
            context.SaveChanges();

            // without type parameter
            var author2 = new Author { FirstName = "William", LastName = "Shakespeare" };
            context.Add(author2);
            context.SaveChanges();

            // Assert
            Assert.Equal(2, context.Authors.Count());
        }
    }
}
