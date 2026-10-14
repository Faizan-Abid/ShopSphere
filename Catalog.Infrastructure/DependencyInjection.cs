using Catalog.Application.Interfaces;
using Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<CatalogDbContext>(o => o.UseNpgsql(connectionString));
            services.AddScoped<IProductRepository, ProductRepository>();
            return services;
        }
    }
}
