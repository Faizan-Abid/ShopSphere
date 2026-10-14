using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Application.DTOs
{

    public record CreateProductRequest(string Name, string Description, decimal Price, int Stock, string Category);

    public record UpdateProductRequest(string Name, string Description, decimal Price, int Stock, string Category);

    public record ProductResponse(Guid Id, string Name, string Description, decimal Price, int Stock, string Category, DateTime CreatedAt);
}
