using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class CreateRequestDto
    {
        [Required]
        public int SenderId { get; set; }

        [Required]
        public int ReceiverId { get; set; }

        [Required]
        [MaxLength(50)]
        public string RequestType { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Message { get; set; } = string.Empty;
    }
}
