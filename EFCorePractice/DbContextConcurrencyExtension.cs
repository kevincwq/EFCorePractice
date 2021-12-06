using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCorePractice
{
    public static class DbContextConcurrencyExtension
    {
        /// <summary>
        /// "Database Win" strategy.
        /// </summary>
        public static async Task SaveChangesAsync_DatabaseWins(this DbContext ctx)
        {
            bool saveFailed;
            do
            {
                saveFailed = false;

                try
                {
                    await ctx.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;
                    // Reload the values of the entities that failed to save from the db 
                    foreach (var entry in ex.Entries)
                        await entry.ReloadAsync();
                }

            } while (saveFailed);
        }

        /// <summary>
        /// "Client Win" strategy.
        public static async Task SaveChangesAsync_ClientWins(this DbContext ctx)
        {
            bool saveFailed;
            do
            {
                saveFailed = false;

                try
                {
                    await ctx.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;
                    // Update original values from the database 
                    foreach (var entry in ex.Entries)
                    {
                        var originalValues = await entry.GetDatabaseValuesAsync();
                        entry.OriginalValues.SetValues(originalValues);
                    }
                }

            } while (saveFailed);
        }

        /// <summary>
        /// "Merge Client Modifies" strategy will reload database values and merge client modified values.
        /// </summary>
        public static async Task SaveChangesAsync_MergeClientModifies(this DbContext ctx)
        {
            bool saveFailed;
            do
            {
                saveFailed = false;

                try
                {
                    await ctx.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;
                    // Merge values 
                    foreach (var entry in ex.Entries)
                    {
                        var databaseValues = await entry.GetDatabaseValuesAsync();

                        // Clone the values currently in the database an initial set of resolved values.
                        var resolvedValues = databaseValues.Clone();
                        // Select modified values from current values.
                        var modifiedValues = entry.Properties.Where(p => p.IsModified).ToDictionary(p => p.Metadata.Name, p => p.CurrentValue);
                        // Merge modified values to resolved values.
                        resolvedValues.SetValues(modifiedValues);

                        // Update the original values with the database values
                        // Update the current values with resolved values.
                        entry.OriginalValues.SetValues(databaseValues);
                        entry.CurrentValues.SetValues(resolvedValues);
                    }
                }

            } while (saveFailed);
        }
    }
}
