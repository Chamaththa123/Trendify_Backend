/************************************************************
 * File:        IUserService.cs
 * Author:      IT21252754 - Madhumalka K.C.S
 * Date:        2024-09-17
 * Description: This file defines the IUserService interface, which 
 *              outlines the contract for user-related operations 
 *              such as registration, authentication, and user management.
 ************************************************************/

using WebService.Models;

namespace WebService.Interfaces
{
    public interface IUserService
    {
        // Register a new user
        Task<User> RegisterUser(User user);

        // Retrieve a user by email (nullable, if no user found)
        Task<User?> GetUserByEmail(string email);

        // Authenticate a user by their email and password
        Task<User?> AuthenticateUser(string email, string password);

        //get all users
        Task<List<User>> GetAllUsers(string role);

        //get specific user by id
        Task<User?> GetUserById(string id);

        //activate or deactivate user
        Task ChangeUserStatus(string id);

        // Retrieve all users with the 'Vendor' role
        Task<List<User>> GetVendors();

        // Retrieve all users with the 'Customer Service Representative' (CSR) role
        Task<List<User>> GetCSRs();

        // Retrieve all users with the 'Customer' role
        Task<List<User>> GetCustomers();

        // Retrieve all customers with pending approval
        Task<List<User>> GetPendingCustomers();

        // Change a user's password, given their email, current password, and new password
        Task<bool> ChangePassword(string email, string currentPassword, string newPassword);

        // Update details of a user by their ID with the provided updated user information
        Task<bool> UpdateUserDetails(string id, User updatedUser);
    }
}
