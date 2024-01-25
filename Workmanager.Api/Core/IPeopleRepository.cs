using System.Linq.Expressions;
using Workmanager.Api.Core.DomainModel.Entities;
namespace Workmanager.Api.Core;

public interface IPeopleRepository : IGenericRepository<Person> {
   
   /// <summary>
   /// Select all People with Address and/or Workorders asynchronously
   /// </summary>
   /// <param name="withTracking">true: use Tracking, i.e. people are added to the repository </param>
   /// <param name="joinAddress">true: Join People with Address</param>
   /// <param name="joinWorkorders">true: Join People with Workorders</param>
   Task<IEnumerable<Person>> SelectJoinAsync(
      bool withTracking = false,
      bool joinAddress = false,
      bool joinWorkorders = false
   );

   /// <summary>
   /// Find a Person by Id with Address and/or Workorders asynchronously
   /// </summary>
   /// <param name="id"></param>
   /// <param name="joinAddress">true: Join People with Address</param>
   /// <param name="joinWorkorders">true: join People with Workorders</param>
   /// <returns>Person?</returns>
   Task<Person?> FindByIdJoinAsync(
      Guid id,
      bool joinAddress = false,
      bool joinWorkorders = false
   );

   /// <summary>
   /// Find a Person by LINQ expression with Address and/or Workorders asynchronously
   /// </summary>
   /// <param name="predicate"></param>
   /// <param name="joinAddress">true: join People with Address</param>
   /// <param name="joinWorkorders">true: join People with Workorders</param>
   /// <returns>Person?</returns>
   Task<Person?> FindByJoinAsync(
      Expression<Func<Person, bool>> predicate,
      bool joinAddress = false,
      bool joinWorkorders = false
   );
}