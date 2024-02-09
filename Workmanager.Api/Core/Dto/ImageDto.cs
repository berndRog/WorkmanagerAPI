namespace Workmanager.Api.Core.Dto;

public class ImageDto {
   public Guid Id { get; set; } = Guid.Empty;
   public string RemoteUriPath { get; set; } = string.Empty;   
   public string ContentType { get; set; } = string.Empty;
   public DateTime Updated { get; set; } = DateTime.UtcNow;
   public Guid   UserId { get; set; } = Guid.Empty;
}