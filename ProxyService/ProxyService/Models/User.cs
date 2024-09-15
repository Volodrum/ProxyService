using ProxyService.Interfaces;
using System.Text.Json.Serialization;

namespace MicroServices1.Models
{
    public class User : IUser
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }
        [JsonPropertyName("last_name")]
        public string LastName { get; set; }
        [JsonPropertyName("avatar")]
        public string Avatar { get; set; }
    }
}
