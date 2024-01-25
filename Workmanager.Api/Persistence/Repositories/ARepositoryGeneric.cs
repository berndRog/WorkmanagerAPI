﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Workmanager.Api.Core;
using Workmanager.Api.Core.DomainModel.Entities;

[assembly: InternalsVisibleToAttribute("Workmanager.ApiTest")]
namespace Workmanager.Api.Persistence.Repositories;

internal abstract class AGenericRepository<T> : IGenericRepository<T>
   where T : ABaseEntity {

   #region fields
   private readonly AppDbContext _dbContext;
   protected readonly DbSet<T> TypeDbSet;
   #endregion

   #region properties
   public AppDbContext DatabaseContext {
      get => _dbContext;
   }
   #endregion
   
   #region ctor
   protected AGenericRepository(
      AppDbContext dbContext
   ){
      _dbContext = dbContext;
      TypeDbSet = _dbContext.Set<T>();
   }
   #endregion

   #region methods
   /// <summary>
   /// Select all items asynchronously
   /// </summary>
   /// <param name="withTracking">== false -> NoTracking, i.e. items are not loaded into the repository</param>
   /// <returns>IEnumerable<T></returns>
   public virtual async Task<IEnumerable<T>> SelectAsync(bool withTracking = false) {
      IQueryable<T> query = TypeDbSet;
      if(!withTracking) query = query.AsNoTracking();
      return await query.ToListAsync(); 
   }

   /// <summary>
   /// Find an item by Id asynchronously
   /// </summary>
   /// <param name="id">Guid</param>
   /// <returns>T?</returns>
   public virtual async Task<T?> FindByIdAsync(Guid id) =>
      await TypeDbSet.FirstOrDefaultAsync(item => item.Id == id);
   
   /// <summary>
   /// Filter items by LINQ expression asynchronously
   /// </summary>
   /// <param name="p">LINQ expression tree used as filter</param>
   /// <returns>IEnumerable</returns>
   public virtual async Task<IEnumerable<T>> FilterAsync(Expression<Func<T, bool>> p) =>
      await TypeDbSet.Where(p).ToListAsync();

   /// <summary>
   /// Find an item by LINQ expression asynchronously
   /// </summary>
   /// <param name="predicate">LINQ expression tree used as filter</param>
   /// <returns>T?</returns>
   public virtual async Task<T?> FindByAsync(Expression<Func<T, bool>> predicate) =>
      await TypeDbSet.FirstOrDefaultAsync(predicate);

   /// <summary>
   /// Write an item to the repository
   /// </summary>
   /// <param name="item">item to add</param>
   public void Add(T item) =>
      TypeDbSet.Add(item);

   /// <summary>
   /// Add a range of items to the repository
   /// </summary>
   /// <param name="items">items to add</param>
   public virtual void AddRange(IEnumerable<T> items) =>
      TypeDbSet.AddRange(items);

   /// <summary>
   /// Update an exiting item asynchronously, item with item.id must exist
   /// </summary>
   /// <param name="item">Item to update</param>
   /// <exception cref="ApplicationException">item with given id not found</exception>
   public async Task UpdateAsync(T item){
      var foundItem = await TypeDbSet.FirstOrDefaultAsync(i => i.Id == item.Id)
         ?? throw new ApplicationException($"Update failed, item not found");
      _dbContext.Entry(foundItem).CurrentValues.SetValues(item);
      _dbContext.Entry(foundItem).State = EntityState.Modified;
   }
   
   /// <summary>
   /// Remove an item from the repository
   /// </summary>
   /// <param name="item">item to remove</param>
   public virtual void Remove(T item){
      var entityEntry = TypeDbSet.Remove(item);
   }

   /// <summary>
   /// Attach an item to the repository
   /// </summary>
   /// <param name="item">item to attach</param>
   /// <returns>T? the attached item</returns>
   public T? Attach(T item) {
      EntityEntry<T> entityEntry = _dbContext.Attach<T>(item);
      return entityEntry.Entity;
   }
   #endregion
}
