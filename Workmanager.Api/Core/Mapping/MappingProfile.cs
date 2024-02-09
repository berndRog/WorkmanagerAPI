using AutoMapper;
using Workmanager.Api.Core.DomainModel.Entities;
using Workmanager.Api.Core.Dto;
using Workmanager.Api.Core.Misc;

namespace Workmanager.Api.Core.Mapping {

    public class MappingProfile : Profile {
      
      public MappingProfile() {

         CreateMap<ImageDto, Image>()
            .ReverseMap();
         
         // Add as many of these lines as you need to map your objects
         CreateMap<Person, PersonDto>();
         CreateMap<PersonDto, Person>()
            .ForMember(m => m.Workorders, options => options.Ignore());
         
         CreateMap<Workorder, WorkorderDto>()
            .ForMember(wDto => wDto.Duration, // convert TimeSpan to nanos/100,  1 tick = 100-nanos
               options => options.MapFrom( w => w.Duration.Ticks*100));  
         CreateMap<WorkorderDto, Workorder>()
            .ForMember(w => w.Duration,   // convert nanos/100 to TimeSpan
               options => options.MapFrom( wDto => new TimeSpan(wDto.Duration/100)));  

      }
   }
}

