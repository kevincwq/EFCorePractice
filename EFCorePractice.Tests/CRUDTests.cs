using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace EFCorePractice.Tests
{
    public class CRUDTests : TestBase
    {
        public CRUDTests(ITestOutputHelper output, DbContextFixture fixture)
            : base(output, fixture)
        {
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
            Assert.Equal(2, context.Authors.AsNoTracking().Count(a => new[] { author1.Id, author2.Id }.Contains(a.Id)));
        }

        [Fact]
        public async Task Add_SimpleRecord_WithKey()
        {
            // Arrange
            var context = await dbFixture.CreateContextAsync();

            // Act
            var author1 = new Author { Id = 5000, FirstName = "William", LastName = "Shakespeare" };
            context.Add<Author>(author1);

            var author2 = new Author { Id = 5001, FirstName = "William", LastName = "Shakespeare" };
            context.Add(author2);

            await context.SaveChangesWithIdentityInsertAsync<Author>();

            // Assert
            var authors = context.Authors.AsNoTracking().Where(a => new[] { author1.Id, author2.Id }.Contains(a.Id)).ToArray();
            Assert.Equal(2, authors.Length);
            Assert.True(authors.All(a => a.Id >= 5000));
        }

        [Fact]
        public async Task Add_Relationship_OneToMany()
        {
            // Arrange
            var context = await dbFixture.CreateContextAsync();

            // Act
            var publisher = new Publisher { Name = "ABC Press" + DbContextFixture.NewUniqueIndex };
            var author = new Author
            {
                FirstName = "William",
                LastName = "Shakespeare",
                Books = new List<Book>
                {
                    new Book { Title = "Hamlet", Isbn = "ISBN:" + DbContextFixture.NewUniqueIndex, Publisher = publisher },
                    new Book { Title = "Othello", Isbn = "ISBN:" + DbContextFixture.NewUniqueIndex, Publisher = publisher},
                    new Book { Title = "MacBeth", Isbn = "ISBN:" + DbContextFixture.NewUniqueIndex, Publisher = publisher }
                }
            };
            // Tells EF that the entity is new and should be inserted into the database, and so sets the state to Added.
            // It also does this for all reachable entities in the graph that it isn't already tracking.
            context.Add(author);
            await context.SaveChangesAsync();

            // Assert
            Assert.Equal(1, context.Authors.AsNoTracking().Count(a => a.Id == author.Id));
            Assert.Equal(1, context.Publishers.AsNoTracking().Count(a => a.Id == publisher.Id));
            Assert.Equal(3, context.Books.AsNoTracking().Count(a => author.Books.Select(b => b.Id).Contains(a.Id)));
        }

        [Fact]
        public async Task Add_Relationship_ManyToOne()
        {
            // Arrange
            var context = await dbFixture.CreateContextAsync();

            // Act
            var publisher = new Publisher { Name = "ABC Press" + DbContextFixture.NewUniqueIndex };
            var author = new Author { FirstName = "Stephen", LastName = "King" };
            var books = new List<Book> {
                new Book { Title = "It", Isbn = "ISBN:" + DbContextFixture.NewUniqueIndex, Author = author, Publisher = publisher },
                new Book { Title = "Carrie", Isbn = "ISBN:" + DbContextFixture.NewUniqueIndex, Author = author, Publisher = publisher },
                new Book { Title = "Misery", Isbn = "ISBN:" + DbContextFixture.NewUniqueIndex, Author = author, Publisher = publisher }
            };
            context.AddRange(books);
            await context.SaveChangesAsync();

            // Assert
            Assert.Equal(1, context.Authors.AsNoTracking().Count(a => a.Id == author.Id));
            Assert.Equal(1, context.Publishers.AsNoTracking().Count(a => a.Id == publisher.Id));
            Assert.Equal(3, context.Books.AsNoTracking().Count(a => books.Select(b => b.Id).Contains(a.Id)));
        }

        [Fact]
        public async Task Add_MultipleRecords()
        {
            // Arrange
            var context = await dbFixture.CreateContextAsync();

            // Act
            var author = new Author { FirstName = "William", LastName = "Shakespeare" };
            var publisher = new Publisher { Name = "ABC Press" + DbContextFixture.NewUniqueIndex };
            var book = new Book { Title = "Adventures of Huckleberry Finn", Isbn = "ISBN:" + DbContextFixture.NewUniqueIndex, Author = author, Publisher = publisher };
            context.AddRange(author, publisher, book);
            await context.SaveChangesAsync();

            // Assert
            Assert.NotNull(context.Authors.AsNoTracking().Single(a => a.Id == author.Id));
            Assert.NotNull(context.Books.AsNoTracking().Single(a => a.Id == book.Id));
            Assert.NotNull(context.Publishers.AsNoTracking().Single(a => a.Id == publisher.Id));
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
            var saved = await context.Authors.AsNoTracking().SingleAsync(a => a.Id == author.Id);
            Assert.Equal("Bill", saved.FirstName);
        }

        [Fact]
        public async Task Update_SetEntityState()
        {
            // Arrange
            var context = await dbFixture.CreateContextAsync();
            var author = new Author { FirstName = "William", LastName = "Shakespeare" };
            context.Add(author);
            await context.SaveChangesAsync();
            context.Entry(author).State = EntityState.Detached;

            // Act
            var updatedAuthor = new Author { Id = author.Id, FirstName = "Bill", LastName = "Shake", RowVersion = author.RowVersion };
            context.Entry(updatedAuthor).State = EntityState.Modified;
            await context.SaveChangesAsync();

            // Assert
            var saved = await context.Authors.AsNoTracking().SingleAsync(a => a.Id == author.Id);
            Assert.Equal("Bill", saved.FirstName);
            Assert.Equal("Shake", saved.LastName);
        }

        [Fact]
        public async Task Update_ContextUpdate()
        {
            // Arrange
            var context = await dbFixture.CreateContextAsync();
            var publisher = new Publisher { Name = "ABC Press" + DbContextFixture.NewUniqueIndex };
            var author = new Author
            {
                FirstName = "William",
                LastName = "Shakespeare",
                Books = new[] {
                    new Book { Title = "Othello", Isbn = "ISBN:" + DbContextFixture.NewUniqueIndex, Publisher = publisher }
                }
            };
            context.Add(author);
            await context.SaveChangesAsync();
            context.Entry(author).State = EntityState.Detached;

            // Act
            var updatedAuthor = new Author
            {
                Id = author.Id,
                FirstName = "Bill",
                LastName = "Shake",
                Books = new[] {
                    new Book { Title = "Hamlet", Isbn = "ISBN:" + DbContextFixture.NewUniqueIndex, Publisher = publisher }
                },
                RowVersion = author.RowVersion
            };
            // update or insert, new Book is inserted too
            context.Update(updatedAuthor);
            await context.SaveChangesAsync();
            context.Entry(updatedAuthor).State = EntityState.Detached;

            // Assert
            var saved = context.Authors.Include(a => a.Books).Where(a => a.Id == author.Id).Single();
            Assert.Equal("Bill", saved.FirstName);
            Assert.Equal("Shake", saved.LastName);
            Assert.Equal(2, saved.Books.Count);
        }


        [Fact]
        public async Task Update_AttachAndPatch()
        {
            // Arrange
            var context = await dbFixture.CreateContextAsync();
            var author = new Author { FirstName = "William", LastName = "Shakespeare" };
            context.Add(author);
            await context.SaveChangesAsync();
            context.Entry(author).State = EntityState.Detached;

            // Act
            var updatedAuthor = author;
            updatedAuthor.FirstName = "Bill";
            updatedAuthor.Books.Add(new Book { Title = "Hamlet", Isbn = "ISBN:" + DbContextFixture.NewUniqueIndex, Publisher = new Publisher { Name = "ABC Press" + DbContextFixture.NewUniqueIndex } });
            // start tracking
            context.Attach(updatedAuthor);
            // just update FirstName
            context.Entry(updatedAuthor).Property("FirstName").IsModified = true;
            // just update LastName
            updatedAuthor.LastName = "Shakespeare1";
            await context.SaveChangesAsync();

            // Assert
            var saved = await context.Authors.AsNoTracking().Include(a => a.Books).FirstAsync(a => a.Id == author.Id);
            Assert.Equal("Bill", saved.FirstName);
            Assert.Equal("Shakespeare1", saved.LastName);
        }

        [Fact]
        public async Task Update_TrackGraph_1()
        {
            // Arrange
            var context = await dbFixture.CreateContextAsync();
            var author = new Author { FirstName = "William", LastName = "Shakespeare" };
            context.Add(author);
            await context.SaveChangesAsync();
            context.Entry(author).State = EntityState.Detached;

            // Act
            var updatedAuthor = new Author
            {
                Id = author.Id,
                FirstName = "William",
                LastName = "Shakespeare"
            };
            var publisher = new Publisher { Name = "ABC Press" + DbContextFixture.NewUniqueIndex };
            updatedAuthor.Books.Add(new Book { AuthorId = updatedAuthor.Id, Title = "Hamlet", Isbn = "ISBN:" + DbContextFixture.NewUniqueIndex, Publisher = publisher });
            updatedAuthor.Books.Add(new Book { AuthorId = updatedAuthor.Id, Title = "Othello", Isbn = "4321", Publisher = publisher });
            updatedAuthor.Books.Add(new Book { AuthorId = updatedAuthor.Id, Title = "MacBeth", Isbn = "5678", Publisher = publisher });

            // assumed that the author entity has not been changed, but the books might have been edited.
            context.ChangeTracker.TrackGraph(updatedAuthor, e =>
            {
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
            var saved = await context.Authors.AsNoTracking().Include(a => a.Books).ThenInclude(b => b.Publisher).FirstAsync(a => a.Id == author.Id);
            Assert.Equal("William", saved.FirstName);
            Assert.Equal("Shakespeare", saved.LastName);
            Assert.Equal(updatedAuthor.Books.Count(), saved.Books.Count());
            Assert.True(saved.Books.All(b => b.Publisher.Id == publisher.Id));
        }

        [Fact]
        public async Task Update_TrackGraph_2()
        {
            // Arrange
            var context = await dbFixture.CreateContextAsync();
            var publisher = new Publisher { Name = "ABC Press" + DbContextFixture.NewUniqueIndex };
            var author = new Author
            {
                FirstName = "William",
                LastName = "Shakespeare",
                Books = new List<Book> {
                    new Book{ Title = "Hamlet", Isbn="1234", Publisher = publisher },
                    new Book{ Title = "Othello", Isbn="2345", Publisher = publisher },
                    new Book{ Title = "MacBeth", Isbn="3456", Publisher = publisher  }
                }
            };
            context.Add(author);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            // Act
            var updatedAuthor = new Author
            {
                Id = author.Id,
                FirstName = "William",
                LastName = "Shakespeare",
                RowVersion = author.RowVersion
            };

            foreach (var bb in author.Books.Select(b => new Book { Id = b.Id, Title = b.Title + " v2", RowVersion = b.RowVersion }))
                updatedAuthor.Books.Add(bb);

            // ensures that all entities are tracked in the UnChanged state, and then indicates that the Isbn property is modified. 
            context.ChangeTracker.TrackGraph(updatedAuthor, e =>
            {
                e.Entry.State = EntityState.Unchanged; //starts tracking
                if ((e.Entry.Entity as Book) != null)
                {
                    context.Entry(e.Entry.Entity as Book).Property(b => b.Title).IsModified = true;
                }
            });
            await context.SaveChangesAsync();

            // Assert
            var saved = await context.Authors.AsNoTracking().Include(a => a.Books).ThenInclude(b => b.Publisher).FirstAsync(a => a.Id == author.Id);
            Assert.Equal("William", saved.FirstName);
            Assert.Equal("Shakespeare", saved.LastName);
            Assert.Equal(updatedAuthor.Books.Count(), saved.Books.Count());
            Assert.True(saved.Books.All(b => b.Title.EndsWith("v2")));
            Assert.True(saved.Books.All(b => b.Publisher.Id == publisher.Id));
        }

        [Fact]
        public async Task Update_Relationship_Tracked()
        {
            // Arrange
            var context = await dbFixture.CreateContextAsync();
            var author = new Author { FirstName = "William", LastName = "Shakespeare" };
            var publisher = new Publisher { Name = "ABC Press" + DbContextFixture.NewUniqueIndex };
            context.AddRange(author, publisher);
            await context.SaveChangesAsync();

            // Act
            var book = new Book { Title = "Romeo and Juliet", Isbn = "ISBN:" + DbContextFixture.NewUniqueIndex, };
            book.Author = context.Authors.Single(a => a.Id == author.Id); //  Author Unchanged
            book.Publisher = context.Publishers.Single(a => a.Id == publisher.Id); // Publisher Unchanged
            book.Author.FirstName = "Bill"; // Author Modified
            context.Add(book); // Book Added, Author Modified
            await context.SaveChangesAsync();

            // Assert
            Assert.Equal(1, context.Authors.AsNoTracking().Count(a => a.Id == author.Id));
            Assert.Equal(1, context.Publishers.AsNoTracking().Count(a => a.Id == publisher.Id));
            Assert.Equal(1, context.Books.AsNoTracking().Count(a => a.Id == book.Id));
            var saved = context.Authors.AsNoTracking().Single(a => a.Id == author.Id);
            Assert.Equal("Bill", saved.FirstName);
        }

        [Fact]
        public async Task Update_Relationship_FakeStub()
        {
            // Arrange
            var context = await dbFixture.CreateContextAsync();
            var book = new Book { Title = "Romeo and Juliet", Isbn = "ISBN:" + DbContextFixture.NewUniqueIndex, Author = new Author { FirstName = "William", LastName = "Shakespeare" }, Publisher = new Publisher { Name = "ABC Press" + DbContextFixture.NewUniqueIndex } };
            context.Add(book);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            // Act
            var stub = new Book { Id = book.Id, RowVersion = book.RowVersion };
            context.Attach(stub);
            stub.Author = new Author
            {
                FirstName = "Charles",
                LastName = "Dickens"
            };
            // Book Modified, Author Added - no "store generated key", Publisher unchanged
            await context.SaveChangesAsync();

            // Assert
            var saved = context.Books.AsNoTracking().Include(b => b.Author).Include(b => b.Publisher).Single(a => a.Id == book.Id);
            Assert.Equal(stub.Author.Id, saved.Author.Id);
            Assert.Equal(book.Publisher.Id, saved.Publisher.Id);
            Assert.Equal(2, context.Authors.AsNoTracking().Count(a => new[] { book.Author.Id, stub.Author.Id }.Contains(a.Id)));
            Assert.Equal(1, context.Publishers.AsNoTracking().Count(a => a.Id == book.Publisher.Id));
            Assert.Equal(1, context.Books.AsNoTracking().Count(a => a.Id == stub.Id));
        }

        [Fact]
        public async Task Update_Relationship_FK()
        {
            // Arrange
            var context = await dbFixture.CreateContextAsync();
            var book = new Book { Title = "Romeo and Juliet", Isbn = "ISBN:" + DbContextFixture.NewUniqueIndex, Author = new Author { FirstName = "William", LastName = "Shakespeare" }, Publisher = new Publisher { Name = "ABC Press" + DbContextFixture.NewUniqueIndex } };
            var author = new Author { FirstName = "Bill", LastName = "Shakespeare" };
            context.AddRange(book, author);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            // Act
            var newBook = new Book { Title = "The Winters Tale", Isbn = "ISBN:" + DbContextFixture.NewUniqueIndex, AuthorId = author.Id, PublisherId = book.Publisher.Id };
            context.Add(newBook);
            await context.SaveChangesAsync();

            // Assert
            var saved = context.Books.AsNoTracking().Include(b => b.Author).Include(b => b.Publisher).Single(a => a.Id == newBook.Id);
            Assert.Equal(author.FirstName, saved.Author.FirstName);
            Assert.Equal(book.Publisher.Name, saved.Publisher.Name);
            Assert.Equal(2, context.Authors.AsNoTracking().Count(a => new[] { book.Author.Id, author.Id }.Contains(a.Id)));
            Assert.Equal(1, context.Publishers.AsNoTracking().Count(a => a.Id == book.Publisher.Id));
            Assert.Equal(2, context.Books.AsNoTracking().Count(a => new[] { book.Id, newBook.Id }.Contains(a.Id)));
        }

        #endregion

        #region Delete

        [Fact]
        public async Task Delete_TrackedRecord()
        {
            // Arrange
            var context = await dbFixture.CreateContextAsync();
            var publisher = new Publisher { Name = "ABC Press" + DbContextFixture.NewUniqueIndex };
            var author = new Author
            {
                FirstName = "William",
                LastName = "Shakespeare",
                Books = new List<Book> {
                    new Book{ Title = "Hamlet", Isbn = "ISBN:" + DbContextFixture.NewUniqueIndex, Publisher = publisher },
                    new Book{ Title = "Othello", Isbn = "ISBN:" + DbContextFixture.NewUniqueIndex, Publisher = publisher },
                    new Book{ Title = "MacBeth", Isbn = "ISBN:" + DbContextFixture.NewUniqueIndex, Publisher = publisher }
                }
            };
            context.Add(author);
            await context.SaveChangesAsync();

            // Act
            context.Remove(author);
            await context.SaveChangesAsync();

            // Assert
            Assert.Equal(0, context.Authors.AsNoTracking().Count(a => a.Id == author.Id));
            Assert.Equal(1, context.Publishers.AsNoTracking().Count(a => a.Id == publisher.Id));
            Assert.Equal(0, context.Books.AsNoTracking().Count(a => author.Books.Select(b => b.Id).Contains(a.Id)));
        }

        [Fact]
        public async Task Delete_RecordByPK()
        {
            // Arrange
            var context = await dbFixture.CreateContextAsync();
            var publisher = new Publisher { Name = "ABC Press" + DbContextFixture.NewUniqueIndex };
            var author = new Author
            {
                FirstName = "William",
                LastName = "Shakespeare",
                Books = new List<Book> {
                    new Book{ Title = "Hamlet", Isbn = "ISBN:" + DbContextFixture.NewUniqueIndex, Publisher = publisher },
                    new Book{ Title = "Othello", Isbn = "ISBN:" + DbContextFixture.NewUniqueIndex, Publisher = publisher },
                    new Book{ Title = "MacBeth", Isbn = "ISBN:" + DbContextFixture.NewUniqueIndex, Publisher = publisher }
                }
            };
            context.Add(author);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            // Act
            var authorToDel = await context.Authors.FindAsync(author.Id);
            context.Remove(authorToDel);
            await context.SaveChangesAsync();

            // Assert
            Assert.Equal(0, context.Authors.AsNoTracking().Count(a => a.Id == author.Id));
            Assert.Equal(1, context.Publishers.AsNoTracking().Count(a => a.Id == publisher.Id));
            Assert.Equal(0, context.Books.AsNoTracking().Count(a => author.Books.Select(b => b.Id).Contains(a.Id)));
        }

        [Fact]
        public async Task Delete_SetEntityState()
        {
            // Arrange
            var context = await dbFixture.CreateContextAsync();
            var publisher = new Publisher { Name = "ABC Press" + DbContextFixture.NewUniqueIndex };
            var author = new Author
            {
                FirstName = "William",
                LastName = "Shakespeare",
                Books = new List<Book> {
                    new Book{ Title = "Hamlet", Isbn = "ISBN:" + DbContextFixture.NewUniqueIndex, Publisher = publisher },
                    new Book{ Title = "Othello", Isbn = "ISBN:" + DbContextFixture.NewUniqueIndex, Publisher = publisher },
                    new Book{ Title = "MacBeth", Isbn = "ISBN:" + DbContextFixture.NewUniqueIndex, Publisher = publisher }
                }
            };
            context.Add(author);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            // Act
            var authorToDel = await context.Authors.FindAsync(author.Id);
            context.Entry(authorToDel).State = EntityState.Deleted;
            await context.SaveChangesAsync();

            // Assert
            Assert.Equal(0, context.Authors.AsNoTracking().Count(a => a.Id == author.Id));
            Assert.Equal(1, context.Publishers.AsNoTracking().Count(a => a.Id == publisher.Id));
            Assert.Equal(0, context.Books.AsNoTracking().Count(a => author.Books.Select(b => b.Id).Contains(a.Id)));
        }

        [Fact]
        public async Task Delete_RelationalRecords()
        {
            // By default, this relationship is configured as optional and the referential constraint action option is configured to NoAction. In addition, EF Core introduces a shadow property to represent the foreign key. It is named AuthorId and is applied to the Book entity, and since the relationship is optional, the AuthorId property is nullable. In order to delete the author, you need to delete the relationship between each book and the author.
            // If Books has AuthorId defined explictly, the referentail constraint will turn to CASCADE DELTE.

            // Arrange
            var context = await dbFixture.CreateContextAsync();
            var publisher = new Publisher { Name = "ABC Press" + DbContextFixture.NewUniqueIndex };
            var author = new Author
            {
                FirstName = "William",
                LastName = "Shakespeare",
                Books = new List<Book> {
                    new Book{ Title = "Hamlet", Isbn = "ISBN:" + DbContextFixture.NewUniqueIndex, Publisher = publisher },
                    new Book{ Title = "Othello", Isbn = "ISBN:" + DbContextFixture.NewUniqueIndex, Publisher = publisher },
                    new Book{ Title = "MacBeth", Isbn = "ISBN:" + DbContextFixture.NewUniqueIndex, Publisher = publisher }
                }
            };
            context.Add(author);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            // Act
            var authorToDel = context.Authors.Single(a => a.Id == author.Id);
            context.Remove(authorToDel);
            await context.SaveChangesAsync();

            // Assert
            Assert.Equal(0, context.Authors.AsNoTracking().Count(a => a.Id == author.Id));
            Assert.Equal(1, context.Publishers.AsNoTracking().Count(a => a.Id == publisher.Id));
            Assert.Equal(0, context.Books.AsNoTracking().Count(a => author.Books.Select(b => b.Id).Contains(a.Id)));
        }

        #endregion
    }
}
