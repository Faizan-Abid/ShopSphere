using Catalog.Application.DTOs;
using Catalog.Application.Interfaces;
using Catalog.Application.Services;
using Catalog.Application.Validators;
using Catalog.Domain.Entities;
using FluentValidation;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.UnitTests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _repoMock = new();
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            // Real validators (they're pure logic), fake repository
            _service = new ProductService(
                _repoMock.Object,
                new CreateProductRequestValidator(),
                new UpdateProductRequestValidator(),
                NullLogger<ProductService>.Instance);
        }

        [Fact]
        public async Task CreateAsync_ValidRequest_SavesAndReturnsProduct()
        {
            // Arrange
            var request = new CreateProductRequest("Laptop", "Gaming laptop", 250000, 10, "Laptops");

            // Act
            var result = await _service.CreateAsync(request);

            // Assert
            Assert.Equal("Laptop", result.Name);
            Assert.Equal(250000, result.Price);
            Assert.NotEqual(Guid.Empty, result.Id);
            _repoMock.Verify(r => r.AddProductAsync(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_InvalidRequest_ThrowsAndDoesNotSave()
        {
            var request = new CreateProductRequest("", "desc", 100, 10, "Laptops");

            await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAsync(request));

            _repoMock.Verify(r => r.AddProductAsync(It.IsAny<Product>()), Times.Never);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-5000)]
        public async Task CreateAsync_PriceNotPositive_Throws(int price)
        {
            var request = new CreateProductRequest("Laptop", "desc", price, 10, "Laptops");

            await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAsync(request));
        }

        [Fact]
        public async Task GetByIdAsync_ProductNotFound_ReturnsNull()
        {
            _repoMock.Setup(r => r.GetProductByIdAsync(It.IsAny<Guid>()))
                     .ReturnsAsync((Product?)null);

            var result = await _service.GetByIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_ExistingProduct_UpdatesFieldsButKeepsIdAndCreatedAt()
        {
            // Arrange
            var existing = new Product { Name = "Old", Price = 100, Stock = 1, Category = "X" };
            var originalCreatedAt = existing.CreatedAt;

            _repoMock.Setup(r => r.GetProductByIdAsync(existing.Id))
                     .ReturnsAsync(existing);

            var request = new UpdateProductRequest("New", "Updated", 200, 5, "Y");

            // Act
            var result = await _service.UpdateAsync(existing.Id, request);

            // Assert
            Assert.True(result);
            _repoMock.Verify(r => r.UpdateProductAsync(It.Is<Product>(p =>
                p.Id == existing.Id &&
                p.CreatedAt == originalCreatedAt &&
                p.Name == "New" &&
                p.Price == 200)), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ProductNotFound_ReturnsFalse()
        {
            _repoMock.Setup(r => r.GetProductByIdAsync(It.IsAny<Guid>()))
                     .ReturnsAsync((Product?)null);

            var request = new UpdateProductRequest("New", "Updated", 200, 5, "Y");

            var result = await _service.UpdateAsync(Guid.NewGuid(), request);

            Assert.False(result);
            _repoMock.Verify(r => r.UpdateProductAsync(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ExistingProduct_ReturnsTrueAndDeletes()
        {
            var product = new Product { Name = "Laptop", Price = 100, Category = "Laptops" };

            _repoMock.Setup(r => r.GetProductByIdAsync(product.Id))
                     .ReturnsAsync(product);

            var result = await _service.DeleteAsync(product.Id);

            Assert.True(result);
            _repoMock.Verify(r => r.DeleteProductAsync(product), Times.Once);
        }
    }
}
