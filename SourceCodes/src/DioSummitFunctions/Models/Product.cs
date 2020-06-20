using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class Product
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public IEnumerable<string> Images { get; set; }
        public IEnumerable<ProductReview> Reviews { get; set; }
    }
}
