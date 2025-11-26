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
                .ReverseMap()
                // Evitar mapear Password (DTO) directamente a PasswordHash (se hace manualmente)
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

            CreateMap<Memorial, MemorialDto>()
                .ForMember(d => d.BirthDate, o => o.MapFrom(s => s.BirthDate.HasValue ? s.BirthDate.Value.ToString("yyyy-MM-dd") : null))
                .ForMember(d => d.DeathDate, o => o.MapFrom(s => s.DeathDate.HasValue ? s.DeathDate.Value.ToString("yyyy-MM-dd") : null))
                .ForMember(d => d.Visibility, o => o.MapFrom(s => s.Visibility.ToString()))
                .ReverseMap();

            CreateMap<MediaAsset, MediaAssetDto>()
                .ForMember(d => d.Date, o => o.MapFrom(s => s.Date.ToString("yyyy-MM-dd")))
                .ForMember(d => d.Kind, o => o.MapFrom(s => s.Kind.ToString()))
                .ReverseMap();

            // Nuevos mapeos
            CreateMap<Invitation, InvitationDto>()
                .ForMember(d => d.ExpiresAt, o => o.MapFrom(s => s.ExpiresAt.HasValue ? s.ExpiresAt.Value.ToString("o") : null))
                .ForMember(d => d.AcceptedAt, o => o.MapFrom(s => s.AcceptedAt.HasValue ? s.AcceptedAt.Value.ToString("o") : null))
                .ReverseMap();

            CreateMap<InvitationCreateDto, Invitation>()
                .ForMember(d => d.Token, o => o.Ignore())
                .ForMember(d => d.Status, o => o.Ignore());

            CreateMap<MemorialMember, MemorialMemberDto>()
                .ForMember(d => d.Role, o => o.MapFrom(s => s.Role.ToString()))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.JoinedAt, o => o.MapFrom(s => s.JoinedAt.ToString("o")))
                .ReverseMap();
        }
    }
}
