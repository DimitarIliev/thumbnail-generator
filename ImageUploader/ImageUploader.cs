using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace ImageUploader
{
    public static class ImageUploader
    {
        [FunctionName("upload")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [Blob("profileimages", Connection = "BlobStorageConnection")] CloudBlobContainer outputContainer)
        {
            byte[] imageBytes;

            var file = req.Form.Files[0];

            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                imageBytes = ms.ToArray();
            }

            var cloudBlockBlob = outputContainer.GetBlockBlobReference(file.FileName);

            using (MemoryStream stream = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                await cloudBlockBlob.UploadFromStreamAsync(stream);
            }

            return new OkResult();
        }
    }
}
