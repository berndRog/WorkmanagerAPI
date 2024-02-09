using Workmanager.Api.Core.Misc;
namespace Workmanager.Api.Core.DomainModel.Entities;

public class Person : ABaseEntity {
   
   #region fields
   protected Guid _id = Guid.Empty;
   #endregion

   #region properties
   public override Guid Id {
      get => _id;
      set => _id = value == Guid.Empty ? Guid.NewGuid() : value;
   }

   public string FirstName { get; set; } = string.Empty;
   public string LastName { get; set; } = string.Empty;
   public string? Email { get; set; } = null;
   public string? Phone { get; set; } = null;
   public string? ImagePath { get; set; } = null;
   public string? RemoteUriPath { get; set; } = null;
   
   // Relation Person --> Image [0..*]
   public Image? Image { get; set; } = null;
   public Guid? ImageId { get; set; } = null;
   
   // Relation Person --> Workorder [0..*]
   public ICollection<Workorder> Workorders { get; set; } = new List<Workorder>();
   #endregion

   #region methods
   public Person Copy(Person source) {
      Id = source.Id;
      FirstName = source.FirstName;
      LastName = source.LastName;
      Email = source.Email;
      Phone = source.Phone;
      ImagePath = source.ImagePath;
      RemoteUriPath = source.RemoteUriPath;
      return this;
   }

   public string AsString() =>
      $"{FirstName} {LastName} {Id.As8()}";

   
   public void AndOrUpdateWorkorder(Workorder workorder) {
      if (workorder.Person != null)
         throw new ApplicationException("Workorder is already asigned to another person");
      workorder.Person = this;
      workorder.PersonId = Id;

      var found = Workorders.FirstOrDefault(w => w.Id == workorder.Id);
      if (found == null) {
         Workorders.Add(workorder); // add new workorder
      } else {
         found.Copy(workorder);     // update existing workorder
      }
   }

   public void RemoveWorkorder(Workorder workorder) {
      if (workorder.PersonId != Id)
         throw new ApplicationException("Workorder is not asigned to this person");
      workorder.Person = null;
      workorder.PersonId = null;
      Workorders.Remove(workorder);
   }
   #endregion
   
}