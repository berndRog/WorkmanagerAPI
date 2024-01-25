namespace Workmanager.Api.Core.Dto;

/// <summary>
/// PersonDto 
/// </summary>
public class PersonDto {
   public Guid Id { get; set; } = Guid.Empty;
   public string FirstName { get; set; } = string.Empty;
   public string LastName { get; set; } = string.Empty;
   public string? Email { get; set; } = string.Empty;
   public string? Phone { get; set; } = string.Empty;
   public string? ImagePath { get; set; } = string.Empty;
   public string? RemoteUriPath { get; set; } = string.Empty;
   public Guid? AddressId { get; set; } = null;
}