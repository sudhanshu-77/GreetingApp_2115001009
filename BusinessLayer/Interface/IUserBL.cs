using System;
using ModelLayer.Entity;
using ModelLayer.Model;

namespace BusinessLayer.Interface
{
    /// <summary>
    /// Interface defining business logic operations for user management.
    /// </summary>
    public interface IUserBL
    {
        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="registerDTO">User registration details.</param>
        /// <returns>Registered user entity.</returns>
        UserEntity RegistrationBL(RegisterModel registerDTO);

        /// <summary>
        /// Logs in a user and generates a JWT token upon successful authentication.
        /// </summary>
        /// <param name="loginDTO">User login details.</param>
        /// <returns>Tuple containing user entity and JWT token.</returns>
        (UserEntity user, string token) LoginnUserBL(LoginModel loginDTO);

        /// <summary>
        /// Validates whether an email exists in the system.
        /// </summary>
        /// <param name="email">Email to be validated.</param>
        /// <returns>Boolean indicating whether the email is valid.</returns>
        bool ValidateEmail(string email);

        /// <summary>
        /// Updates the password of a user based on their email.
        /// </summary>
        /// <param name="email">User email.</param>
        /// <param name="newPassword">New password to be updated.</param>
        /// <returns>Boolean indicating whether the update was successful.</returns>
        bool UpdateUserPassword(string email, string newPassword);

        /// <summary>
        /// Retrieves user details by email.
        /// </summary>
        /// <param name="email">Email of the user.</param>
        /// <returns>User entity if found, otherwise null.</returns>
        UserEntity GetByEmail(string email);
    }
}
