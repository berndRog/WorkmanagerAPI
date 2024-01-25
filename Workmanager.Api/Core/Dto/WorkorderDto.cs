using Workmanager.Api.Core.Enums;

namespace Workmanager.Api.Core.Dto;
/// <summary>
/// WorkorderDto (Arbeitsauftrag)
/// </summary>
public class WorkorderDto {
   public Guid Id { get; set; } = Guid.Empty;
   public string Title { get; set; } = string.Empty;
   public string Description { get; set; } = string.Empty;
   public DateTime Created { get; set; } = DateTime.UtcNow;
   public DateTime Started { get; set; } = DateTime.UtcNow;
   public DateTime Completed { get; set; } = DateTime.UtcNow;
   public long Duration { get; set; } = 0L;
   public string Remark { get; set; } = string.Empty;
   public WorkorderState State { get; set; } = WorkorderState.Default;
   
   public Guid? PersonId { get; set; } = null;
   public Guid? AddressId { get; set; } = null;

}
