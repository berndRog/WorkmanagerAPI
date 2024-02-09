using Workmanager.Api.Core;
using Workmanager.Api.Core.DomainModel.Entities;
using Workmanager.Api.Core.Misc;

namespace Workmanager.ApiTest.Persistence;
public class ArrangeWorkmanagerTest(
   IPeopleRepository peopleRepository,
   IWorkordersRepository workordersRepository,
   IDataContext dataContext
) {

   public async Task Person1Async(Seed seed) {
      peopleRepository.Add(seed.Person1);
      await dataContext.SaveAllChangesAsync();
      dataContext.ClearChangeTracker();
   }
   
   
   public async Task Person1WithWorkorder1Async(Seed seed){
      seed.Person1.AndOrUpdateWorkorder(seed.Workorder01);
      peopleRepository.Add(seed.Person1);
      workordersRepository.Add(seed.Workorder01);
      await dataContext.SaveAllChangesAsync();
   }

   public async Task PeopleAsync(Seed seed) {
      peopleRepository.AddRange(seed.People);
      await dataContext.SaveAllChangesAsync();
      dataContext.ClearChangeTracker();
   }


   public async Task PeopleWithWorkordersAsync(Seed seed) {
      seed.InitPeopleWithWordorders();
      peopleRepository.AddRange(seed.People);
      await dataContext.SaveAllChangesAsync();
      dataContext.ClearChangeTracker();
   }



}