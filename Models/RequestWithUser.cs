using System;

namespace Models
{
    public class RequestWithUser
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string RequestType { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? RespondedAt { get; set; }

        // User-Daten des Senders
        public string SenderUsername { get; set; } = string.Empty;
        public string SenderAvatarUrl { get; set; } = string.Empty;
        public string SenderKey { get; set; } = string.Empty;

        // User-Daten des Empfängers
        public string ReceiverUsername { get; set; } = string.Empty;
        public string ReceiverAvatarUrl { get; set; } = string.Empty;
    }
}
