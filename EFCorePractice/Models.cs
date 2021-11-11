using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;
using System.Collections.Generic;

namespace EFCorePractice
{
    public abstract class BaseEntity
    {
        public DateTimeOffset CreatedUtc { get; set; }

        public DateTimeOffset UpdatedUtc { get; set; }
    }

    public abstract class BaseEntityTypeConfiguration<TBase> : IEntityTypeConfiguration<TBase> where TBase : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<TBase> entityTypeBuilder)
        {
            // Optimistic concurrency control
            // SQL Server uses a rowversion column.
            // Unfortunately, SQLite has no such feature. So we implement similar functionality using a trigger.
            //CREATE TRIGGER UpdateXXXXVersion
            //AFTER UPDATE ON XXXXX
            //BEGIN
            //    UPDATE XXXXX
            //    SET Version = Version + 1
            //    WHERE rowid = NEW.rowid;
            //END;
            entityTypeBuilder.Property<byte[]>("RowVersion").IsRowVersion();
        }
    }

    public class Author : BaseEntity
    {
        public long Id { get; set; }

        // [Required]
        public string FirstName { get; set; }

        // [Required]
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

        // [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
    }

    public class AuthorTypeConfiguration : BaseEntityTypeConfiguration<Author>
    {
        public override void Configure(EntityTypeBuilder<Author> entityTypeBuilder)
        {
            entityTypeBuilder.Property(b => b.FirstName).IsRequired();
            entityTypeBuilder.Property(b => b.LastName).IsRequired();
            entityTypeBuilder.Ignore(b => b.FullName);
            base.Configure(entityTypeBuilder);
        }
    }

    public class Book : BaseEntity
    {
        public long Id { get; set; }

        // [Required]
        public string Title { get; set; }

        public string Isbn { get; set; }

        // The relationship is a required relationship.
        // That means that a dependant cannot exist without its principal.
        // If an author is deleted, all of the authors books must also be deleted.
        // EF Core will set up this behaviour automatically if you explicitly include a non-nullable foreign key property,
        public long AuthorId { get; set; }

        public Author Author { get; set; }

        public long PublisherId { get; set; }

        public Publisher Publisher { get; set; }

        // Many to Many
        public ICollection<Category> Categories { get; set; } = new List<Category>();
    }

    public class BookTypeConfiguration : BaseEntityTypeConfiguration<Book>
    {
        public override void Configure(EntityTypeBuilder<Book> entityTypeBuilder)
        {
            entityTypeBuilder.Property(b => b.Title).IsRequired();
            entityTypeBuilder.Property(b => b.Isbn).IsRequired();
            entityTypeBuilder.HasIndex(b => b.Title);
            entityTypeBuilder.HasIndex(b => b.Isbn).IsUnique();
            base.Configure(entityTypeBuilder);
        }
    }

    public class Publisher : BaseEntity
    {
        public long Id { get; set; }

        // [Required]
        public string Name { get; set; }

        public Address Address { get; set; }

        // Many to Many
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }

    public class PublisherTypeConfiguration : BaseEntityTypeConfiguration<Publisher>
    {
        public override void Configure(EntityTypeBuilder<Publisher> entityTypeBuilder)
        {
            entityTypeBuilder.Property(b => b.Name).IsRequired();
            entityTypeBuilder.HasAlternateKey(b => b.Name);
            base.Configure(entityTypeBuilder);
        }
    }

    public class Category : BaseEntity
    {
        public long Id { get; set; }

        // [Required]
        public string Name { get; set; }

        public ICollection<Book> Books { get; set; } = new List<Book>();
    }

    public class CategoryTypeConfiguration : BaseEntityTypeConfiguration<Category>
    {
        public override void Configure(EntityTypeBuilder<Category> entityTypeBuilder)
        {
            entityTypeBuilder.Property(b => b.Name).IsRequired();
            entityTypeBuilder.HasAlternateKey(b => b.Name);
            base.Configure(entityTypeBuilder);
        }
    }

    public class Address : BaseEntity
    {
        public long Id { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        public string StateOrProvince { get; set; }

        public string Country { get; set; }
    }

    public class AddressTypeConfiguration : BaseEntityTypeConfiguration<Address>
    {
        public override void Configure(EntityTypeBuilder<Address> entityTypeBuilder)
        {
            entityTypeBuilder.Property(b => b.City).IsRequired();
            entityTypeBuilder.Property(b => b.Street).IsRequired();
            entityTypeBuilder.Property(b => b.StateOrProvince).IsRequired();
            entityTypeBuilder.Property(b => b.Country).IsRequired();
            base.Configure(entityTypeBuilder);
        }
    }

    public class AuthorBiography : BaseEntity
    {
        public long Id { get; set; }

        public string Biography { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string PlaceOfBirth { get; set; }

        public string Nationality { get; set; }

        public long AuthorId { get; set; }

        public Author Author { get; set; }
    }

    public class AuthorBiographyTypeConfiguration : BaseEntityTypeConfiguration<AuthorBiography>
    {
        public override void Configure(EntityTypeBuilder<AuthorBiography> entityTypeBuilder)
        {
            entityTypeBuilder.Property(b => b.Biography).IsRequired();
            base.Configure(entityTypeBuilder);
        }
    }

    public class Contact : BaseEntity
    {
        public long Id { get; set; }

        // [Required]
        public string FirstName { get; set; }

        // [Required]
        public string LastName { get; set; }

        // [Required]
        public string Email { get; set; }

        public User CreatedBy { get; set; }

        public User UpdatedBy { get; set; }

        // [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
    }

    public class ContactTypeConfiguration : BaseEntityTypeConfiguration<Contact>
    {
        public override void Configure(EntityTypeBuilder<Contact> entityTypeBuilder)
        {
            // shadow property
            entityTypeBuilder.Property<DateTime>("LastUpdated");
            entityTypeBuilder.HasAlternateKey(b => b.Email);
            entityTypeBuilder.Property(b => b.FirstName).IsRequired();
            entityTypeBuilder.Property(b => b.LastName).IsRequired();
            entityTypeBuilder.Property(b => b.Email).IsRequired();
            entityTypeBuilder.Ignore(b => b.FullName);
            base.Configure(entityTypeBuilder);
        }
    }

    public class User : BaseEntity
    {
        public long Id { get; set; }

        // [Required]
        public string UserName { get; set; }

        // [InverseProperty("CreatedBy")]
        public List<Contact> ContactsCreated { get; set; }

        // [InverseProperty("UpdatedBy")]
        public List<Contact> ContactsUpdated { get; set; }
    }

    public class UserTypeConfiguration : BaseEntityTypeConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> entityTypeBuilder)
        {
            entityTypeBuilder.Property(b => b.UserName).IsRequired();
            entityTypeBuilder.HasAlternateKey(b => b.UserName);
            entityTypeBuilder.HasMany(u => u.ContactsCreated).WithOne(c => c.CreatedBy);
            entityTypeBuilder.HasMany(u => u.ContactsUpdated).WithOne(c => c.UpdatedBy);
            base.Configure(entityTypeBuilder);
        }
    }

    public abstract class Contract : BaseEntity
    {
        public long Id { get; set; }
        public DateTime StartDate { get; set; }
        public int Months { get; set; }
        public decimal Charge { get; set; }

        public string ContractType { get; set; }
    }

    public class MobileContract : Contract
    {
        public string MobileNumber { get; set; }
    }

    public class BroadbandContract : Contract
    {
        public int DownloadSpeed { get; set; }
    }

    public class TvContract : Contract
    {
        public PackageType PackageType { get; set; }
    }

    public enum PackageType
    {
        S, M, L, XL
    }

    public class ContractConfiguration : BaseEntityTypeConfiguration<Contract>
    {
        public override void Configure(EntityTypeBuilder<Contract> entityTypeBuilder)
        {
            // https://www.learnentityframeworkcore.com/inheritance/table-per-hierarchy
            // https://docs.microsoft.com/en-us/ef/core/modeling/inheritance

            // TPC: A separate table is used to represent each concrete type in the inheritance chain.

            // TPT: A separate table is used to represent each type in the inheritance chain, including abstract types. Tables that represent derived types are related to their base type via foreign keys.

            // TPH: One table is used to represent all classes in the hierarchy. A "discriminator" column is used to discriminate between differing types. The table takes the name of the base class or its associated DbSet property by default.
            entityTypeBuilder.ToTable("Contracts")
            .HasDiscriminator(c => c.ContractType)
            .HasValue<TvContract>("Tv")
            .HasValue<MobileContract>("Mobile")
            .HasValue<BroadbandContract>("Broadband");

            base.Configure(entityTypeBuilder);
        }
    }
}
