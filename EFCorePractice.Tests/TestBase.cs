﻿using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace EFCorePractice.Tests
{
    public abstract class TestBase : IDisposable
    {
        private readonly DbConnection _connection;
        private readonly DbContextOptions<AppDbContext> _options;

        public TestBase()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                 .UseSqlite(CreateInMemoryDatabase())
                 .Options;

            _connection = RelationalOptionsExtension.Extract(_options).Connection;
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            return connection;
        }

        public void Dispose() => _connection.Dispose();

        /// <summary>
        /// SQLite is a relational provider and can also use in-memory databases.
        /// Consider using this for testing to more closely match common relational database behaviors.
        /// https://docs.microsoft.com/en-us/ef/core/testing/in-memory
        /// This database is destroyed when the connection is closed.
        /// This means, you must keep the connection open until the test ends.
        /// Indeed, when the context is disposed, the connection is closed and the database is destroyed.
        /// </summary>
        /// <returns></returns>
        protected async Task<AppDbContext> CreateSQLiteDbContextAsync()
        {
            var context = new AppDbContext(_options);

            if (context != null)
            {
                // await context.Database.EnsureDeletedAsync();
                await context.Database.MigrateAsync();
            }
            return context;
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
