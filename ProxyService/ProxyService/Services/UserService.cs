using MicroServices1.Models;
using ProxyService.Interfaces;
using Serilog;
using System.Text.Json;

namespace ProxyService.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        // Кеш для збереження користувачів у пам'яті
        private static readonly Dictionary<int, User> _userCache = new Dictionary<int, User>();

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<User> GetUserById(int id)
        {
            // Перевіряємо, чи є користувач в кеші
            if (_userCache.ContainsKey(id))
            {
                Log.Information("User with ID {Id} found in cache", id);
                return _userCache[id];
            }

            // Якщо користувача немає в кеші, виконуємо HTTP-запит до reqres.in
            // Логування запиту до зовнішнього API
            Log.Information("Fetching user with ID {Id} from external API", id);
            var response = await _httpClient.GetAsync($"https://reqres.in/api/users/{id}");
            if (!response.IsSuccessStatusCode)
            {
                Log.Warning("User with ID {Id} not found in external API", id);
                return null; // повертаємо null, якщо користувача не знайдено
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var userResponse = JsonSerializer.Deserialize<ReqresUserResponse>(responseContent, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            //var userResponse = JsonSerializer.Deserialize<ReqresUserResponse>(responseContent);

            // Додаємо користувача в кеш
            // Логування додавання користувача в кеш
            Log.Information("Adding user with ID {Id} to cache", id);
            _userCache[id] = userResponse.Data;

            return userResponse.Data;
        }
    }
}
