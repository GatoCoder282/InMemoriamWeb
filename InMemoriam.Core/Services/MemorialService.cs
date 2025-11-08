using InMemoriam.Core.CustomEntities;
using InMemoriam.Core.Entities;
using InMemoriam.Core.Exceptions;
using InMemoriam.Core.Interfaces;
using InMemoriam.Core.QueryFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InMemoriam.Core.Services
{
    public sealed class MemorialService : IMemorialService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMemorialRepository _memRepo;
        private readonly IUserRepository _userRepo;

        public MemorialService(IUnitOfWork uow) { _uow = uow; _memRepo = uow.MemorialRepository; _userRepo = uow.UserRepository; }

        public Task<PagedList<Memorial>> GetPagedAsync(MemorialQueryFilter f) => _memRepo.GetPaged(f);
        public Task<Memorial?> GetByIdAsync(int id) => _memRepo.GetById(id);

        public async Task<Memorial> CreateAsync(Memorial e)
        {
            // validaciones
            var owner = await _userRepo.GetById(e.OwnerUserId);
            if (owner == null) throw new BusinessException("El usuario propietario no existe");
            if (e.BirthDate.HasValue && e.DeathDate.HasValue && e.BirthDate > e.DeathDate)
                throw new BusinessException("La fecha de nacimiento no puede ser posterior a la fecha de fallecimiento");

            e.Slug = NormalizeSlug(e.Slug ?? e.FullName);
            if (await _memRepo.SlugExists(e.Slug)) throw new BusinessException("El slug ya está en uso");

            var todayCount = await _memRepo.CountByOwnerToday(e.OwnerUserId, DateTime.UtcNow);
            if (todayCount >= 50) throw new BusinessException("Se alcanzó el máximo de memoriales permitidos hoy");

            await _memRepo.Add(e);
            await _uow.SaveChangesAsync();
            return e;
        }

        public async Task UpdateAsync(Memorial e)
        {
            var current = await _memRepo.GetById(e.Id) ?? throw new NotFoundException("Memorial no encontrado");
            if (e.BirthDate.HasValue && e.DeathDate.HasValue && e.BirthDate > e.DeathDate)
                throw new BusinessException("Fechas inconsistentes");

            if (!string.Equals(current.Slug, e.Slug, StringComparison.OrdinalIgnoreCase))
            {
                e.Slug = NormalizeSlug(e.Slug ?? e.FullName);
                if (await _memRepo.SlugExists(e.Slug)) throw new BusinessException("El slug ya está en uso");
            }

            await _memRepo.Update(e);
            await _uow.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            if (await _memRepo.GetById(id) is null) throw new NotFoundException("Memorial no encontrado");
            await _memRepo.Delete(id);
            await _uow.SaveChangesAsync();
        }

        private static string NormalizeSlug(string s)
        {
            s = s.Trim().ToLowerInvariant();
            s = Regex.Replace(s, @"[^a-z0-9\-\s]", "");
            s = Regex.Replace(s, @"\s+", "-");
            s = Regex.Replace(s, @"-+", "-");
            return s;
        }
    }

}
