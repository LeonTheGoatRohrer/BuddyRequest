using System.Net.Http;
using System.Net.Http.Json;
using Models;

namespace Messanger.Services
{
    public class ApiService
    {
        private readonly HttpClient client;
        private const string baseUrl = "http://localhost:5231/";

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

        public async Task<User?> LoginAsync(string username, string password)
        {
            var payload = new
            {
                Username = username,
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

        public async Task<List<User>> GetAllUsersAsync(int currentUserId)
        {
            var response = await client.GetAsync($"api/Users/all?userId={currentUserId}");

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

        public async Task<List<FriendRequestWithUser>> GetPendingRequestsWithUserAsync(int userId)
        {
            var response = await client.GetAsync($"api/Friends/pending/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                return new List<FriendRequestWithUser>();
            }

            var requests = await response.Content.ReadFromJsonAsync<List<FriendRequestWithUser>>();
            return requests ?? new List<FriendRequestWithUser>();
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

        public async Task<bool> AcceptFriendRequestAsync(int requestId)
        {
            var response = await client.PutAsync($"api/Friends/accept/{requestId}", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeclineFriendRequestAsync(int requestId)
        {
            var response = await client.DeleteAsync($"api/Friends/decline/{requestId}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateBioAsync(int userId, string bio)
        {
            try
            {
                var payload = new { Bio = bio };
                var response = await client.PatchAsync(
                    $"api/Users/update-bio/{userId}", 
                    JsonContent.Create(payload));

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[ApiService] UpdateBio failed: {response.StatusCode}, {error}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ApiService] UpdateBio exception: {ex.Message}");
                return false;
            }
        }

        public async Task<string?> UpdateAvatarAsync(int userId)
        {
            try
            {
                var response = await client.PatchAsync($"api/Users/update-avatar/{userId}", null);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[ApiService] UpdateAvatar failed: {response.StatusCode}, {error}");
                    return null;
                }

                var result = await response.Content.ReadFromJsonAsync<AvatarResponse>();
                return result?.AvatarUrl;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ApiService] UpdateAvatar exception: {ex.Message}");
                return null;
            }
        }

        public async Task<User?> UpdateProfileAsync(int userId, string username, string email)
        {
            try
            {
                var payload = new { Username = username, Email = email };
                var response = await client.PatchAsync(
                    $"api/Users/update-profile/{userId}", 
                    JsonContent.Create(payload));

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[ApiService] UpdateProfile failed: {response.StatusCode}, {error}");
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<User>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ApiService] UpdateProfile exception: {ex.Message}");
                return null;
            }
        }

        private class AvatarResponse
        {
            public string AvatarUrl { get; set; }
        }

        public async Task<bool> SendMessageAsync(int senderId, int receiverId, string message)
        {
            try
            {
                var payload = new { SenderId = senderId, ReceiverId = receiverId, Message = message };
                var response = await client.PostAsJsonAsync("api/Messages/send", payload);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ApiService] SendMessage exception: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Models.Messages>> GetChatHistoryAsync(int userId1, int userId2)
        {
            try
            {
                var response = await client.GetAsync($"api/Messages/chat/{userId1}/{userId2}");
                
                if (!response.IsSuccessStatusCode)
                    return new List<Models.Messages>();

                return await response.Content.ReadFromJsonAsync<List<Models.Messages>>() ?? new List<Models.Messages>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ApiService] GetChatHistory exception: {ex.Message}");
                return new List<Models.Messages>();
            }
        }
    }
}
