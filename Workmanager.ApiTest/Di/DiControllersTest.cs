using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Workmanager.Api;
using Workmanager.Api.Controllers;
using Workmanager.Api.Core.DomainModel.Entities;
using Workmanager.Api.Core.Mapping;

namespace Workmanager.ApiTest.Di;

public static class DiControllersTest {
   public static IServiceCollection AddControllersTest(
      this IServiceCollection services
   ) {
      services.AddAutoMapper(typeof(Person), typeof(MappingProfile));
      // Auto Mapper Configurations
      var mapperConfig = new MapperConfiguration(mc => { mc.AddProfile(new MappingProfile()); });
      
      // Controllers
      services.AddScoped<ImagesController>();
      services.AddScoped<PeopleController>();
      services.AddScoped<WorkordersController>();
      
      return services;
   }
}