using Workmanager.Api.Core.Enums;
using Workmanager.Api.Core.Misc;

namespace Workmanager.Api.Core.DomainModel.Entities;

public class Workorder: ABaseEntity {
   #region fields
   private Guid _id = Guid.Empty;
   #endregion

   #region properties
   public override Guid Id {
      get => _id;
      set => _id = value == Guid.Empty ? Guid.NewGuid() : value;
   }
   public string Title { get; set; } = string.Empty;
   public string Description { get; set; } = string.Empty;
   public DateTime Created { get; set; } = DateTime.UtcNow;
   public DateTime Started { get; set; } = DateTime.UtcNow;
   public DateTime Completed { get; set; } = DateTime.UtcNow;
   public TimeSpan Duration { get; set; } = TimeSpan.Zero;
   public string Remark { get; set; } = string.Empty;
   public WorkorderState State { get; set; } = WorkorderState.Default;
   // Workorder -> Person [0..1]
   public Person? Person {get; set; } = null;
   public Guid?   PersonId { get; set; } = null;
   #endregion

   #region methods
   public Workorder Copy(Workorder source) {
      Id = source.Id;
      Title = source.Title;
      Description = source.Description;
      Created = source.Created;
      Started = source.Started;
      Completed = source.Completed;
      Duration = source.Duration;
      Remark = source.Remark;
      State = source.State;
      Person = source.Person;
      PersonId = source.PersonId;
      return this;
   } 
   
   public string AsString() =>
      $"{Title} {Id.As8()}";
   #endregion

}