using Workmanager.Api.Core.DomainModel.Entities;
using Workmanager.Api.Core.Misc;

namespace Workmanager.Api.Core.DomainModel.NullEntities;
// https://jonskeet.uk/csharp/singleton.html

public sealed class NullPerson: Person {  
   // Singleton Skeet Version 4
   private static readonly NullPerson instance = new NullPerson();
   public static NullPerson Instance { get => instance; }

   static NullPerson() { }
   
   private NullPerson() { 
      _id = Guid.Empty;
   }
   
   public override Guid Id { get => _id; }
}