using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Workmanager.Api.Core;
using Workmanager.Api.Core.DomainModel.Entities;
using Workmanager.Api.Core.Misc;
using Workmanager.Api.Persistence;
using Workmanager.ApiTest.Di;

namespace Workmanager.ApiTest.Persistence.Repositories;
[Collection(nameof(SystemTestCollectionDefinition))]
public  class WorkordersRepositoryUt {
   private readonly IWorkordersRepository _workordersRepository;
   private readonly IPeopleRepository _peopleRepository;
   private readonly IDataContext _dataContext;
   private readonly ArrangeWorkmanagerTest _arrangeTest;
   private readonly Seed _seed;
   
   public WorkordersRepositoryUt(){      
      IServiceCollection services = new ServiceCollection();
      services.AddPersistenceTest();
      ServiceProvider serviceProvider = services.BuildServiceProvider()
         ?? throw new Exception("Failed to create an instance of ServiceProvider");      
      
      //-- Service Locator    
      AppDbContext dbContext = serviceProvider.GetRequiredService<AppDbContext>()
                            ?? throw new Exception("Failed to create AppDbContext");
      dbContext.Database.EnsureDeleted(); 
      dbContext.Database.EnsureCreated();

      _dataContext = serviceProvider.GetRequiredService<IDataContext>() 
         ?? throw new Exception("Failed to create an instance of IDataContext");

      _peopleRepository = serviceProvider.GetRequiredService<IPeopleRepository>()
         ?? throw new Exception("Failed create an instance of IPeopleRepository");
      _workordersRepository = serviceProvider.GetRequiredService<IWorkordersRepository>()
         ?? throw new Exception("Failed create an instance of IWorkordersRepository");
      
      _arrangeTest = serviceProvider.GetRequiredService<ArrangeWorkmanagerTest>()
         ?? throw new Exception("Failed create an instance of ArrangeTest");

      _seed = new Seed();
   }
   [Fact]
   public async Task SelectAsyncUt() {
      // Arrange
      await _arrangeTest.PeopleWithWorkordersAsync(_seed);
      // Act  with tracking
      IEnumerable<Workorder> actual = await _workordersRepository.SelectAsync();   
      _dataContext.LogChangeTracker("SelectAsync()");
      // Assert
      actual.Should()
         .NotBeNull().And
         .NotBeEmpty().And
         .HaveCount(10).And
         .BeEquivalentTo(_seed.Workorders, options => 
            options.Excluding(w => w.Person)
         );   
   }

   [Fact]
   public async Task FilterByLastNameUt() {
      // Arrange
      await _arrangeTest.PeopleAsync(_seed);
      IEnumerable<Person> expected = new List<Person>{_seed.Person5 };
      // Act
      IEnumerable<Person> actual = 
         await _peopleRepository.FilterAsync(p => p.LastName.Contains("Engel"));
      _dataContext.LogChangeTracker("FilterAsync by LastName()");
      // Assert
      actual.Should()
         .NotBeNull().And
         .HaveCount(1).And
         .BeEquivalentTo(expected);
   }
   [Fact]
   public async Task FilterByEmailUt() {
      // Arrange
      await _arrangeTest.PeopleAsync(_seed);
      List<Person> expected = new List<Person>{ _seed.Person2, _seed.Person7, _seed.Person10 };
      // Act
      IEnumerable<Person> actual = 
         await _peopleRepository.FilterAsync(p => 
            p.Email != null && p.Email.Contains("gmx"));
      _dataContext.LogChangeTracker("FilterByEmail");
      // Assert
      actual.Should()
         .NotBeNull().And
         .HaveCount(3).And
         .BeEquivalentTo(expected);
   }
   [Fact]
   public async Task FindByIdAsyncUt() {
      // Arrange
      await _arrangeTest.PeopleAsync(_seed);
      // Act
      Person? actual = await _peopleRepository.FindByIdAsync(_seed.Person1.Id);
      // Assert
      _dataContext.LogChangeTracker("FindByIdAsync");
      actual.Should()
         .NotBeNull().And
         .BeEquivalentTo(_seed.Person1);
   }
   
   [Fact]
   public async Task FindByNameUt() {
      // Arrange
      await _arrangeTest.PeopleAsync(_seed);
      Person expected = _seed.Person5;
      // Act
      Person? actual = 
         await _peopleRepository.FindByAsync(p => p.LastName.Contains("Engel"));   
      // Assert
      _dataContext.LogChangeTracker("FindByIdName");
      actual.Should()
         .NotBeNull().And
         .BeEquivalentTo(expected);
   }
   [Fact]
   public async Task FindByEmailUt() {
      // Arrange
      await _arrangeTest.PeopleAsync(_seed);
      Person expected = _seed.Person8;
      expected.Email.Should().NotBeNull();
      
      // Act
      Person? actual =
         await _peopleRepository.FindByAsync(
            p => p.Email != null && p.Email.Contains(expected.Email!));
      // Assert
      _dataContext.LogChangeTracker("FindByEmail");
      actual.Should().NotBeNull()
         .And.BeEquivalentTo(expected);
   }
   [Fact]
   public async Task AddUt() {
      // Arrange
      Person person = _seed.Person1;
      Workorder workorder = _seed.Workorder01;
      person.AndOrUpdateWorkorder(workorder);
      // Act
//      _workordersRepository.Add(workorder);
      _workordersRepository.Add(workorder);
      await _dataContext.SaveAllChangesAsync();
      _dataContext.ClearChangeTracker();
      // Assert
      Workorder? actual = await _workordersRepository.FindByIdAsync(workorder.Id);
      _dataContext.LogChangeTracker("FindByid");
      actual.Should().BeEquivalentTo(workorder, options => options.Excluding(w => w.Person));
   }
   
   [Fact]
   public async Task AddRangeUt() {
      // Arrange
      IEnumerable<Workorder> expected = _seed.Workorders;
      // Act
      _workordersRepository.AddRange(_seed.Workorders);
      await _dataContext.SaveAllChangesAsync();
      // Assert       
      IEnumerable<Workorder> actual = await _workordersRepository.SelectAsync();   
      actual.Should().NotBeNull()
         .And.NotBeEmpty()
         .And.HaveCount(10)
         .And.BeEquivalentTo(expected);
   }
   
   [Fact]
   public async Task UpdateUt() {
      // Arrange
      await _arrangeTest.PeopleAsync(_seed);
      Person expected = _seed.Person8;
      // Act
      Person updPerson = new Person {
         Id = expected.Id,
         FirstName = expected.FirstName,
         LastName = "Hoffman-Meier",
         Email = "hanna.hoffman-meier@t-online.de",
         Phone = expected.Phone
      };
      await _peopleRepository.UpdateAsync(updPerson);
      await _dataContext.SaveAllChangesAsync();
      // Assert
      Person? actual = await _peopleRepository.FindByIdAsync(updPerson.Id);
      _dataContext.LogChangeTracker("find updated Person");
      actual.Should().BeEquivalentTo(updPerson);
   }


}