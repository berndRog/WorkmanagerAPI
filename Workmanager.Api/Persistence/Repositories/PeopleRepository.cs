using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Workmanager.Api.Core;
using Workmanager.Api.Core.DomainModel.Entities;
using Workmanager.Api.Core.Misc;
[assembly: InternalsVisibleTo("Workmanager.ApiTest")]
namespace Workmanager.Api.Persistence.Repositories;

internal class PeopleRepository(
   AppDbContext dbContext,
   ILogger<PeopleRepository> logger
) : AGenericRepository<Person>(dbContext), IPeopleRepository {
   
   #region local methods
   /// <summary>
   /// Join Person with Workorders
   /// </summary>
   /// <param name="withTracking">true: use Tracking, i.e. People are added to the repository </param>
   /// <param name="joinWorkorders">true: join People with Workorders</param>
   /// <param name="query">IQueryable<Person></param>
   /// <returns>IQueryable<Person></returns>
   private static IQueryable<Person> Join(
      bool withTracking,
      bool joinWorkorders,
      IQueryable<Person> query
   ) {
      if(!withTracking) query = query.AsNoTracking();  
      if (joinWorkorders) query = query.Include(p => p.Workorders)
         .AsSplitQuery();
      return query;
   }
   #endregion

   #region methods    
   /// <summary>
   /// Select all People with Workorders asynchronously
   /// </summary>
   /// <param name="withTracking">true: use Tracking, i.e. people are added to the repository </param>
   /// <param name="joinWorkorders">true: Join People with Workorders</param>
   /// <returns>IEnumerable<Person></returns>
   public async Task<IEnumerable<Person>> SelectJoinAsync(
      bool withTracking = false,
      bool joinWorkorders = false
   ) {
      logger.LogDebug($"SelectAsync People joinWorkorders={joinWorkorders}");
      IQueryable<Person> query = TypeDbSet;
      query = Join(withTracking, joinWorkorders, query);
      return await query
         .ToListAsync();
   }
   
   /// <summary>
   /// Find a Person by Id with Workorders asynchronously
   /// </summary>
   /// <param name="id"></param>
   /// <param name="joinWorkorders">true: join People with Workorders</param>
   /// <returns>Person?</returns>
   public async Task<Person?> FindByIdJoinAsync(
      Guid id, 
      bool joinWorkorders = false
   ) {
      logger.LogDebug($"FindByIdAsync Person {id.As8()} joinWorkorders={joinWorkorders}");
      IQueryable<Person> query = TypeDbSet;
      query = Join(true, joinWorkorders, query);
      return await query
         .FirstOrDefaultAsync(p => p.Id == id);
   }
   
   /// <summary>
   /// Find a Person by LINQ expression with Address and/or Workorders asynchronously
   /// </summary>
   /// <param name="predicate"></param>
   /// <param name="joinWorkorders">true: join People with Workorders</param>
   /// <returns>Person?</returns>
   public async Task<Person?> FindByJoinAsync(
      Expression<Func<Person, bool>> predicate,
      bool joinWorkorders = false
   ) {
      logger.LogDebug($"FindByAsync Person joinWorkorders={joinWorkorders}");

      IQueryable<Person> query = TypeDbSet;
      query = Join(true, joinWorkorders, query);
      return await query
         .FirstOrDefaultAsync(predicate);
   }
   #endregion
}