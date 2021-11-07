using System;
using System.Collections.Generic;

namespace EFCorePractice
{
    public class Author
    {
        public long Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Address Address { get; set; }

        public AuthorBiography Biography { get; set; }

        public ICollection<Book> Books { get; set; } = new List<Book>();
    }

    public class Book
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public string Isbn { get; set; }

        public long AuthorId { get; set; }

        public Author Author { get; set; }

        public long PublisherId { get; set; }

        public Publisher Publisher { get; set; }
    }

    public class Publisher
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public Address Address { get; set; }

        public ICollection<Book> Books { get; set; } = new List<Book>();
    }

    public class Address
    {
        public long Id { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        public string StateOrProvince { get; set; }

        public string Country { get; set; }
    }

    public class AuthorBiography
    {
        public long Id { get; set; }

        public string Biography { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string PlaceOfBirth { get; set; }

        public string Nationality { get; set; }

        public long AuthorId { get; set; }

        public Author Author { get; set; }
    }
}
