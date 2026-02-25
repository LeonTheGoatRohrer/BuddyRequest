using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Messages
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int EmpfaengerId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }

        [NotMapped]
        public string SenderName { get; set; } = string.Empty;

        [NotMapped]
        public bool IsOwnMessage { get; set; }

        [NotMapped]
        public string SentAtWithSender => $"{SentAt:HH:mm} | {SenderName}";
    }
}
