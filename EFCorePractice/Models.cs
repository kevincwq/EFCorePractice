using System;
using System.Collections.Generic;

namespace EFCorePractice
{
    public class Author
    {
        public long Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        // One to One/Zero
        // a Reference navigation property having a multiplicity of zero or one
        public Address Address { get; set; }

        // One to One/Zero
        // A one to one (or more usually a one to zero or one) relationship exists
        // when only one row of data in the principal table is linked to zero or one row in a dependent table.
        // Note:
        // - One reason for implementing this kind of relationship is when you are working with inheritance.
        // For example, you may have a Vehicle entity, with sub classes such as Car, Truck, Motorcycle etc.
        // - Other reasons include database design and/or efficiency.
        // For example, you may want to apply extra database security to the dependent table because it contains confidential information (an employee's health record, for example), or you just want to move data that isn't referenced very often into a separate table to improve search and retrieval times for data that is used all the time.
        public AuthorBiography Biography { get; set; }

        // One to Many
        // a Collection navigation property having a multiplicity of many
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

        // Many to Many
        public ICollection<Category> Categories { get; set; } = new List<Category>();
    }

    public class Publisher
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public Address Address { get; set; }

        // Many to Many
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }

    public class Category
    {
        public long Id { get; set; }
        public string CategoryName { get; set; }

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

    public class Contact
    {
        public long Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }
    }
}
