using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EFCorePractice.Tests
{
    public class CRUDTests : IClassFixture<DbContextFixture>
    {
        DbContextFixture dbFixture;

        public CRUDTests(DbContextFixture fixture)
        {
            this.dbFixture = fixture;
        }

        #region Create

        [Fact]
        public async Task Add_SimpleRecord()
        {
            // Arrange
            var context = await dbFixture.CreateContextAsync();

            // Act
            // with type parameter
            var author1 = new Author { FirstName = "William", LastName = "Shakespeare" };
            context.Add<Author>(author1);
            await context.SaveChangesAsync();

            // without type parameter
            var author2 = new Author { FirstName = "William", LastName = "Shakespeare" };
            context.Add(author2);
            await context.SaveChangesAsync();

            // Assert
            Assert.Equal(2, context.Authors.Count());

            // Clear data
            context.RemoveRange(author1, author2);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task Add_RelationalRecords_1()
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
            await context.SaveChangesAsync();

            // Assert
            Assert.Equal(1, context.Authors.Count());
            Assert.Equal(3, context.Books.Count());

            // Clear data
            context.Remove(author);
            await context.SaveChangesAsync();
            Assert.Equal(0, context.Authors.Count());
            Assert.Equal(0, context.Books.Count());
        }

        [Fact]
        public async Task Add_RelationalRecords_2()
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
            await context.SaveChangesAsync();

            // Assert
            Assert.Equal(1, context.Authors.Count());
            Assert.Equal(3, context.Books.Count());

            // Clear data
            context.Remove(author);
            await context.SaveChangesAsync();
            Assert.Equal(0, context.Authors.Count());
            Assert.Equal(0, context.Books.Count());
        }

        [Fact]
        public async Task Add_MultipleRecords()
        {
            // Arrange
            var context = await dbFixture.CreateContextAsync();

            // Act
            var author = new Author { FirstName = "William", LastName = "Shakespeare" };
            var book = new Book { Title = "Adventures of Huckleberry Finn", Author = author };
            context.AddRange(author, book);
            await context.SaveChangesAsync();

            // Assert
            Assert.Equal(1, context.Authors.Count());
            Assert.Equal(1, context.Books.Count());

            // Clear data
            context.RemoveRange(author, book);
            await context.SaveChangesAsync();
            Assert.Equal(0, context.Authors.Count());
            Assert.Equal(0, context.Books.Count());
        }

        #endregion

        #region Update

        [Fact]
        public async Task Update_TrackedRecord()
        {
            // Arrange
            var context = await dbFixture.CreateContextAsync();
            var author = new Author { FirstName = "William", LastName = "Shakespeare" };
            context.Add(author);
            await context.SaveChangesAsync();

            // Act
            author.FirstName = "Bill";
            await context.SaveChangesAsync();

            // Assert
            context.ChangeTracker.Clear();
            var saved = await context.Authors.FindAsync(author.Id);
            Assert.Equal("Bill", saved.FirstName);

            // Clear data
            context.Remove(saved);
            await context.SaveChangesAsync();
            Assert.Equal(0, context.Authors.Count());
            Assert.Equal(0, context.Books.Count());
        }

        [Fact]
        public async Task Update_SettingEntityState()
        {
            // Arrange
            var context = await dbFixture.CreateContextAsync();
            var author = new Author { FirstName = "William", LastName = "Shakespeare" };
            context.Add(author);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            // Act
            var updatedAuthor = new Author { Id = author.Id, FirstName = "Bill", LastName = "Shake" };
            context.Entry(updatedAuthor).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await context.SaveChangesAsync();

            // Assert
            context.ChangeTracker.Clear();
            var saved = await context.Authors.FindAsync(author.Id);
            Assert.Equal("Bill", saved.FirstName);
            Assert.Equal("Shake", saved.LastName);

            // Clear data
            context.Remove(saved);
            await context.SaveChangesAsync();
            Assert.Equal(0, context.Authors.Count());
            Assert.Equal(0, context.Books.Count());
        }

        [Fact]
        public async Task Update_DbContextUpdate()
        {
            // Arrange
            var context = await dbFixture.CreateContextAsync();
            var author = new Author { FirstName = "William", LastName = "Shakespeare" };
            context.Add(author);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            // Act
            var updatedAuthor = new Author { Id = author.Id, FirstName = "Bill", LastName = "Shake" };
            // update or insert
            context.Update(updatedAuthor);
            await context.SaveChangesAsync();

            // Assert
            context.ChangeTracker.Clear();
            var saved = await context.Authors.FindAsync(author.Id);
            Assert.Equal("Bill", saved.FirstName);
            Assert.Equal("Shake", saved.LastName);

            // Clear data
            context.Remove(saved);
            await context.SaveChangesAsync();
            Assert.Equal(0, context.Authors.Count());
            Assert.Equal(0, context.Books.Count());
        }


        [Fact]
        public async Task Update_AttachAndPatch_1()
        {
            // Arrange
            var context = await dbFixture.CreateContextAsync();
            var author = new Author { FirstName = "William", LastName = "Shakespeare" };
            context.Add(author);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            // Act
            var updatedAuthor = new Author { Id = author.Id, FirstName = "Bill", LastName = "Shake" };
            updatedAuthor.Books.Add(new Book { Id = 1, Title = "Othello" });

            // just update FirstName
            context.Attach(updatedAuthor);
            context.Entry(updatedAuthor).Property("FirstName").IsModified = true;
            updatedAuthor.LastName = "Shakespeare1";
            await context.SaveChangesAsync();

            // Assert
            context.ChangeTracker.Clear();
            var saved = await context.Authors.Include(a => a.Books).FirstAsync(a => a.Id == author.Id);
            Assert.Equal("Bill", saved.FirstName);
            Assert.Equal("Shakespeare1", saved.LastName);

            // Clear data
            context.Remove(saved);
            await context.SaveChangesAsync();
            Assert.Equal(0, context.Authors.Count());
            Assert.Equal(0, context.Books.Count());
        }

        [Fact]
        public async Task Update_TrackGraph_1()
        {
            // Arrange
            var context = await dbFixture.CreateContextAsync();
            var author = new Author { FirstName = "William", LastName = "Shakespeare" };
            context.Add(author);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            // Act
            var updatedAuthor = new Author
            {
                Id = author.Id,
                FirstName = "William",
                LastName = "Shakespeare"
            };
            updatedAuthor.Books.Add(new Book { AuthorId = updatedAuthor.Id, Title = "Hamlet", Isbn = "1234" });
            updatedAuthor.Books.Add(new Book { AuthorId = updatedAuthor.Id, Title = "Othello", Isbn = "4321" });
            updatedAuthor.Books.Add(new Book { AuthorId = updatedAuthor.Id, Title = "MacBeth", Isbn = "5678" });

            // assumed that the author entity has not been changed, but the books might have been edited.
            context.ChangeTracker.TrackGraph(updatedAuthor, e => {
                if ((e.Entry.Entity as Author) != null)
                {
                    e.Entry.State = EntityState.Unchanged;
                }
                else
                {
                    e.Entry.State = EntityState.Added;
                }
            });
            await context.SaveChangesAsync();

            // Assert
            context.ChangeTracker.Clear();
            var saved = await context.Authors.Include(a => a.Books).FirstAsync(a => a.Id == author.Id);
            Assert.Equal("William", saved.FirstName);
            Assert.Equal("Shakespeare", saved.LastName);
            Assert.Equal(updatedAuthor.Books.Count(), saved.Books.Count());

            // Clear data
            context.Remove(saved);
            await context.SaveChangesAsync();
            Assert.Equal(0, context.Authors.Count());
            Assert.Equal(0, context.Books.Count());
        }

        [Fact]
        public async Task Update_TrackGraph_2()
        {
            // Arrange
            var context = await dbFixture.CreateContextAsync();
            var author = new Author { FirstName = "William", LastName = "Shakespeare", Books = new List<Book> { 
                new Book{ Title = "Hamlet" },
                new Book{ Title = "Othello" },
                new Book{ Title = "MacBeth" }
            } };
            context.Add(author);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            // Act
            var updatedAuthor = new Author
            {
                Id = author.Id,
                FirstName = "William",
                LastName = "Shakespeare"
            };
            updatedAuthor.Books.Add(new Book { Id = author.Books[0].Id, Isbn = "1234" });
            updatedAuthor.Books.Add(new Book { Id = author.Books[1].Id, Isbn = "4321" });
            updatedAuthor.Books.Add(new Book { Id = author.Books[2].Id, Isbn = "5678" });

            // ensures that all entities are tracked in the UnChanged state, and then indicates that the Isbn property is modified. 
            context.ChangeTracker.TrackGraph(updatedAuthor, e => {
                e.Entry.State = EntityState.Unchanged; //starts tracking
                if ((e.Entry.Entity as Book) != null)
                {
                    context.Entry(e.Entry.Entity as Book).Property("Isbn").IsModified = true;
                }
            });
            await context.SaveChangesAsync();

            // Assert
            context.ChangeTracker.Clear();
            var saved = await context.Authors.Include(a => a.Books).FirstAsync(a => a.Id == author.Id);
            Assert.Equal("William", saved.FirstName);
            Assert.Equal("Shakespeare", saved.LastName);
            Assert.Equal(updatedAuthor.Books.Count(), saved.Books.Count());
            Assert.True(saved.Books.All(b => !string.IsNullOrWhiteSpace(b.Isbn)));

            // Clear data
            context.Remove(saved);
            await context.SaveChangesAsync();
            Assert.Equal(0, context.Authors.Count());
            Assert.Equal(0, context.Books.Count());
        }

        #endregion

        #region Delete

        #endregion
    }
}
