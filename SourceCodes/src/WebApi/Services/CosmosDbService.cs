using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Services
{
    public class CosmosDbService : ICosmosDbService
    {
        private Container _container;

        public CosmosDbService(
            CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            this._container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task AddProductAsync(Product product)
        {
            product.Id = Guid.NewGuid().ToString();
            await this._container.CreateItemAsync(product, new PartitionKey(product.Id));
        }

        public async Task<Product> GetProductAsync(string id)
        {
            try
            {
                ItemResponse<Product> response = await this._container.ReadItemAsync<Product>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<IEnumerable<Product>> GetProductListAsync()
        {
            var query = this._container.GetItemQueryIterator<Product>(new QueryDefinition("SELECT * FROM c"));
            List<Product> results = new List<Product>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task UpdateProductAsync(string id, Product product)
        {
            product.Id = id;
            await this._container.UpsertItemAsync(product, new PartitionKey(id));
        }
    }
}
