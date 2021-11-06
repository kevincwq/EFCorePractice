using System.Collections.Generic;

namespace EFCorePractice
{
    public class Author
    {
        public long Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public IList<Book> Books { get; set; } = new List<Book>();
    }

    public class Book
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public long AuthorId { get; set; }
        public Author Author { get; set; }
    }
}
