using Catalog.Application.DTOs;
using Catalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Application.Mappings
{
    public static class ProductMappings
    {
        public static ProductResponse ToResponse(this Product p) =>
            new(p.Id, p.Name, p.Description, p.Price, p.Stock, p.Category, p.CreatedAt);
    }
}
