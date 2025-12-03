using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InMemoriam.Infraestructure.DTOs
{
    public class    MediaAssetDto
    {
        public int Id { get; set; }
        public int MemorialId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string Kind { get; set; } = "photo"; 
        public string StorageKey { get; set; } = null!;
        public long SizeBytes { get; set; }
        public string Checksum { get; set; } = null!;
        public string Date { get; set; } = null!; 
        public string? Tags { get; set; }
        public bool IsActive { get; set; }
    }
}
