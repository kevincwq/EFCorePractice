﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

        public DbSet<MobileContract> MobileContracts { get; set; }

        public DbSet<BroadbandContract> BroadbandContracts { get; set; }

        public DbSet<TvContract> TvContracts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
            base.OnModelCreating(modelBuilder);
        }

        private void UpdateUtcs()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((BaseEntity)entityEntry.Entity).UpdatedUtc = DateTimeOffset.UtcNow;

                if (entityEntry.State == EntityState.Added)
                {
                    ((BaseEntity)entityEntry.Entity).CreatedUtc = DateTimeOffset.UtcNow;
                }
            }
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            UpdateUtcs();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public async override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            UpdateUtcs();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
