using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InMemoriam.Infraestructure.DTOs
{
    public class InvitationDto
    {
        public int Id { get; set; }
        public int MemorialId { get; set; }
        public string Email { get; set; } = null!;
        public string Role { get; set; } = "Viewer";
        public string Token { get; set; } = null!;
        public string Status { get; set; } = "pending";
        public string? ExpiresAt { get; set; }
        public string? AcceptedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
