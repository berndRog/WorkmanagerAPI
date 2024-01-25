using System.Net.Mime;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Workmanager.Api.Core;
using Workmanager.Api.Core.DomainModel.Entities;
using Workmanager.Api.Core.Dto;
namespace Workmanager.Api.Controllers;

[ApiVersion("1.0")]
[Route("workmanagerapi/v{version:apiVersion}")]

[ApiController]
public class ImagesController(
    IWebHostEnvironment webHostingEnvironment,
    ImagesRepository repository,
    IDataContext dataContext,
    IMapper mapper,
    ILogger<ImagesController> logger
) : ControllerBase {

    /// <summary>
    /// Get the imageFile by fileName
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    [HttpGet("imageFiles/{fileName}")]
    public async Task<IActionResult> DownloadFile(
        string fileName
    ) {
        // get the complete uri path of the image file
        var uriPath = $"{Request.Scheme}://{Request.Host}{Request.Path}";
        // get the image by uri path from database
        Image? image = await repository.GetImageByUriPathAsync(uriPath);
        if(image == null) return NotFound("Image with given uri path not found.");
        
        // get the local file path of the image file
        var filePath = Path.Combine(webHostingEnvironment.WebRootPath, "images", fileName);
        // load the file from local file system
        var (byteArray, contentType, fileDownloadName) = 
            await repository.LoadFile(filePath, image!.ContentType);
        
        // return the imageFile
        return File(byteArray, contentType, fileDownloadName);
    }
    
    /// <summary>
    /// Get an image by Id. 
    /// </summary>
    /// <param name="id:Guid">id of the beneficiary</param>
    /// <returns>ImageDto?</returns>
    /// <response code="200">Ok: Image with given id returned</response>
    /// <response code="404">NotFound: Image with given id not found</response>
    [HttpGet("images/{id:Guid}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ImageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ImageDto>> GetById(
        [FromRoute] Guid id
    ){
        logger.LogDebug("Get {id}", id);
        Image? image = await repository.FindByIdAsync(id);
        if(image == null) return NotFound("Image with given id not found.");

        return Ok(mapper.Map<ImageDto>(image));
    }
    
    
    /// <summary>
    /// Action for upload one large image file
    /// </summary>
    /// <remarks>
    /// Request to this action will not trigger any model binding or model validation,
    /// because this is a no-argument action
    /// </remarks>
    /// <returns></returns>
    [HttpPost("imageFiles")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<ImageDto>> UploadFile() {
        
        // Check if request is multipart/form-data
        var request = HttpContext.Request;
        if(request == null) return BadRequest("No request." );
        
        // Check if request is multipart/form-data
        if (!request.HasFormContentType ||
            !MediaTypeHeaderValue.TryParse(request.ContentType, out var mediaTypeHeader) ||
            string.IsNullOrEmpty(mediaTypeHeader.Boundary.Value)) {
            return new UnsupportedMediaTypeResult();
        }

        // Get boundary from content-type header
        var boundary = HeaderUtilities.RemoveQuotes(mediaTypeHeader.Boundary.Value).Value;
        if (boundary == null) return BadRequest("Invalid boundary.");

        // Create multipart reader
        var reader = new MultipartReader(boundary, request.Body);
        var section = await reader.ReadNextSectionAsync();

        // This sample try to get the first file from request and save it
        if(section != null) {
            // Get mime type from section header
            var mimeType = section.ContentType;
            if (mimeType == null || ! mimeType.StartsWith("image/")) return BadRequest("Invalid file type.");

            if (ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition) &&
                contentDisposition.DispositionType.Equals("form-data") &&
                !string.IsNullOrEmpty(contentDisposition.FileName.Value)) {

                // Don't trust any file name, file extension, and file data from the request unless you trust them completely
                var path = Path.Combine(webHostingEnvironment.WebRootPath, "images");
                // Save the imageFile to the local file system
                var fileName = await repository.StoreFile(path, section.Body);
                if (fileName == null) return BadRequest("File not saved.");
                
                // store the uri path and the contentType (Mime) of the imageFile in the image
                var imageUriPath = $"{request.Scheme}://{request.Host}{request.Path}/{fileName}";
                Image image = new Image {
                    ContentType = mimeType!,
                    RemoteUriPath = imageUriPath
                //  userId = AuthorizedUser.Id  for later use    
                };
                // save the image to repository and write to database
                repository.Add(image);
                await dataContext.SaveAllChangesAsync();
                
                var uri = new Uri(path, UriKind.Relative);
                return Created(uri: uri, value: mapper.Map<ImageDto>(image));
            }
        }
        return BadRequest("No file data in the request.");
    }

    
    /// <summary>
    /// Update an existing imageFile
    /// </summary>
    /// <param name="fileName">Name of the exisitng ImageFile</param>
    /// <returns>Updated ImageDto</returns>
    [HttpPut("imageFiles/{fileName}")]
    public async Task<ActionResult<ImageDto>> UpdateFile(
        string fileName
    ) {
        var request = HttpContext.Request;
        if(request == null) return BadRequest("No request." );
        
        // get the complete uri path of the image file
        var uriPath = $"{request.Scheme}://{request.Host}{request.Path}";
        
        // Retrieve the existing image from the repository
        var existingImage = await repository.GetImageByUriPathAsync(uriPath);
        if (existingImage == null) return NotFound($"Image not found.");
        
        // Check if request is multipart/form-data
        if (!request.HasFormContentType ||
            !MediaTypeHeaderValue.TryParse(request.ContentType, out var mediaTypeHeader) ||
            string.IsNullOrEmpty(mediaTypeHeader.Boundary.Value)) {
            return new UnsupportedMediaTypeResult();
        }

        // Get boundary from content-type header
        var boundary = HeaderUtilities.RemoveQuotes(mediaTypeHeader.Boundary.Value).Value;
        if (boundary == null) return BadRequest("Invalid boundary.");

        // Create multipart reader
        var reader = new MultipartReader(boundary, request.Body);
        var section = await reader.ReadNextSectionAsync();

        if (section != null) {
            // Get mime type from section header
            var mimeType = section.ContentType;
            if (mimeType == null || !mimeType.StartsWith("image/")) return BadRequest("Invalid file type.");
            
            if (ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition) &&
                contentDisposition.DispositionType.Equals("form-data") &&
                !string.IsNullOrEmpty(contentDisposition.FileName.Value)) {

                // Don't trust any file name, file extension, and file data from the request unless you trust them completely
                var path = Path.Combine(webHostingEnvironment.WebRootPath, "images");
                
                // Delete the old file
                var oldFileName = existingImage.RemoteUriPath.Split('/').Last();
                var oldFilePath = Path.Combine(path, oldFileName);
                if (System.IO.File.Exists(oldFilePath)) {
                    System.IO.File.Delete(oldFilePath);
                }

                // Save the new file
                var newFileName = await repository.StoreFile(path, section.Body);
                if (newFileName == null) return BadRequest("File not saved.");

                var imageUriPath = $"{Request.Scheme}://{Request.Host}{Request.Path}/{newFileName}";

                // Update the existing image record
                existingImage.ContentType = mimeType;
                existingImage.RemoteUriPath = imageUriPath;

                await dataContext.SaveAllChangesAsync();

                var uri = new Uri(path, UriKind.Relative);
                return Created(uri: uri, value: mapper.Map<ImageDto>(existingImage));
            }
        }

        return BadRequest("No file data in the request.");
    }
    
    /// <summary>
    /// Delete an existing imageFile
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("imageFiles/{fileName}")]
    public async Task<ActionResult> DeleteFile(
        string fileName
    ){
        
        var request = HttpContext.Request;
        if(request == null) return BadRequest("No request." );
        
        // get the complete uri path of the image file
        var uriPath = $"{request.Scheme}://{request.Host}{request.Path}";
        if(uriPath.Split('/').Last() != fileName) return BadRequest(
            "FileName in UriPath and fileName in Parameter are not equal" );
        
        // Retrieve the image from the repository using the UriPath
        var image = await repository.GetImageByUriPathAsync(uriPath);
        if (image == null)
            return NotFound($"Image not found.");

        // Construct the path to the file
        var path = Path.Combine(webHostingEnvironment.WebRootPath, "images");
        var filePath = Path.Combine(path, fileName);

        // Check if the file exists and delete it
        if (System.IO.File.Exists(filePath))
            System.IO.File.Delete(filePath);
        else 
            throw new IOException("File not found on the server");

        // Remove the image record from the database
        repository.Remove(image);
        await dataContext.SaveAllChangesAsync();

        return NoContent();
    }

    
    /// <summary>
    /// Action for upload a list of large image files
    /// </summary>
    /// <remarks>
    /// Request to this action will not trigger any model binding or model validation,
    /// because this is a no-argument action
    /// </remarks>
    /// <returns></returns>
    [HttpPost("imageFiles/multiple")]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<string>> UploadFiles() {
        var request = HttpContext.Request;

        // Check if request is multipart/form-data
        if (!request.HasFormContentType ||
            !MediaTypeHeaderValue.TryParse(request.ContentType, out var mediaTypeHeader) ||
            string.IsNullOrEmpty(mediaTypeHeader.Boundary.Value)) {
            return new UnsupportedMediaTypeResult();
        }

        var boundary = HeaderUtilities.RemoveQuotes(mediaTypeHeader.Boundary.Value).Value;
        if (boundary == null) return BadRequest("Invalid boundary.");

        var reader = new MultipartReader(boundary, request.Body);
        var section = await reader.ReadNextSectionAsync();

        while (section != null) {
            if (ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition) &&
                contentDisposition.DispositionType.Equals("form-data") &&
                !string.IsNullOrEmpty(contentDisposition.FileName.Value)) {
                // Don't trust any file name, file extension, and file data from the request unless you trust them completely
                var path = Path.Combine(webHostingEnvironment.WebRootPath, "images");
                var fileName = await repository.StoreFile(path, section.Body);
                if (fileName == null) return BadRequest("File not saved.");
                // only the first images is saved
                var uriPath = $"{Request.Path}/{fileName}";
                var uri = new Uri(uriPath, UriKind.Relative);
                return Created(uri, fileName);
            }
            section = await reader.ReadNextSectionAsync();
        }
        return BadRequest("No file data in the request.");
    }
    

    
    
/*


    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> UpdateImage(Guid id,
        [FromBody] ImageDtoUpdate imageDtoUpdate) {
        // var image = await _imagesRepository.GetImageAsync(id);
        // if (image == null) return NotFound();
        //
        // _mapper.Map(imageDtoUpdate, image);
        // await _imagesRepository.UpdateAsync(image);
        // await _imagesRepository.SaveChangesAsync();
        return NoContent();
    }
    #endregion
    */
}