using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoPlatform.Domain.Models;

namespace VideoPlatform.Domain.Interfaces
{
    public interface IVideoStorageAccessor
    {
        Task<List<Video>> GetContainerVideoListAsync(string containerName);
        Task<Video?> GetVideoByContainerAndNameAsync(string containerName, string fileName);

        Task<bool> UploadVideoAsync(string containerName, string fileName, Stream videoStream);
        Task<bool> DeleteVideoIfExistsAsync(string containerName, string fileName);
    }
}
