using System;
using System.Collections.Generic;
using System.Linq;
using Azure;
using Azure.AI.TextAnalytics;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebApi.Models;

namespace DioSummitFunctions
{
    public static class Sentiments
    {
        [FunctionName("Sentiments")]
        public static void Run([ServiceBusTrigger("newproduct", "Sentiment", Connection = "ServiceBusConnectionString")] string message,
            [CosmosDB(databaseName: "ProductStore",
                collectionName: "Product",
                ConnectionStringSetting = "CosmosDb")] out dynamic result,
            ILogger log, ExecutionContext context)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            log.LogInformation($"C# ServiceBus topic trigger function processed message: {message}");

            var product = JsonConvert.DeserializeObject<Product>(message);

            if (product != null && product.Reviews != null)
            {
                AzureKeyCredential credentials = new AzureKeyCredential(config["AzureKeyCrededntial"]);
                Uri endpoint = new Uri(config["SentimentEndpoint"]);
                var client = new TextAnalyticsClient(endpoint, credentials);

                var reviews = product.Reviews.ToList();
                product.Reviews = new List<ProductReview>();

                foreach(var review in reviews)
                {
                    DocumentSentiment documentSentiment = client.AnalyzeSentiment(review.Title + "; " + review.Content);
                    review.Type = documentSentiment.Sentiment.ToString();
                }

                product.Reviews = reviews;;
                result = product;
            }
            else
            {
                result = null;
            }
        }
    }
}
