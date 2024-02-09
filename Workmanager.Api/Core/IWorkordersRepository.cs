using Workmanager.Api.Core.DomainModel.Entities;
namespace Workmanager.Api.Core;

public interface IWorkordersRepository : IGenericRepository<Workorder> {
   
   Task<IEnumerable<Workorder>> SelectByPersonIdAsync(Guid personId);
   
}