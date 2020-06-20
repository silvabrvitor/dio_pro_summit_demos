using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Services
{
    public interface ICosmosDbService
    {
        Task<IEnumerable<Product>> GetProductListAsync();
        Task<Product> GetProductAsync(string id);
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(string id, Product product);
    }
}
