using System.Linq.Expressions;
using Workmanager.Api.Core.DomainModel.Entities;
using Workmanager.Api.Persistence;

namespace Workmanager.Api.Core; 

public interface IGenericRepository<T> where T : ABaseEntity {

   T? Attach(T item);
   
   // read from database?
   Task<IEnumerable<T>> SelectAsync  (bool withTracking = false);
   Task<T?>             FindByIdAsync(Guid id);
//                                   LINQ expression with lamdba 
   Task<IEnumerable<T>> FilterAsync  (Expression<Func<T, bool>> p);
   Task<T?>             FindByAsync  (Expression<Func<T, bool>> predicate); 
   
   // write to in-memory repository
   void                 Add          (T item);
   void                 AddRange     (IEnumerable<T> items);
   Task                 UpdateAsync  (T item);
   void                 Remove       (T item);
   
   // write to database
//   Task<bool>           SaveChangesAsync();
//   void                 ClearChangeTracker();
   


}