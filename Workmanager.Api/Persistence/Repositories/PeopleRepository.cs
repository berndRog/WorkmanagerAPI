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
   /// Join Person with Address and/or Workorders
   /// </summary>
   /// <param name="withTracking">true: use Tracking, i.e. People are added to the repository </param>
   /// <param name="joinAddress">true: Join People with Address</param>
   /// <param name="joinWorkorders">true: join People with Workorders</param>
   /// <param name="query">IQueryable<Person></param>
   /// <returns>IQueryable<Person></returns>
   private static IQueryable<Person> Join(
      bool withTracking,
      bool joinAddress,
      bool joinWorkorders,
      IQueryable<Person> query
   ) {
      if(!withTracking) query = query.AsNoTracking();  
      if (joinAddress) query = query.Include(p=> p.Address)
         .AsSingleQuery();
      if (joinWorkorders) query = query.Include(p => p.Workorders)
         .AsSplitQuery();
      return query;
   }
   #endregion

   #region methods    
   /// <summary>
   /// Select all People with Address and/or Workorders asynchronously
   /// </summary>
   /// <param name="withTracking">true: use Tracking, i.e. people are added to the repository </param>
   /// <param name="joinAddress">true: Join People with Address</param>
   /// <param name="joinWorkorders">true: Join People with Workorders</param>
   /// <returns>IEnumerable<Person></returns>
   public async Task<IEnumerable<Person>> SelectJoinAsync(
      bool withTracking = false,
      bool joinAddress = false,
      bool joinWorkorders = false
   ) {
      logger.LogDebug($"SelectAsync People joinOwner={joinAddress} joinWorkorders={joinWorkorders}");
      IQueryable<Person> query = TypeDbSet;
      query = Join(withTracking, joinAddress, joinWorkorders, query);
      return await query
         .ToListAsync();
   }
   
   /// <summary>
   /// Find a Person by Id with Address and/or Workorders asynchronously
   /// </summary>
   /// <param name="id"></param>
   /// <param name="joinAddress">true: Join People with Address</param>
   /// <param name="joinWorkorders">true: join People with Workorders</param>
   /// <returns>Person?</returns>
   public async Task<Person?> FindByIdJoinAsync(
      Guid id, 
      bool joinAddress = false,
      bool joinWorkorders = false
   ) {
      logger.LogDebug($"FindByIdAsync Person {id.As8()} joinAddress={joinAddress} joinWorkorders={joinWorkorders}");
      IQueryable<Person> query = TypeDbSet;
      query = Join(true, joinAddress, joinWorkorders, query);
      return await query
         .FirstOrDefaultAsync(p => p.Id == id);
   }
   
   /// <summary>
   /// Find a Person by LINQ expression with Address and/or Workorders asynchronously
   /// </summary>
   /// <param name="predicate"></param>
   /// <param name="joinAddress">true: join People with Address</param>
   /// <param name="joinWorkorders">true: join People with Workorders</param>
   /// <returns>Person?</returns>
   public async Task<Person?> FindByJoinAsync(
      Expression<Func<Person, bool>> predicate,
      bool joinAddress = false,
      bool joinWorkorders = false
   ) {
      logger.LogDebug($"FindByAsync Person joinAddress={joinAddress} joinWorkorders={joinWorkorders}");

      IQueryable<Person> query = TypeDbSet;
      query = Join(true, joinAddress, joinWorkorders, query);
      return await query
         .FirstOrDefaultAsync(predicate);
   }
   #endregion
}