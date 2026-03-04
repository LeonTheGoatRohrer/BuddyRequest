using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Request
    {
        public int Id { get; set; }

        [Required]
        public int SenderId { get; set; }

        [Required]
        public int ReceiverId { get; set; }

        [Required]
        [MaxLength(50)]
        public string RequestType { get; set; } = string.Empty; // z.B. "Wasser", "Lebensmittel", "Allgemein"

        [Required]
        [MaxLength(500)]
        public string Message { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending"; // "Pending", "Accepted", "Declined"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? RespondedAt { get; set; }
    }
}
