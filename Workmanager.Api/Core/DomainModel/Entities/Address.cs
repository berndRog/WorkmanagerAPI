namespace Workmanager.Api.Core.DomainModel.Entities;

public class Address: ABaseEntity {
   #region fields
   protected Guid _id = Guid.Empty;
   #endregion

   #region properties
   public override Guid Id {
      get => _id;
      set => _id = value == Guid.Empty ? Guid.NewGuid() : value;
   }
   public string Street { get; set; } = string.Empty;
   public string Number { get; set; } = string.Empty;
   public string ZipCode { get; set; } = string.Empty;
   public string City { get; set; } = string.Empty;
   #endregion
}