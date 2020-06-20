using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Azure.Documents;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DioSummitFunctions
{
    public static class UpdateImages
    {
        [FunctionName("UpdateImages")]
        public static void Run([CosmosDBTrigger(
            databaseName: "ProductStore",
            collectionName: "Product",
            ConnectionStringSetting = "CosmosDb",
            LeaseCollectionName = "leases2")]IReadOnlyList<Document> input,
            ILogger log, ExecutionContext context)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            if (input != null && input.Count > 0)
            {
                try
                {
                    var document = input[0];

                    var images = document.GetPropertyValue<IEnumerable<string>>("Images");
                    var image = images.First();
                    var fileName = image.Split("?")[0].Split("/").Last();
                    var extension = fileName.Contains(".") ? fileName.Substring(fileName.LastIndexOf(".")) : "jpg";

                    IEnumerable<string> imagesNew = new List<string>()
                    {
                        $"https://dioprosummitimages.blob.core.windows.net/resized/thumbs/{document.Id}{extension}",
                        $"https://dioprosummitimages.blob.core.windows.net/resized/small/{document.Id}{extension}",
                        $"https://dioprosummitimages.blob.core.windows.net/resized/medium/{document.Id}{extension}",
                        $"https://dioprosummitimages.blob.core.windows.net/resized/large/{document.Id}{extension}",
                    };

                    document.SetPropertyValue("Images", imagesNew);

                    var topicClient = new TopicClient(config["ServiceBusConnectionString"], "newproduct");
                    topicClient.SendAsync(new Message(Encoding.UTF8.GetBytes(document.ToString())));
                }
                catch (Exception ex)
                {
                    log.LogError(ex.Message);
                }
            }
        }
    }
}
