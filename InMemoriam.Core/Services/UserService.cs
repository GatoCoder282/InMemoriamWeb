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

            e.IsActive = true;

            await _repo.Add(e);
            await _uow.SaveChangesAsync();
            return e;
        }

        public async Task UpdateAsync(User e)
        {
            // Recuperar el registro actual (origen de verdad)
            var current = await _repo.GetByIdDapperAsync(e.Id) ?? throw new NotFoundException("Usuario no encontrado");

            // Regla: el Id no se puede editar (ignorar cualquier cambio en e.Id)
            // Regla: la contraseña no se actualiza aquí (debe tener un controlador/método específico)
            // Regla: IsActive no se modifica por este método para evitar sobrescrituras no intencionadas.
            // Solo se permiten cambios explícitos en el conjunto de campos permitidos.

            // Nombre y apellido
            if (!string.IsNullOrWhiteSpace(e.FirstName) && !string.Equals(current.FirstName, e.FirstName, StringComparison.Ordinal))
                current.FirstName = e.FirstName;

            if (!string.IsNullOrWhiteSpace(e.LastName) && !string.Equals(current.LastName, e.LastName, StringComparison.Ordinal))
                current.LastName = e.LastName;

            // Email: si se intentó cambiar, validar y comprobar unicidad
            if (!string.IsNullOrWhiteSpace(e.Email) && !string.Equals(current.Email, e.Email, StringComparison.OrdinalIgnoreCase))
            {
                if (!Regex.IsMatch(e.Email ?? "", @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) throw new BusinessException("Email inválido");
                if (await _repo.ExistsEmailAsync(e.Email)) throw new BusinessException("Email ya registrado");
                current.Email = e.Email;
            }

            // Fecha de nacimiento (DateOnly?)
            if (e.DateOfBirth.HasValue)
                current.DateOfBirth = e.DateOfBirth;

            // Teléfono: si se envía (puede ser cadena vacía si el cliente lo desea explícitamente)
            if (e.Telephone != null)
                current.Telephone = e.Telephone;

            // NOTA: No tocar current.PasswordHash ni current.IsActive aquí.

            await _repo.Update(current);
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
