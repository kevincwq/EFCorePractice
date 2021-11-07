using Microsoft.EntityFrameworkCore;
using System;

namespace EFCorePractice
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }

        public DbSet<Book> Books { get; set; }

        public DbSet<Author> Authors { get; set; }

        public DbSet<AuthorBiography> AuthorBiographies { get; set; }

        public DbSet<Address> Addresses { get; set; }

        public DbSet<Publisher> Publishers { get; set; }

        public DbSet<Contact> Contacts { get; set; }

        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Shadow properties
            modelBuilder.Entity<Contact>()
                .Property<DateTime>("LastUpdated");

            base.OnModelCreating(modelBuilder);
        }

        //public override int SaveChanges()
        //{
        //    ChangeTracker.DetectChanges();

        //    foreach (var entry in ChangeTracker.Entries())
        //    {
        //        if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
        //        {
        //            entry.Property("LastUpdated").CurrentValue = DateTime.UtcNow;
        //        }
        //    }
        //    return base.SaveChanges();
        //}
    }
}
