using System.Net.Http;
using System.Net.Http.Json;
using Models;

namespace Messanger.Services
{
    public class ApiService
    {
        private readonly HttpClient client;
        private const string baseUrl = "https://localhost:7246/";

        public ApiService()
        {
            client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
        }

        public async Task<bool> RegisterAsync(string username, string email, string password)
        {
            var payload = new
            {
                Username = username,
                Email = email,
                Password = password
            };

            var response = await client
              .PostAsJsonAsync("api/Users/register", payload);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            // Debug-Infos holen
            var statusCode = (int)response.StatusCode;
            var reason = response.ReasonPhrase ?? "";
            var content = await response.Content.ReadAsStringAsync();

            // Exception werfen -> wird im ViewModel gefangen
            throw new Exception(
              $"Registrierung fehlgeschlagen " +
              $"(HTTP {statusCode} {reason}). " +
              $"Server-Antwort: {content}");
        }

        public async Task<User?> LoginAsync(string username, string password, string email)
        {
            var payload = new
            {
                Username = username,
                Email = email,
                Password = password
            };

            var response = await client
              .PostAsJsonAsync("api/Users/login", payload);

            if (!response.IsSuccessStatusCode)
            {
                var statusCode = (int)response.StatusCode;
                var reason = response.ReasonPhrase ?? "";
                var content = await response.Content.ReadAsStringAsync();

                throw new Exception(
                  $"Login fehlgeschlagen (HTTP {statusCode} {reason}). " +
                  $"Server-Antwort: {content}");
            }

            var user =
              await response.Content.ReadFromJsonAsync<User>();

            return user;
        }

        public async Task<List<User>> SearchUsersAsync(string query)
        {
            var response = await client.GetAsync($"api/Friends/search?query={query}");

            if (!response.IsSuccessStatusCode)
            {
                return new List<User>();
            }

            var users = await response.Content.ReadFromJsonAsync<List<User>>();
            return users ?? new List<User>();
        }

        public async Task<bool> SendFriendRequestAsync(int userId, int friendUserId)
        {
            var payload = new
            {
                UserId = userId,
                FriendUserId = friendUserId
            };

            var response = await client.PostAsJsonAsync("api/Friends/request", payload);

            if (!response.IsSuccessStatusCode)
            {
                var statusCode = (int)response.StatusCode;
                var content = await response.Content.ReadAsStringAsync();
                
                throw new Exception(
                    $"Anfrage fehlgeschlagen (HTTP {statusCode}). " +
                    $"Server: {content}");
            }

            return response.IsSuccessStatusCode;
        }

        public async Task<List<Friends>> GetPendingRequestsAsync(int userId)
        {
            var response = await client.GetAsync($"api/Friends/pending/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                return new List<Friends>();
            }

            var requests = await response.Content.ReadFromJsonAsync<List<Friends>>();
            return requests ?? new List<Friends>();
        }

        public async Task<List<User>> GetFriendsAsync(int userId)
        {
            var response = await client.GetAsync($"api/Friends/list/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                return new List<User>();
            }

            var friends = await response.Content.ReadFromJsonAsync<List<User>>();
            return friends ?? new List<User>();
        }

        public async Task<bool> AcceptRequestAsync(int requestId)
        {
            var response = await client.PutAsync($"api/Friends/accept/{requestId}", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeclineRequestAsync(int requestId)
        {
            var response = await client.DeleteAsync($"api/Friends/decline/{requestId}");
            return response.IsSuccessStatusCode;
        }
    }
}
