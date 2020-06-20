using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace DioSummitFunctions
{
    public static class ResizeImage
    {
        [FunctionName("ResizeImage")]
        public static void Run([BlobTrigger("originals/{name}", Connection = "OriginalsStorage")] Stream image, string name,
            [Blob("resized/thumbs/{name}", FileAccess.Write, Connection = "ResizedStorage")] Stream imageThumb,
            [Blob("resized/small/{name}", FileAccess.Write, Connection = "ResizedStorage")] Stream imageSmall,
            [Blob("resized/medium/{name}", FileAccess.Write, Connection = "ResizedStorage")] Stream imageMedium,
            [Blob("resized/large/{name}", FileAccess.Write, Connection = "ResizedStorage")] Stream imageLarge,
            ILogger log)
        {
            IImageFormat format;
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {image.Length} Bytes");

            try
            {
                using (Image<Rgba32> input = Image.Load<Rgba32>(image, out format))
                {
                    DoResize(input, imageThumb, ImageSize.Thumb, format);
                }

                image.Position = 0;
                using (Image<Rgba32> input = Image.Load<Rgba32>(image, out format))
                {
                    DoResize(input, imageSmall, ImageSize.Small, format);
                }

                image.Position = 0;
                using (Image<Rgba32> input = Image.Load<Rgba32>(image, out format))
                {
                    DoResize(input, imageMedium, ImageSize.Medium, format);
                }

                image.Position = 0;
                using (Image<Rgba32> input = Image.Load<Rgba32>(image, out format))
                {
                    DoResize(input, imageLarge, ImageSize.Large, format);
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
            }
        }

        public static void DoResize(Image<Rgba32> input, Stream output, ImageSize size, IImageFormat format)
        {
            var dimensions = imageDimensionsTable[size];

            input.Mutate(x => x.Resize(dimensions.Item1, dimensions.Item2));
            input.Save(output, format);
        }

        public enum ImageSize { Thumb, Small, Medium, Large }

        private static Dictionary<ImageSize, (int, int)> imageDimensionsTable = new Dictionary<ImageSize, (int, int)>() {
            { ImageSize.Thumb,      (320, 200) },
            { ImageSize.Small,      (640, 400) },
            { ImageSize.Medium,     (800, 600) },
            { ImageSize.Large,     (1920, 1080) }
        };
    }
}
