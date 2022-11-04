using System.Drawing;
using System.IO;
using Microsoft.Azure.WebJobs;

namespace ThumbnailImageGenerator
{
    public class ThumbnailGenerator
    {
        [FunctionName("generate")]
        public void Run([BlobTrigger("profileimages/{name}", Connection = "BlobStorageConnection")]Stream image, [Blob("thumbnailimages/thumbnail-{name}", FileAccess.Write, Connection = "BlobStorageConnection")] Stream thumbnail)
        {
            byte[] imageBytes;

            byte[] buffer = new byte[16 * 1024];

            using (MemoryStream stream = new MemoryStream())
            {
                int read;
                while ((read = image.Read(buffer, 0, buffer.Length)) > 0)
                {
                    stream.Write(buffer, 0, read);
                }
                imageBytes = stream.ToArray();
            }

            using (MemoryStream stream = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                using (Image imageFromStream = Image.FromStream(stream))
                {
                    int height = 100;
                    int width = 100;

                    using (Bitmap bitmap = new Bitmap(imageFromStream, new Size(width, height)))
                    {
                        bitmap.Save(thumbnail, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                }
            }
        }
    }
}
