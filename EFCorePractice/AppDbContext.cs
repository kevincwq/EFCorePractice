using Microsoft.EntityFrameworkCore;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
