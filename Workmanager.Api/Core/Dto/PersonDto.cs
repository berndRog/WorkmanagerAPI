namespace Workmanager.Api.Core.Dto;

/// <summary>
/// PersonDto 
/// </summary>
public class PersonDto {
   public Guid Id { get; set; } = Guid.Empty;
   public string FirstName { get; set; } = string.Empty;
   public string LastName { get; set; } = string.Empty;
   public string? Email { get; set; } = null;
   public string? Phone { get; set; } = null;
   public string? ImagePath { get; set; } = null;    
   public string? RemoteUriPath { get; set; } = null;
   public Guid? AddressId { get; set; } = null;
   public Guid? ImageId { get; set; } = null;
}