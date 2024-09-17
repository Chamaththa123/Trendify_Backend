using WebService.Models;

namespace WebService.Interfaces
{
    public interface IUserService
    {
        Task<User> RegisterUser(User user);
        Task<User?> GetUserByEmail(string email);
        Task<User?> AuthenticateUser(string email, string password);
    }
}
