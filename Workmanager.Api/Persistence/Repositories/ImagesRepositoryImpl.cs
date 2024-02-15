using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Workmanager.Api.Core;
using Workmanager.Api.Core.DomainModel.Entities;

namespace Workmanager.Api.Persistence.Repositories;

internal class ImagesRepositoryImpl(
    AppDbContext dbContext
) : AGenericRepository<Image>(dbContext), ImagesRepository {

    private readonly AppDbContext _dbContext = dbContext;
    
    #region methods
    public async Task<bool> ImageExistsAsync(Guid id) {
        return await _dbContext.Images.AnyAsync(i => i.Id == id);
    }

    public async Task<Image?> GetImageAsync(Guid id) {
        return await _dbContext.Images
            .FirstOrDefaultAsync(i => i.Id == id);
    }
    
    
    
    
    // IMAGE FILES
    public async Task<Image?> GetImageByUriPathAsync(string uriPath) {
        return await _dbContext.Images
            .FirstOrDefaultAsync(i => i.RemoteUriPath == uriPath);
    }

    public async Task<(byte[], string, string)> LoadImageFile(
        string filePath, 
        string contentType
    ) {
        var readAllBytesAsync = await File.ReadAllBytesAsync(filePath);
        return (readAllBytesAsync, contentType, Path.GetFileName(filePath));
    }
    
    public async Task<string?> StoreImageFile(
        string path, 
        Stream stream
    ) {
        // Get the temporary folder, and combine a random file name with it
        var fileName = Path.GetRandomFileName();
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        var filePath = Path.Combine(path, fileName);

        await using FileStream targetStream = File.Create(filePath);
        await stream.CopyToAsync(targetStream);
        return fileName;
    }
    #endregion
}