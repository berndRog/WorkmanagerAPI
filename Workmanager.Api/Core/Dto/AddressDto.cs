namespace Workmanager.Api.Core.Dto;

public class AddressDto {
   public Guid Id { get; set; } = Guid.Empty;
   public string Street { get; set; } = string.Empty;
   public string Number { get; set; } = string.Empty;
   public string Postal { get; set; } = string.Empty;
   public string City { get; set; } = string.Empty;
}