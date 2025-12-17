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

        public async Task<bool> AcceptRequestAsync(int requestId)
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

        public async Task<bool> DeclineRequestAsync(int requestId)
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
    }
}