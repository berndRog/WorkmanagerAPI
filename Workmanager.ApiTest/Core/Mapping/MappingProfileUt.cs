using AutoMapper;
using FluentAssertions;
using Workmanager.Api.Core.DomainModel.Entities;
using Workmanager.Api.Core.Dto;
using Workmanager.Api.Core.Mapping;

namespace Workmanager.ApiTest.Core.Mapping;

public class MappingProfileUt {
   
   private readonly Seed _seed;
   private readonly IMapper _mapper;

   public MappingProfileUt() {
      var config = new MapperConfiguration(config =>
         config.AddProfile(new MappingProfile())
      );
      _mapper = new Mapper(config);
      _seed = new Seed();
   }
   
   [Fact]
   public void Person2PersonDtoDtoUt() {
      // Arrange
       _seed.InitPeopleWithWordorders();
      // Act
      PersonDto? actualDto = _mapper.Map<PersonDto>(_seed.Person1);
      // Assert
      actualDto.Should().NotBeNull().And
         .BeOfType<PersonDto>();
      actualDto.Id.Should().Be(_seed.Person1.Id);
      actualDto.FirstName.Should().Be(_seed.Person1.FirstName);
      actualDto.LastName.Should().Be(_seed.Person1.LastName);
      actualDto.Email.Should().Be(_seed.Person1.Email);
      actualDto.Phone.Should().Be(_seed.Person1.Phone);
      
      
   }

   [Fact]
   public void PersonDto2PersonUt() {
      // Arrange
      _seed.InitPeopleWithWordorders();
      PersonDto? personDto = _mapper.Map<PersonDto>(_seed.Person1);
      // Act
      Person? actual = _mapper.Map<Person>(personDto);
      // Assert
      actual.Should().NotBeNull().And
         .BeOfType<Person>();
      actual.Id.Should().Be(_seed.Person1.Id);
      actual.FirstName.Should().Be(_seed.Person1.FirstName);
      actual.LastName.Should().Be(_seed.Person1.LastName);
      actual.Email.Should().Be(_seed.Person1.Email);
      actual.Phone.Should().Be(_seed.Person1.Phone);
      //actual.Workorders.Should().BeEquivalentTo(_seed.Person1.Workorders);
      
      
   }
   
}