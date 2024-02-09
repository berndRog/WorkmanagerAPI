using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Workmanager.Api.Controllers;
using Workmanager.Api.Core;
using Workmanager.Api.Core.Dto;
using Workmanager.Api.Persistence;
using Workmanager.ApiTest.Persistence;
using Workmanager.ApiTest.Di;

namespace Workmanager.ApiTest.Controllers;
[Collection(nameof(SystemTestCollectionDefinition))]
public class WorkordersControllerTest {
   private readonly WorkordersController _workordersController;  
   private readonly IPeopleRepository _peopleRepository;
   private readonly IWorkordersRepository _workordersRepository;
   private readonly IDataContext _dataContext;
   private readonly ArrangeWorkmanagerTest _arrangeTest;
   private readonly IMapper _mapper;
   private readonly Seed _seed;

   public WorkordersControllerTest(){
      
      IServiceCollection serviceCollection = new ServiceCollection();
      serviceCollection.AddPersistenceTest();
      serviceCollection.AddControllersTest();
      ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider() 
         ?? throw new Exception("Failed to build Serviceprovider");

      AppDbContext dbContext = serviceProvider.GetRequiredService<AppDbContext>()
         ?? throw new Exception("Failed to create an instance of AppDbContext");
      dbContext.Database.EnsureDeleted();
      dbContext.Database.EnsureCreated();
      
      _dataContext = serviceProvider.GetRequiredService<IDataContext>() 
         ?? throw new Exception("Failed to create an instance of IDataContext");
      
      _peopleRepository = serviceProvider.GetRequiredService<IPeopleRepository>()
         ?? throw new Exception("Failed create an instance of IPeopleRepository");
      _workordersRepository = serviceProvider.GetRequiredService<IWorkordersRepository>()
         ?? throw new Exception("Failed create an instance of IWorkordersRepository");
      
      _workordersController = serviceProvider.GetRequiredService<WorkordersController>()
         ?? throw new Exception("Failed to create an instance of WorkordersController");      

      _arrangeTest = serviceProvider.GetRequiredService<ArrangeWorkmanagerTest>()
         ?? throw new Exception("Failed create an instance of CArrangeTest");
      _mapper = serviceProvider.GetRequiredService<IMapper>();
      _seed = new Seed();
   }
   [Fact]
   public async Task GetAsyncTest() {
      // Arrange
      await _arrangeTest.Person1WithWorkorder1Async(_seed);
      List<WorkorderDto> expected = _mapper.Map<List<WorkorderDto>>(_seed.Workorders); 
      // Act
      ActionResult<IEnumerable<WorkorderDto>> response = await _workordersController.Get();
      _dataContext.LogChangeTracker("GetAllTest()");
      // Assert
      OkObjectResult result = 
          TestHelper.ResultFromResponse<OkObjectResult, IEnumerable<WorkorderDto>>(response);
      result.StatusCode.Should().Be(200);
      (result.Value as List<WorkorderDto>).Should()
         .NotBeNull().And
         .BeEquivalentTo(expected);
   }
   [Fact]
   public async Task GetByIdTest() {
      // Arrange
      await _arrangeTest.Person1WithWorkorder1Async(_seed);
      WorkorderDto workorderDto = _mapper.Map<WorkorderDto>(_seed.Person1); 
      // Act
      ActionResult<WorkorderDto> response = 
         await _workordersController.GetById(_seed.Workorder01.Id);
      _dataContext.LogChangeTracker("GetByIdTest()");
      // Assert
      OkObjectResult result = 
         TestHelper.ResultFromResponse<OkObjectResult, WorkorderDto>(response);
      result.StatusCode.Should().Be(200);
      (result.Value as WorkorderDto).Should()
         .NotBeNull().And
         .BeEquivalentTo<WorkorderDto>(workorderDto);
   }
   
   [Fact]
   public async Task PostTest() {
      // Arrange
      await _arrangeTest.Person1WithWorkorder1Async(_seed);
      WorkorderDto workorderDto = _mapper.Map<WorkorderDto>(_seed.Workorder01); 
      // Act
      ActionResult<WorkorderDto> response = await _workordersController.Post(workorderDto);
      // Assert
      CreatedResult result = 
         TestHelper.ResultFromResponse<CreatedResult, WorkorderDto>(response);
      result.StatusCode.Should().Be(201);
      (result.Value as WorkorderDto).Should()
         .NotBeNull().And
         .BeEquivalentTo(workorderDto);
   }
   /*
   [Fact]
   public async Task PutTest() {
      // Arrange
      await _arrangeTest.PeopleAsync(_seed);
      // Act
      PersonDto person8Dto = _mapper.Map<PersonDto>(_seed.Person8);
      person8Dto.LastName = "Hoffman-Meier";
      person8Dto.Email = "hanna.hoffman-meier@t-online.de";
      // Act
      ActionResult<PersonDto> response
         = await _workordersController.Put(person8Dto.Id, person8Dto);
      // Assert
      OkObjectResult result =
         TestHelper.ResultFromResponse<OkObjectResult, PersonDto>(response);
      result.StatusCode.Should().Be(200);
      (result.Value as PersonDto).Should()
         .NotBeNull().And
         .BeEquivalentTo(person8Dto);  
   }
*/
}