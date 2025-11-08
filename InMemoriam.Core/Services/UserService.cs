using InMemoriam.Core.Entities;
using InMemoriam.Core.Exceptions;
using InMemoriam.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InMemoriam.Core.Services
{
    public sealed class UserService : IUserService
    {
        private readonly IUnitOfWork _uow;
        private readonly IUserRepository _repo;
        public UserService(IUnitOfWork uow) { _uow = uow; _repo = uow.UserRepository; }

        public Task<IEnumerable<User>> GetAllAsync() => _repo.GetAllDapperAsync();
        public Task<User?> GetByIdAsync(int id) => _repo.GetByIdDapperAsync(id);

        public async Task<User> CreateAsync(User e)
        {
            if (!Regex.IsMatch(e.Email ?? "", @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) throw new BusinessException("Email inválido");
            if (await _repo.ExistsEmailAsync(e.Email)) throw new BusinessException("Email ya registrado");

            await _repo.Add(e);
            await _uow.SaveChangesAsync();
            return e;
        }

        public async Task UpdateAsync(User e)
        {
            var current = await _repo.GetByIdDapperAsync(e.Id) ?? throw new NotFoundException("Usuario no encontrado");
            if (!string.Equals(current.Email, e.Email, StringComparison.OrdinalIgnoreCase))
            {
                if (!Regex.IsMatch(e.Email ?? "", @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) throw new BusinessException("Email inválido");
                if (await _repo.ExistsEmailAsync(e.Email)) throw new BusinessException("Email ya registrado");
            }
            await _repo.Update(e);
            await _uow.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            if (await _repo.GetByIdDapperAsync(id) is null) throw new NotFoundException("Usuario no encontrado");
            await _repo.Delete(id);
            await _uow.SaveChangesAsync();
        }
    }

}
