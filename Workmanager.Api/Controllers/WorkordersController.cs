using System.Net.Mime;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Workmanager.Api.Core;
using Workmanager.Api.Core.DomainModel.Entities;
using Workmanager.Api.Core.Dto;
namespace Workmanager.Api.Controllers;

[ApiVersion("1.0")]
[Route("workmanagerapi/v{version:apiVersion}")]

[ApiController]
public class WorkordersController(
   IWorkordersRepository repository,
   IPeopleRepository peopleRepository,
   IDataContext dataContext,
   IMapper mapper,
   ILogger<WorkordersController> logger
) : ControllerBase {

 
   #region methods
   /// <summary>
   /// Get all workorders 
   /// </summary>
   /// <returns>IEnumerable{WorkorderDto}; </returns>
   /// <response code="200">Ok. List{WorkorderDto} returned</response>
   [HttpGet("workorders")]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesDefaultResponseType]
   public async Task<ActionResult<IEnumerable<WorkorderDto>>> Get() {
      logger.LogDebug("GetAll()");
      IEnumerable<Workorder> workorders = await repository.SelectAsync();
      return Ok(mapper.Map<IEnumerable<WorkorderDto>>(workorders));
   }
   
   /// <summary>
   /// Find the workorder with the given id 
   /// </summary>
   /// <param name="id:Guid">id of the workorder</param>
   /// <returns>WorkorderDto?</returns>
   /// <response code="200">Ok: Workorder with given id returned</response>
   /// <response code="404">NotFound: Workorder with given id not found</response>
   /// <response code="500">Server internal error.</response>
   [HttpGet("workorders/{id:Guid}")]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesResponseType(StatusCodes.Status404NotFound)]
   [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
   [ProducesDefaultResponseType]
   public async Task<ActionResult<WorkorderDto>> GetById(
      [FromRoute] Guid id
   ) {
      logger.LogDebug("GetById {id}",id);
      Workorder? workorder = await repository.FindByIdAsync(id);
      if(workorder == null) return NotFound("Workorder with given id not found");
      return Ok(mapper.Map<WorkorderDto>(workorder));        
   }
   
   /// <summary>
   /// Get workorders of a person with personId 
   /// </summary>
   /// <returns>IEnumerable{WorkorderDto}; </returns>
   /// <response code="200">Ok. Tranfers returned</response>
   [HttpGet("people/{personId}/workorders")]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesDefaultResponseType]
   public async Task<ActionResult<IEnumerable<WorkorderDto>>> Get(
      [FromRoute] Guid personId
   ){
      logger.LogDebug("Get by personId {PersonId}", personId);
      var person = await peopleRepository.FindByIdAsync(personId);
      if(person == null)
         return BadRequest("Bad request: personId does not exist.");

      var workorders =
         await repository.SelectByPersonIdAsync(personId);
      return Ok(mapper.Map<IEnumerable<WorkorderDto>>(workorders));
   }
  
   /// <summary>
   /// Insert a Workorder
   /// </summary>
   /// <param name="WorkorderDto"></param>
   /// <returns>WorkorderDto?</returns>
   /// <response code="201">Created: Workorder is created</response>
   /// <response code="409">Conflict: Workorder with given id already exists.</response>
   [HttpPost("workorders")]
   [Consumes(MediaTypeNames.Application.Json)]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status201Created)]
   [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
   [ProducesDefaultResponseType]
   public async Task<ActionResult<WorkorderDto>> Post(
      [FromBody] WorkorderDto workorderDto
   ) {
      logger.LogDebug("Post");
      var workorder = mapper.Map<Workorder>(workorderDto);

      if (workorder.Id == Guid.Empty) workorder.Id = Guid.NewGuid();
      if (await repository.FindByIdAsync(workorder.Id) != null) 
         return Conflict($"Post: Workorder with given id already exists");

      // save to repository and write to database 
      repository.Add(workorder);
      await dataContext.SaveAllChangesAsync();
      
      // https://ochzhen.com/blog/created-createdataction-createdatroute-methods-explained-aspnet-core
      // Request == null in unit tests
      var path = Request == null 
         ? $"/banking/v1/workorders/{workorder.Id}" 
         : $"{Request.Path}/{workorder.Id}";
      var uri = new Uri(path, UriKind.Relative);
      workorderDto = mapper.Map<WorkorderDto>(workorder);
      return Created(uri: uri, value: workorderDto);
   }

   /// <summary>
   /// Update a workorder (workorder with id must exist)
   /// </summary>
   /// <param name="id:Guid">Given id</param>
   /// <param name="updWorkordereDto">Workorder with new properties</param>
   /// <returns>WorkorderDto?</returns>
   /// <response code="200">Ok: Workorder with given id updated.</response>
   /// <response code="400">Bad Request: id and Workorder.Id do not match.</response>
   [HttpPut("workorders/{id:Guid}")]
   [Consumes(MediaTypeNames.Application.Json)]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesResponseType(StatusCodes.Status201Created)]
   [ProducesResponseType(StatusCodes.Status400BadRequest)]
   [ProducesDefaultResponseType]
   public async Task<ActionResult<WorkorderDto>> Put(
      [FromRoute] Guid id,
      [FromBody]  WorkorderDto updWorkorderDto
   ) {
      logger.LogDebug("Put {id}", id);

      Workorder updWorkorder = mapper.Map<Workorder>(updWorkorderDto);
      
      Workorder? workorder = await repository.FindByIdAsync(id);
      if (workorder == null)
         return NotFound($"Put: Workorder with given id not found.");
      
      // Update workorder
      workorder!.Copy(updWorkorder);
      
      // save to repository and write to database 
      await repository.UpdateAsync(workorder);
      await dataContext.SaveAllChangesAsync();

      return Ok(mapper.Map<WorkorderDto>(workorder));
   }

   /// <summary>
   /// Delete workorder with given id
   /// </summary>
   /// <param name="id"></param>
   /// <returns></returns>
   /// <response code="204">NoContent: Workorder deleted.</response>
   /// <response code="404">NotFound: Workorder with given id not found</response>
   [HttpDelete("workorders/{id:Guid}")]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status204NoContent)]
   [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
   [ProducesDefaultResponseType]
   public async Task<IActionResult> Delete(
      [FromRoute] Guid id
   ) {
      logger.LogDebug("Delete {id}",id);

      Workorder? workorder = await repository.FindByIdAsync(id);
      if(workorder == null) return NotFound($"Delete: Workorder with given id not found");
      repository.Remove(workorder);
      await dataContext.SaveAllChangesAsync();
      return NoContent();  // 204 = Ok with no content
   }
   #endregion
}