using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Workmanager.Api.Core;
using Workmanager.Api.Core.DomainModel.Entities;

[assembly: InternalsVisibleTo("Workmanager.ApiTest")]
namespace Workmanager.Api.Persistence.Repositories;

internal class WorkordersRepository(
   AppDbContext dbContext
) : AGenericRepository<Workorder>(dbContext), IWorkordersRepository {
   
   public async Task<IEnumerable<Workorder>> SelectByPersonIdAsync(Guid personId) {
      return await TypeDbSet
         .Where(w => w.PersonId == personId)
         .ToListAsync();
   }

}