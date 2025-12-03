using AutoMapper;
using InMemoriam.Core.Entities;
using InMemoriam.Core.Enum;
using InMemoriam.Infraestructure.DTOs;
using System;
using System.Globalization;

namespace InMemoriam.Infraestructure.Mappings
{

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Habilita mapeo en ambas direcciones (User <-> UserDto)
            CreateMap<User, UserDto>()
                .ForMember(d => d.DateOfBirth, o => o.MapFrom(s => FormatNullableDate(s.DateOfBirth)))
                .ReverseMap()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => ParseNullableDate(src.DateOfBirth)))
                // Solo mapear miembros del DTO hacia la entidad cuando el valor fuente NO sea null.
                // Esto evita sobrescribir campos no enviados (por ejemplo Telephone o IsActive).
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Memorial, MemorialDto>()
                .ForMember(d => d.BirthDate, o => o.MapFrom(s => FormatNullableDate(s.BirthDate)))
                .ForMember(d => d.DeathDate, o => o.MapFrom(s => FormatNullableDate(s.DeathDate)))
                .ForMember(d => d.Visibility, o => o.MapFrom(s => s.Visibility.ToString()))
                .ReverseMap()
                .ForMember(d => d.BirthDate, o => o.MapFrom(s => ParseNullableDate(s.BirthDate)))
                .ForMember(d => d.DeathDate, o => o.MapFrom(s => ParseNullableDate(s.DeathDate)))
                .ForMember(d => d.Visibility, o => o.MapFrom(s => TryParseEnum<MemorialVisibility>(s.Visibility, MemorialVisibility.Private)));

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

        private static string? FormatNullableDate(DateOnly? d) =>
            d.HasValue ? d.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : null;

        private static DateOnly? ParseNullableDate(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;

            // Aceptar múltiples formatos comunes (ISO yyyy-MM-dd y formato dd-MM-yyyy que usas en Postman)
            var formats = new[] { "yyyy-MM-dd", "dd-MM-yyyy", "yyyy/MM/dd", "dd/MM/yyyy" };
            if (DateOnly.TryParseExact(s, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d))
                return d;

            // Intentar parse ISO completo por si viene con hora
            if (DateOnly.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d2))
                return d2;

            return null;
        }

        private static T TryParseEnum<T>(string? s, T @default) where T : struct
        {
            if (!string.IsNullOrWhiteSpace(s) && Enum.TryParse<T>(s, true, out var value))
                return value;
            return @default;
        }
    }
}


