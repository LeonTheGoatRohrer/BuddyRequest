using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ORM.Services
{
    public class DbManager : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Friends> Friends { get; set; }
        public DbSet<Messages> Messages { get; set; }
        public DbSet<Request> Requests { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = "Server=localhost;database=Messanger;user=root;password=Leon Rohrer 2006";
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }

        public async Task<User?> findeBenutzerNachIdAsync(int id)
        {
            return await this.Users.FindAsync(id);
        }

        public async Task<User?> findeBenutzerNachNameAsync(string username)
        {
            return await this.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> findeBenutzerNachSchluesselAsync(string key)
        {
            return await this.Users.FirstOrDefaultAsync(u => u.Key == key);
        }

        public async Task<User> registriereBenutzerAsync(User benutzer)
        {
            // Falls Client eine Id mitliefert, sicherstellen, dass DB die Id generiert
            benutzer.Id = 0;

            await this.Users.AddAsync(benutzer);
            await this.SaveChangesAsync();
            return benutzer;
        }

        public async Task<List<User>> GetAllUsersAsync(int userId)
        {
            // Alle User-IDs die bereits Freunde sind
            var friendIds = await this.Friends
                .Where(f => (f.UserId == userId || f.FriendUserId == userId) && f.Angenommen)
                .Select(f => f.UserId == userId ? f.FriendUserId : f.UserId)
                .ToListAsync();

            // Alle User außer: sich selbst und existierenden Freunden
            return await this.Users
                .Where(u => u.Id != userId && !friendIds.Contains(u.Id))
                .ToListAsync();
        }

        public async Task<List<User>> GetAllUsersWithRequestStatusAsync(int userId)
        {
            // Alle User-IDs die bereits Freunde sind
            var friendIds = await this.Friends
                .Where(f => (f.UserId == userId || f.FriendUserId == userId) && f.Angenommen)
                .Select(f => f.UserId == userId ? f.FriendUserId : f.UserId)
                .ToListAsync();

            // Alle ausstehenden Friend Requests die der User gesendet hat
            var sentRequestIds = await this.Friends
                .Where(f => f.UserId == userId && !f.Angenommen)
                .Select(f => f.FriendUserId)
                .ToListAsync();

            // Alle User außer: sich selbst und existierenden Freunden
            var users = await this.Users
                .Where(u => u.Id != userId && !friendIds.Contains(u.Id))
                .ToListAsync();

            // RequestSent Status setzen
            foreach (var user in users)
            {
                user.RequestSent = sentRequestIds.Contains(user.Id);
            }

            return users;
        }

        public async Task<List<User>> SucheBenutzerAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new List<User>();
            }

            return await this.Users
                .Where(u => u.Username.Contains(query) || u.Key.Contains(query))
                .Take(10)
                .ToListAsync();
        }

        public async Task<List<User>> SucheBenutzerWithRequestStatusAsync(string query, int userId)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new List<User>();
            }

            // Alle ausstehenden Friend Requests die der User gesendet hat
            var sentRequestIds = await this.Friends
                .Where(f => f.UserId == userId && !f.Angenommen)
                .Select(f => f.FriendUserId)
                .ToListAsync();

            var users = await this.Users
                .Where(u => u.Username.Contains(query) || u.Key.Contains(query))
                .Take(10)
                .ToListAsync();

            // RequestSent Status setzen
            foreach (var user in users)
            {
                user.RequestSent = sentRequestIds.Contains(user.Id);
            }

            return users;
        }

        public async Task<Friends?> SendeFriendRequestAsync(int userId, int friendUserId)
        {
            var existingRequest = await this.Friends
                .FirstOrDefaultAsync(f => 
                    (f.UserId == userId && f.FriendUserId == friendUserId) ||
                    (f.UserId == friendUserId && f.FriendUserId == userId));

            if (existingRequest != null)
            {
                return null;
            }

            var request = new Friends
            {
                UserId = userId,
                FriendUserId = friendUserId,
                Angenommen = false,
                CreatedAt = DateTime.UtcNow
            };

            await this.Friends.AddAsync(request);
            await this.SaveChangesAsync();
            return request;
        }

        public async Task<List<Friends>> GetPendingRequestsAsync(int userId)
        {
            return await this.Friends
                .Where(f => f.FriendUserId == userId && !f.Angenommen)
                .ToListAsync();
        }

        public async Task<List<FriendRequestWithUser>> GetPendingRequestsWithUserAsync(int userId)
        {
            var requests = await this.Friends
                .Where(f => f.FriendUserId == userId && !f.Angenommen)
                .Join(this.Users,
                    f => f.UserId,
                    u => u.Id,
                    (f, u) => new FriendRequestWithUser
                    {
                        RequestId = f.Id,
                        SenderId = u.Id,
                        SenderUsername = u.Username,
                        SenderKey = u.Key ?? string.Empty,
                        SenderEmail = u.Email,
                        SenderAvatar = string.IsNullOrEmpty(u.AvatarUrl) 
                            ? $"https://api.dicebear.com/7.x/avataaars/svg?seed={u.Username}&backgroundColor=random"
                            : u.AvatarUrl,
                        CreatedAt = f.CreatedAt
                    })
                .ToListAsync();

            return requests;
        }

        public async Task<List<User>> GetFreundeAsync(int userId)
        {
            var friendIds = await this.Friends
                .Where(f => (f.UserId == userId || f.FriendUserId == userId) && f.Angenommen)
                .Select(f => f.UserId == userId ? f.FriendUserId : f.UserId)
                .ToListAsync();

            return await this.Users
                .Where(u => friendIds.Contains(u.Id))
                .ToListAsync();
        }

        public async Task<bool> AcceptFriendRequestAsync(int requestId)
        {
            var request = await this.Friends.FindAsync(requestId);
            if (request == null)
            {
                return false;
            }

            request.Angenommen = true;
            await this.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeclineFriendRequestAsync(int requestId)
        {
            var request = await this.Friends.FindAsync(requestId);
            if (request == null)
            {
                return false;
            }

            this.Friends.Remove(request);
            await this.SaveChangesAsync();
            return true;
        }

        public async Task<bool> saveToDbAsync()
        {
            try
            {
                return await this.SaveChangesAsync() >= 1;
            }
            catch
            {
                //maybe logging...
            }

            return false;
        }

        public async Task<Messages> SendeNachrichtAsync(Messages message)
        {
            await this.Messages.AddAsync(message);
            await this.SaveChangesAsync();
            return message;
        }

        public async Task<List<Messages>> GetChatHistoryAsync(int userId1, int userId2)
        {
            return await this.Messages
                .Where(m => (m.SenderId == userId1 && m.EmpfaengerId == userId2) ||
                            (m.SenderId == userId2 && m.EmpfaengerId == userId1))
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<List<int>> GetConversationsAsync(int userId)
        {
            var conversationPartners = await this.Messages
                .Where(m => m.SenderId == userId || m.EmpfaengerId == userId)
                .Select(m => m.SenderId == userId ? m.EmpfaengerId : m.SenderId)
                .Distinct()
                .ToListAsync();

            return conversationPartners;
        }

        // ===== REQUEST METHODS =====

        public async Task<Request> CreateRequestAsync(Request request)
        {
            request.Id = 0;
            request.CreatedAt = DateTime.UtcNow;
            request.Status = "Pending";

            await this.Requests.AddAsync(request);
            await this.SaveChangesAsync();
            return request;
        }

        public async Task<List<RequestWithUser>> GetPendingRequestsForUserAsync(int userId)
        {
            var requests = await this.Requests
                .Where(r => r.ReceiverId == userId && r.Status == "Pending")
                .Join(this.Users,
                    r => r.SenderId,
                    u => u.Id,
                    (r, u) => new RequestWithUser
                    {
                        Id = r.Id,
                        SenderId = r.SenderId,
                        ReceiverId = r.ReceiverId,
                        RequestType = r.RequestType,
                        Message = r.Message,
                        Status = r.Status,
                        CreatedAt = r.CreatedAt,
                        RespondedAt = r.RespondedAt,
                        SenderUsername = u.Username,
                        SenderAvatarUrl = string.IsNullOrEmpty(u.AvatarUrl)
                            ? $"https://api.dicebear.com/7.x/avataaars/png?seed={u.Username}"
                            : u.AvatarUrl,
                        SenderKey = u.Key ?? string.Empty,
                        ReceiverUsername = string.Empty,
                        ReceiverAvatarUrl = string.Empty
                    })
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return requests;
        }

        public async Task<List<RequestWithUser>> GetSentRequestsAsync(int userId)
        {
            var requests = await this.Requests
                .Where(r => r.SenderId == userId)
                .Join(this.Users,
                    r => r.ReceiverId,
                    u => u.Id,
                    (r, u) => new RequestWithUser
                    {
                        Id = r.Id,
                        SenderId = r.SenderId,
                        ReceiverId = r.ReceiverId,
                        RequestType = r.RequestType,
                        Message = r.Message,
                        Status = r.Status,
                        CreatedAt = r.CreatedAt,
                        RespondedAt = r.RespondedAt,
                        SenderUsername = string.Empty,
                        SenderAvatarUrl = string.Empty,
                        SenderKey = string.Empty,
                        ReceiverUsername = u.Username,
                        ReceiverAvatarUrl = string.IsNullOrEmpty(u.AvatarUrl)
                            ? $"https://api.dicebear.com/7.x/avataaars/png?seed={u.Username}"
                            : u.AvatarUrl
                    })
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return requests;
        }

        public async Task<List<RequestWithUser>> GetRequestHistoryAsync(int userId)
        {
            var requests = await this.Requests
                .Where(r => (r.SenderId == userId || r.ReceiverId == userId) && r.Status != "Pending")
                .ToListAsync();

            var requestsWithUser = new List<RequestWithUser>();

            foreach (var request in requests)
            {
                var otherUserId = request.SenderId == userId ? request.ReceiverId : request.SenderId;
                var otherUser = await this.Users.FindAsync(otherUserId);

                if (otherUser != null)
                {
                    var requestWithUser = new RequestWithUser
                    {
                        Id = request.Id,
                        SenderId = request.SenderId,
                        ReceiverId = request.ReceiverId,
                        RequestType = request.RequestType,
                        Message = request.Message,
                        Status = request.Status,
                        CreatedAt = request.CreatedAt,
                        RespondedAt = request.RespondedAt
                    };

                    if (request.SenderId == userId)
                    {
                        requestWithUser.ReceiverUsername = otherUser.Username;
                        requestWithUser.ReceiverAvatarUrl = string.IsNullOrEmpty(otherUser.AvatarUrl)
                            ? $"https://api.dicebear.com/7.x/avataaars/png?seed={otherUser.Username}"
                            : otherUser.AvatarUrl;
                    }
                    else
                    {
                        requestWithUser.SenderUsername = otherUser.Username;
                        requestWithUser.SenderAvatarUrl = string.IsNullOrEmpty(otherUser.AvatarUrl)
                            ? $"https://api.dicebear.com/7.x/avataaars/png?seed={otherUser.Username}"
                            : otherUser.AvatarUrl;
                        requestWithUser.SenderKey = otherUser.Key ?? string.Empty;
                    }

                    requestsWithUser.Add(requestWithUser);
                }
            }

            return requestsWithUser.OrderByDescending(r => r.RespondedAt ?? r.CreatedAt).ToList();
        }

        public async Task<bool> AcceptRequestAsync(int requestId)
        {
            var request = await this.Requests.FindAsync(requestId);
            if (request == null || request.Status != "Pending")
            {
                return false;
            }

            request.Status = "Accepted";
            request.RespondedAt = DateTime.UtcNow;
            await this.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeclineRequestAsync(int requestId)
        {
            var request = await this.Requests.FindAsync(requestId);
            if (request == null || request.Status != "Pending")
            {
                return false;
            }

            request.Status = "Declined";
            request.RespondedAt = DateTime.UtcNow;
            await this.SaveChangesAsync();
            return true;
        }
    }
}