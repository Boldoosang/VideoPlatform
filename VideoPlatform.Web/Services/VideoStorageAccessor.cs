using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using VideoPlatform.Domain.Interfaces;
using VideoPlatform.Domain.Models;

namespace VideoPlatform.Web.Services
{
    public class VideoStorageAccessor : IVideoStorageAccessor
    {
        private readonly BlobServiceClient _blobServiceClient;

        public VideoStorageAccessor(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<List<Video>> GetContainerVideoListAsync(string containerName) {
            var container = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobs = container.GetBlobsAsync();

            var editedVideoList = new List<Video>();

            await foreach (var blob in blobs) {
                if (!blob.Name.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase))
                    continue;

                var blobClient = container.GetBlobClient(blob.Name);
                var blobProperties = await blobClient.GetPropertiesAsync();

                editedVideoList.Add(new Video() {
                    Title = blob.Name,
                    FilePath = blobClient.Uri.ToString(),
                    UploadDate = blobProperties.Value.LastModified.DateTime,
                });
            }

            return editedVideoList;
        }


        public async Task<Video?> GetVideoByContainerAndNameAsync(string containerName, string fileName)
        {
            var container = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = container.GetBlobClient(fileName);

            if (await blobClient.ExistsAsync())
            {
                var blobProperties = await blobClient.GetPropertiesAsync();

                return new Video
                {
                    Title = blobClient.Name,
                    FilePath = blobClient.Uri.ToString(),
                    UploadDate = blobProperties.Value.LastModified.DateTime,
                };
            }

            return null;
        }
        public async Task<bool> DeleteVideoIfExistsAsync(string containerName, string fileName) {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var videoBlobClient = containerClient.GetBlobClient(fileName);
            var videoDeleted = await videoBlobClient.DeleteIfExistsAsync();

            var thumbnailFileName = Path.ChangeExtension(fileName, ".png");
            var thumbnailBlobClient = containerClient.GetBlobClient(thumbnailFileName);
            await thumbnailBlobClient.DeleteIfExistsAsync();

            return videoDeleted.Value;
        }

        public async Task<bool> UploadVideoAsync(string containerName, string fileName, Stream videoStream)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            try
            {
                var response = await blobClient.UploadAsync(videoStream);
                return true;
            } catch(Exception)
            {
                return false;
            }
        }
    }
}
