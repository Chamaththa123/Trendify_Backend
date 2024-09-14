using WebService.Models;

namespace WebService.Interfaces
{
    public interface IUserService
    {
        Task RegisterUser(User newUser);
        Task<User?> GetUserByEmail(string email);
        Task<User?> AuthenticateUser(string email, string password);
    }
}
