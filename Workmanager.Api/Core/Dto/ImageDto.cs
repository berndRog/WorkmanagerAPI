namespace Workmanager.Api.Core.Dto;

public class ImageDto {
   public Guid Id { get; set; } = Guid.Empty;
   
   public string ContentType { get; set; } = string.Empty;
   public string RemoteUriPath { get; set; } = string.Empty;
   
   public Guid   UserId { get; set; } = Guid.Empty;
}