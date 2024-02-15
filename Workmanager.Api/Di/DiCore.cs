using AutoMapper;
using Workmanager.Api.Core.DomainModel.Entities;
using Workmanager.Api.Core.Mapping;
using Workmanager.Api.Core.Misc;
namespace Workmanager.Api.Di; 
public static class DiCore {
   public static IServiceCollection AddCore(
      this IServiceCollection services
   ){

      services.AddAutoMapper(typeof(Person), typeof(MappingProfile));
      services.AddAutoMapper(typeof(Workorder), typeof(MappingProfile));
      services.AddAutoMapper(typeof(Image), typeof(MappingProfile));

      // Auto Mapper Configurations
      var mapperConfig = new MapperConfiguration(mc => {
         mc.AddProfile(new MappingProfile());
      });

      return services;
   }
}