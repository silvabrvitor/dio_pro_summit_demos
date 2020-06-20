using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DioSummitFunctions
{
    public static class GetAndStoreImage
    {
        [FunctionName("GetAndStoreImage")]
        public static void Run([CosmosDBTrigger(
            databaseName: "ProductStore",
            collectionName: "Product",
            ConnectionStringSetting = "CosmosDb",
            LeaseCollectionName = "leases")]IReadOnlyList<Document> input,
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
                    var doc = input.First();
                    var images = doc.GetPropertyValue<IEnumerable<string>>("Images");

                    if (images == null
                        || images.Count() > 1
                        || images.Any(img => img.Contains("blob.core.windows.net/originals")))
                    {
                        return;
                    }

                    var image = images.First();
                    var fileName = image.Split("?")[0].Split("/").Last();
                    var extension = fileName.Contains(".") ? fileName.Substring(fileName.LastIndexOf(".")) : "jpg";

                    var content = new WebClient().DownloadData(image);

                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(config["OriginalsStorage"]);
                    CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("originals");

                    CloudBlockBlob blob = cloudBlobContainer.GetBlockBlobReference($"{doc.Id}{extension}");
                    blob.Properties.ContentType = $"image/{extension.Replace(".", string.Empty)}";

                    blob.UploadFromByteArray(content, 0, content.Length);
                }
                catch (Exception ex)
                {
                    log.LogError(ex.Message);
                }
            }
        }
    }
}
