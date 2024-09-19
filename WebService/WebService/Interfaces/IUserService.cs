using WebService.Models;

namespace WebService.Interfaces
{
    public interface IUserService
    {
        Task<User> RegisterUser(User user);
        Task<User?> GetUserByEmail(string email);
        Task<User?> AuthenticateUser(string email, string password);

        //get all users
        Task<List<User>> GetAllUsers(string role);

        //get specific user by id
        Task<User?> GetUserById(string id);

        //activate or deactivate user
        Task ChangeUserStatus(string id);
    }
}
