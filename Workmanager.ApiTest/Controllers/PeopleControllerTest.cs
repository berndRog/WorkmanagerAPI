using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Workmanager.Api.Controllers;
using Workmanager.Api.Core;
using Workmanager.Api.Core.DomainModel.Entities;
using Workmanager.Api.Core.Dto;
using Workmanager.Api.Persistence;
using Workmanager.ApiTest.Persistence;
using Workmanager.ApiTest.Di;

namespace Workmanager.ApiTest.Controllers;
[Collection(nameof(SystemTestCollectionDefinition))]
public class PeopleControllerTest {
   private readonly PeopleController _peopleController;  
   private readonly IPeopleRepository _peopleRepository;
   private readonly IDataContext _dataContext;
   private readonly ArrangeWorkmanagerTest _arrangeTest;
   private readonly IMapper _mapper;
   private readonly Seed _seed;

   public PeopleControllerTest(){
      
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
      
      _peopleController = serviceProvider.GetRequiredService<PeopleController>()
         ?? throw new Exception("Failed to create an instance of PeopleController");      

      _arrangeTest = serviceProvider.GetRequiredService<ArrangeWorkmanagerTest>()
         ?? throw new Exception("Failed create an instance of CArrangeTest");
      _mapper = serviceProvider.GetRequiredService<IMapper>();
      _seed = new Seed();
   }
   [Fact]
   public async Task GetAsyncTest() {
      // Arrange
      await _arrangeTest.PeopleAsync(_seed);
      List<PersonDto> expected = _mapper.Map<List<PersonDto>>(_seed.People); 
      // Act
      ActionResult<IEnumerable<PersonDto>> response = 
         await _peopleController.Get();
      _dataContext.LogChangeTracker("GetAllTest()");
      // Assert
      OkObjectResult result = 
          TestHelper.ResultFromResponse<OkObjectResult, IEnumerable<PersonDto>>(response);
      result.StatusCode.Should().Be(200);
      (result.Value as List<PersonDto>).Should()
         .NotBeNull().And
         .BeEquivalentTo(expected);
   }
   [Fact]
   public async Task GetByIdTest() {
      // Arrange
      await _arrangeTest.PeopleAsync(_seed);
      PersonDto personDto = _mapper.Map<PersonDto>(_seed.Person1); 
      // Act
      ActionResult<PersonDto> response = 
         await _peopleController.GetById(_seed.Person1.Id);
      _dataContext.LogChangeTracker("GetByIdTest()");
      // Assert
      OkObjectResult result = 
         TestHelper.ResultFromResponse<OkObjectResult, PersonDto>(response);
      result.StatusCode.Should().Be(200);
      (result.Value as PersonDto).Should()
         .NotBeNull().And
         .BeEquivalentTo<PersonDto>(personDto);
   }
   
   [Fact]
   public async Task PostTest() {
      // Arrange
      PersonDto personDto = _mapper.Map<PersonDto>(_seed.Person1); 
      // Act
      ActionResult<PersonDto> response = await _peopleController.Post(personDto);
      // Assert
      CreatedResult result = 
         TestHelper.ResultFromResponse<CreatedResult, PersonDto>(response);
      result.StatusCode.Should().Be(201);
      (result.Value as PersonDto).Should()
         .NotBeNull().And
         .BeEquivalentTo(personDto);
   }
   
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
         = await _peopleController.Put(person8Dto.Id, person8Dto);
      // Assert
      OkObjectResult result =
         TestHelper.ResultFromResponse<OkObjectResult, PersonDto>(response);
      result.StatusCode.Should().Be(200);
      (result.Value as PersonDto).Should()
         .NotBeNull().And
         .BeEquivalentTo(person8Dto);  
   }

}