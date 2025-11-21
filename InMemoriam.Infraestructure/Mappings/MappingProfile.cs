using AutoMapper;
using InMemoriam.Core.Entities;
using InMemoriam.Infraestructure.DTOs;

namespace InMemoriam.Infraestructure.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Habilita mapeo en ambas direcciones (User <-> UserDto)
            CreateMap<User, UserDto>()
                .ForMember(d => d.DateOfBirth, o => o.Ignore())
                .ReverseMap();

            CreateMap<Memorial, MemorialDto>()
                .ForMember(d => d.BirthDate, o => o.MapFrom(s => s.BirthDate.HasValue ? s.BirthDate.Value.ToString("yyyy-MM-dd") : null))
                .ForMember(d => d.DeathDate, o => o.MapFrom(s => s.DeathDate.HasValue ? s.DeathDate.Value.ToString("yyyy-MM-dd") : null))
                .ForMember(d => d.Visibility, o => o.MapFrom(s => s.Visibility.ToString()))
                .ReverseMap();

            CreateMap<MediaAsset, MediaAssetDto>()
                .ForMember(d => d.Date, o => o.MapFrom(s => s.Date.ToString("yyyy-MM-dd")))
                .ForMember(d => d.Kind, o => o.MapFrom(s => s.Kind.ToString()))
                .ReverseMap();
        }
    }
}
