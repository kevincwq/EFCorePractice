using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EFCorePractice.Tests
{
    [CollectionDefinition(nameof(DbContextCollection))]
    public class DbContextCollection : ICollectionFixture<DbContextFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

    public class DbContextFixture : IDisposable
    {
        private static long UniqueIndex = 1;
        public static long NewUniqueIndex => Interlocked.Increment(ref UniqueIndex);

        private readonly DbConnection _connection;
        private readonly DbContextOptions<AppDbContext> _options;

        public DbContextFixture()
        {
            _options = BuildDbContextOptions(false);

            _connection = RelationalOptionsExtension.Extract(_options).Connection;
        }

        private DbContextOptions<AppDbContext> BuildDbContextOptions(bool sqlite)
        {
            if (sqlite)
            {
                return new DbContextOptionsBuilder<AppDbContext>()
                 .UseSqlite(CreateInMemoryDatabase())
                 .Options;
            }
            else
            {
                return new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=EFCorePractice;Trusted_Connection=True;MultipleActiveResultSets=true;ConnectRetryCount=0")
                    .Options;
            }
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            return connection;
        }

        public void Dispose()
        {
            ClearAllData(new AppDbContext(_options));
            _connection?.Dispose();
        }

        /// <summary>
        /// SQLite is a relational provider and can also use in-memory databases.
        /// Consider using this for testing to more closely match common relational database behaviors.
        /// https://docs.microsoft.com/en-us/ef/core/testing/in-memory
        /// This database is destroyed when the connection is closed.
        /// This means, you must keep the connection open until the test ends.
        /// Indeed, when the context is disposed, the connection is closed and the database is destroyed.
        /// </summary>
        /// <returns></returns>
        public async Task<AppDbContext> CreateContextAsync()
        {
            var context = new AppDbContext(_options);

            if (context != null)
            {
                // await context.Database.EnsureDeletedAsync();
                await context.Database.MigrateAsync();
            }

            return context;
        }

        private void ClearAllData(AppDbContext context)
        {
            // var dbContext = await CreateContextAsync();
            //var tableNames = dbContext.Model.GetEntityTypes().Select(t => $"{t.GetSchema()}.\"{t.GetTableName()}\"").Distinct().ToList();
            //await dbContext.Database.ExecuteSqlRawAsync($"TRUNCATE TABLE {string.Join(",", tableNames)}");

            context.RemoveRange(context.Books);
            context.RemoveRange(context.AuthorBiographies);
            context.RemoveRange(context.Authors);
            context.RemoveRange(context.Publishers);
            context.RemoveRange(context.Addresses);
            context.RemoveRange(context.Contacts);
            context.RemoveRange(context.Categories);
            context.RemoveRange(context.MobileContracts);
            context.RemoveRange(context.BroadbandContracts);
            context.RemoveRange(context.TvContracts);
            context.SaveChanges();
        }

        /// <summary>
        /// The EF in-memory database often behaves differently than relational databases.
        /// Only use the EF in-memory database after fully understanding the issues and trade-offs involved.
        /// https://docs.microsoft.com/en-us/ef/core/testing/in-memory
        /// https://github.com/npgsql/efcore.pg/issues/774
        /// The database name allows the scope of the in-memory database to be controlled independently of the context.
        /// The in-memory database is shared anywhere the same name is used.
        /// </summary>
        private async Task<AppDbContext> CreateInMemoryDbContextAsync(string dbName = "InMemoryAppDb")
        {
            // The database name allows the scope of the in-memory database
            // to be controlled independently of the context. The in-memory database is shared
            // anywhere the same name is used.
            var option = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var context = new AppDbContext(option);
            if (context != null)
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.MigrateAsync();
            }

            return context;
        }

    }
}
