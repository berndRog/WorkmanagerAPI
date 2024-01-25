using System.Runtime.CompilerServices;
using Workmanager.Api.Core;

[assembly: InternalsVisibleToAttribute("Workmanager.ApiTest")]
namespace Workmanager.Api.Persistence.Repositories;
internal class DataContext(
   AppDbContext dbContext
) : IDataContext{
   
   public Task<bool> SaveAllChangesAsync() => dbContext.SaveAllChangesAsync();

   public void ClearChangeTracker() => dbContext.ClearChangeTracker();

   public void LogChangeTracker(string text) => dbContext.LogChangeTracker(text);
}
