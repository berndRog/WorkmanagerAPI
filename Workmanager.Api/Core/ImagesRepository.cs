using Workmanager.Api.Core.DomainModel.Entities;
namespace Workmanager.Api.Core;

public interface ImagesRepository: IGenericRepository<Image> {
    
    Task<Image?> GetImageAsync(Guid id);
    Task<Image?> GetImageByUriPathAsync(string uriPath);
    
    Task<(byte[], string, string)> LoadImageFile(string filePath, string contentType);
    Task<string?> StoreImageFile(string path, Stream stream);


}

