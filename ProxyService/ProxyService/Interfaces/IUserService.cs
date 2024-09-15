using MicroServices1.Models;

namespace ProxyService.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserById(int id);
    }
}
