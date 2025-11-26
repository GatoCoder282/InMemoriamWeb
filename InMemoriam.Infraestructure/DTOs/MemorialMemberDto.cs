using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InMemoriam.Infraestructure.DTOs
{
    public class MemorialMemberDto
    {
        public int Id { get; set; }
        public int MemorialId { get; set; }
        public int UserId { get; set; }
        public string Role { get; set; } = "Viewer";
        public string Status { get; set; } = "active";
        public string JoinedAt { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
