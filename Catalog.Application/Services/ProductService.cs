using Catalog.Application.DTOs;
using Catalog.Application.Interfaces;
using Catalog.Application.Mappings;
using Catalog.Domain.Entities;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Catalog.Application.Services
{
    public class ProductService(IProductRepository repo, 
        IValidator<CreateProductRequest> createValidator, 
        IValidator<UpdateProductRequest> updateValidator,
        ILogger<ProductService> logger) : IProductService
    {
        public async Task<List<ProductResponse>> GetAllAsync()
        {
            var products = await repo.GetAllProductsAsync();
            return products.Select(p => p.ToResponse()).ToList();
        }
        
        public async Task<ProductResponse?> GetByIdAsync(Guid id)
        {
            var product = await repo.GetProductByIdAsync(id);
            return product?.ToResponse();
        }

        public async Task<ProductResponse> CreateAsync(CreateProductRequest request)
        {
            await createValidator.ValidateAndThrowAsync(request);

            var product = new Product
            { 
                Name = request.Name, 
                Description = request.Description, 
                Price = request.Price, 
                Stock = request.Stock, 
                Category = request.Category 
            };

            await repo.AddProductAsync(product);

            logger.LogInformation("Product {ProductId} created with name {ProductName}", product.Id, product.Name);
            return product.ToResponse();
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateProductRequest request)
        {
            await updateValidator.ValidateAndThrowAsync(request);

            var product = await repo.GetProductByIdAsync(id);
            if (product is null) return false;

            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.Stock = request.Stock;
            product.Category = request.Category;

            await repo.UpdateProductAsync(product);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var product = await repo.GetProductByIdAsync(id);
            if (product is null)
            {
                logger.LogWarning("Delete Failed: prodcut {ProductId} not found", id);
                return false;
            }
            await repo.DeleteProductAsync(product);

            logger.LogInformation("Product {ProductId} deleted", id);
            return true;
        }
    }
}
