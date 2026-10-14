using Catalog.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace Catalog.IntegrationTest
{
    public class ProductApiTests(CatalogApiFactory factory) : IClassFixture<CatalogApiFactory>
    {
        private readonly HttpClient _client = factory.CreateClient();

        [Fact]
        public async Task CreateThenGet_ReturnsSameProduct()
        {
            var request = new CreateProductRequest("Laptop", "Gaming laptop", 250000, 10, "Laptops");

            var createResponse = await _client.PostAsJsonAsync("/api/products", request);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            var created = await createResponse.Content.ReadFromJsonAsync<ProductResponse>();

            var getResponse = await _client.GetAsync($"/api/products/{created!.Id}");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var fetched = await getResponse.Content.ReadFromJsonAsync<ProductResponse>();
            Assert.Equal("Laptop", fetched!.Name);
            Assert.Equal(250000, fetched.Price);
        }

        [Fact]
        public async Task Create_InvalidProduct_Returns400()
        {
            var request = new CreateProductRequest("", "desc", -1, 0, "");

            var response = await _client.PostAsJsonAsync("/api/products", request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetById_NonExisting_Returns404()
        {
            var response = await _client.GetAsync($"/api/products/{Guid.NewGuid()}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ExistingProduct_Returns204_ThenGetReturns404()
        {
            var request = new CreateProductRequest("Phone", "Smartphone", 80000, 5, "Phones");
            var createResponse = await _client.PostAsJsonAsync("/api/products", request);
            var created = await createResponse.Content.ReadFromJsonAsync<ProductResponse>();

            var deleteResponse = await _client.DeleteAsync($"/api/products/{created!.Id}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            var getResponse = await _client.GetAsync($"/api/products/{created.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }
    }
}
