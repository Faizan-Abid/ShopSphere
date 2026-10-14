using Catalog.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Testcontainers.PostgreSql;

namespace Catalog.IntegrationTest
{
    public class CatalogApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly PostgreSqlContainer _db = new PostgreSqlBuilder("postgres:17")
            .Build();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Point the API at the test container instead of your real database
            builder.UseSetting("ConnectionStrings:CatalogDb", _db.GetConnectionString());
        }

        public async Task InitializeAsync()
        {
            await _db.StartAsync();

            // Create the tables in the fresh test database
            using var scope = Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
            await context.Database.MigrateAsync();
        }

        Task IAsyncLifetime.DisposeAsync() => _db.DisposeAsync().AsTask();
    }
}
