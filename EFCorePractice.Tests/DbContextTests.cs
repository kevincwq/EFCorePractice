using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace EFCorePractice.Tests
{
    public class DbContextTests : IClassFixture<DbContextFixture>
    {
        DbContextFixture dbFixture;

        public DbContextTests(DbContextFixture fixture)
        {
            this.dbFixture = fixture;
        }

        [Fact]
        public async void Add_SimpleRecord()
        {
            // Arrange
            var context = await dbFixture.CreateContextAsync();

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

            // Clear data
            context.RemoveRange(author1, author2);
            context.SaveChanges();
        }

        [Fact]
        public async void Add_RelationalRecords_1()
        {
            // Arrange
            var context = await dbFixture.CreateContextAsync();

            // Act
            var author = new Author
            {
                FirstName = "William",
                LastName = "Shakespeare",
                Books = new List<Book>
                {
                    new Book { Title = "Hamlet"},
                    new Book { Title = "Othello" },
                    new Book { Title = "MacBeth" }
                }
            };
            context.Add(author);
            context.SaveChanges();

            // Assert
            Assert.Equal(1, context.Authors.Count());
            Assert.Equal(3, context.Books.Count());

            // Clear data
            context.Remove(author);
            context.SaveChanges();
            Assert.Equal(0, context.Authors.Count());
            Assert.Equal(0, context.Books.Count());
        }

        [Fact]
        public async void Add_RelationalRecords_2()
        {
            // Arrange
            var context = await dbFixture.CreateContextAsync();

            // Act
            var author = new Author { FirstName = "Stephen", LastName = "King" };
            var books = new List<Book> {
                new Book { Title = "It", Author = author },
                new Book { Title = "Carrie", Author = author },
                new Book { Title = "Misery", Author = author }
            };
            context.AddRange(books);
            context.SaveChanges();

            // Assert
            Assert.Equal(1, context.Authors.Count());
            Assert.Equal(3, context.Books.Count());

            // Clear data
            context.Remove(author);
            context.SaveChanges();
            Assert.Equal(0, context.Authors.Count());
            Assert.Equal(0, context.Books.Count());
        }

        [Fact]
        public async void Add_MultipleRecords()
        {
            // Arrange
            var context = await dbFixture.CreateContextAsync();

            // Act
            var author = new Author { FirstName = "William", LastName = "Shakespeare" };
            var book = new Book { Title = "Adventures of Huckleberry Finn", Author = author };
            context.AddRange(author, book);
            context.SaveChanges();

            // Assert
            Assert.Equal(1, context.Authors.Count());
            Assert.Equal(1, context.Books.Count());

            // Clear data
            context.RemoveRange(author, book);
            context.SaveChanges();
            Assert.Equal(0, context.Authors.Count());
            Assert.Equal(0, context.Books.Count());
        }
    }
}
