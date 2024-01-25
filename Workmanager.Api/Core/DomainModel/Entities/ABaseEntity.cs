﻿
namespace Workmanager.Api.Core.DomainModel.Entities;
[Serializable]
public abstract class ABaseEntity {
   
   #region properties
   public abstract Guid     Id      { get; set; }

   #endregion

   #region ctor
   protected ABaseEntity() {}
   #endregion

}