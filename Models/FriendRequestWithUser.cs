using Models;

namespace Models
{
    public class FriendRequestWithUser
    {
        public int RequestId { get; set; }
        public int SenderId { get; set; }
        public string SenderUsername { get; set; } = string.Empty;
        public string SenderKey { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderAvatar { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
