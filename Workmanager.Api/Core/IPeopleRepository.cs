using System.Linq.Expressions;
using Workmanager.Api.Core.DomainModel.Entities;
namespace Workmanager.Api.Core;

public interface IPeopleRepository : IGenericRepository<Person> {
   
   Task<IEnumerable<Person>> SelectJoinAsync(
      bool withTracking = false,
      bool joinWorkorders = false
   );

   Task<Person?> FindByIdJoinAsync(
      Guid id,
      bool joinWorkorders = false
   );

   Task<Person?> FindByJoinAsync(
      Expression<Func<Person, bool>> predicate,
      bool joinWorkorders = false
   );
}