using System.Threading.Tasks;

namespace Vocalia.Ingest.Image
{
    public interface IImageStorage
    {
        /// <summary>
        /// Uploads the provided byte array to the Ingest blob storage account,
        /// and returns the URL.
        /// </summary>
        /// <param name="image">Image byte array to upload.</param>
        /// <param name="imageType">File type of the image beiung uploaded.</param>
        /// <returns></returns>
        Task<string> UploadImageAsync(byte[] image, string imageType);
    }
}