using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Infrastructure.Data
{
    public class ProductRepository(CatalogDbContext db) : IProductRepository
    {
        public Task<List<Product>> GetAllProductsAsync() =>
            db.Products.AsNoTracking().ToListAsync();

        public Task<Product?> GetProductByIdAsync(Guid id) =>
            db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        
        public async Task AddProductAsync(Product product)
        {
            await db.Products.AddAsync(product);
            await db.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            db.Products.Update(product);
            await db.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(Product product)
        {
            db.Products.Remove(product);
            await db.SaveChangesAsync();
        }
    }
}
