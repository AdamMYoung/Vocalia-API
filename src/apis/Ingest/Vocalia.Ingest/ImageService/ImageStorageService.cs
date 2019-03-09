using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Threading.Tasks;

namespace Vocalia.Ingest.ImageService
{
    public class ImageStorageService: IImageStorageService
    {
        private IConfiguration Config { get; }

        public ImageStorageService(IConfiguration config)
        {
            Config = config;
        }

        /// <summary>
        /// Uploads the provided byte array to the Ingest blob storage account,
        /// and returns the URL.
        /// </summary>
        /// <param name="image">Image byte array to upload.</param>
        /// <param name="imageType">File type of the image beiung uploaded.</param>
        /// <returns></returns>
        public async Task<string> UploadImageAsync(byte[] image, string imageType)
        {
            var fileName = Guid.NewGuid().ToString();
            var url = string.Concat(Config["BlobStorage:URL"], fileName, imageType);

            var creds = new StorageCredentials(Config["BlobStorage:Account"], Config["BlobStorage:Key"]);
            var blob = new CloudBlockBlob(new Uri(url), creds);

            await blob.UploadFromByteArrayAsync(image, 0, image.Length);

            return url;
        }
    }
}
