using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InMemoriam.Infraestructure.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string DateOfBirth { get; set; } = null!;

        public string? Telephone { get; set; }
        public bool IsActive { get; set; }

        // Nuevo: sólo para entrada (creación/actualización). No se mapea automáticamente a PasswordHash.
        public string? Password { get; set; }
    }
}
