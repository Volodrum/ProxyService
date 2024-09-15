using System.Text.Json.Serialization;

namespace ProxyService.Interfaces
{
    public interface IUser
    {
        int Id { get; set; }
        string Email { get; set; }
        string FirstName { get; set; }
        string Avatar { get; set; }
    }
}
