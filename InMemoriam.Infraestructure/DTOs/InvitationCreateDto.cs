using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InMemoriam.Infraestructure.DTOs
{
    public class InvitationCreateDto
    {
        public int MemorialId { get; set; }
        public string Email { get; set; } = null!;
        public string Role { get; set; } = "Viewer";
        public string? ExpiresAt { get; set; } // "yyyy-MM-ddTHH:mm:ss" optional
    }
}
