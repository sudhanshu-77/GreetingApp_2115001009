using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Model;
using ModelLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface IUserRL
    {
        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="registerModel">The model containing the registration details of the user.</param>
        /// <returns>Returns the created UserEntity object.</returns>
        UserEntity Registration(RegisterModel registerModel);

        /// <summary>
        /// Logs in a user and retrieves their information.
        /// </summary>
        /// <param name="loginModel">The model containing the login credentials of the user.</param>
        /// <returns>Returns the UserEntity object associated with the logged-in user.</returns>
        UserEntity LoginnUserRL(LoginModel loginModel);

        /// <summary>
        /// Validates if the provided email exists in the system.
        /// </summary>
        /// <param name="email">The email address to validate.</param>
        /// <returns>Returns true if the email exists; otherwise, false.</returns>
        bool ValidateEmail(string email);

        /// <summary>
        /// Finds a user by their email address.
        /// </summary>
        /// <param name="email">The email address of the user to find.</param>
        /// <returns>Returns the UserEntity object if found; otherwise, null.</returns>
        UserEntity FindByEmail(string email);

        /// <summary>
        /// Updates the details of an existing user.
        /// </summary>
        /// <param name="user">The UserEntity object containing updated user information.</param>
        /// <returns>Returns true if the update was successful; otherwise, false.</returns>
        bool Update(UserEntity user);
    }
}
