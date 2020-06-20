using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ICosmosDbService _cosmosDbService;

        public ProductsController(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductListAsync()
        {
            return Ok(await _cosmosDbService.GetProductListAsync());
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProductAsync(string productId)
        {
            var result = await _cosmosDbService.GetProductAsync(productId);

            if (result != null)
            {
                return Ok(result);
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> AddProductAsync([FromBody] Product product)
        {
            await _cosmosDbService.AddProductAsync(product);
            return Ok();
        }

        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateProductAsync(string productId, [FromBody] Product product)
        {
            await _cosmosDbService.UpdateProductAsync(productId, product);
            return Ok();

        }
    }
}
