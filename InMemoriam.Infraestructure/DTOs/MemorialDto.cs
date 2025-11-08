using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InMemoriam.Infraestructure.DTOs
{
    public class MemorialDto
    {
        public int Id { get; set; }
        public string Slug { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? BirthDate { get; set; } 
        public string? DeathDate { get; set; } 
        public string Visibility { get; set; } = "Private";
        public int OwnerUserId { get; set; }
        public bool IsActive { get; set; }
    }
}
