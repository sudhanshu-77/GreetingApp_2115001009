using System;
using ModelLayer.Model;
using Microsoft.Extensions.Logging;
using ModelLayer.Entity;
using RepositoryLayer.Interface;
using BusinessLayer.Interface;
using Middleware.JwtHelper;

namespace BusinessLayer.Service
{
    /// <summary>
    /// Business Layer class for user management functionalities.
    /// </summary>
    public class UserBL : IUserBL
    {
        private readonly ILogger<UserBL> _logger;
        private readonly IUserRL _userRL;
        private readonly JwtTokenHelper _jwtTokenHelper;

        /// <summary>
        /// Constructor for UserBL.
        /// </summary>
        /// <param name="userRL">Repository layer dependency for user operations</param>
        /// <param name="logger">Logger for logging user-related operations</param>
        /// <param name="jwtTokenHelper">Helper for generating JWT tokens</param>
        public UserBL(IUserRL userRL, ILogger<UserBL> logger, JwtTokenHelper jwtTokenHelper)
        {
            _logger = logger;
            _userRL = userRL;
            _jwtTokenHelper = jwtTokenHelper;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="registerDTO">User registration details</param>
        /// <returns>Registered user entity</returns>
        public UserEntity RegistrationBL(RegisterModel registerDTO)
        {
            try
            {
                _logger.LogInformation("Attempting to register user: {Email}", registerDTO.Email);
                var result = _userRL.Registration(registerDTO);
                if (result != null)
                {
                    _logger.LogInformation("User registration successful for: {Email}", registerDTO.Email);
                }
                else
                {
                    _logger.LogWarning("User registration failed for: {Email}", registerDTO.Email);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration for {Email}", registerDTO.Email);
                throw;
            }
        }

        /// <summary>
        /// Logs in a user and generates a JWT token upon successful authentication.
        /// </summary>
        /// <param name="loginDTO">User login details</param>
        /// <returns>Tuple containing user entity and JWT token</returns>
        public (UserEntity user, string token) LoginnUserBL(LoginModel loginDTO)
        {
            try
            {
                _logger.LogInformation("Attempting to log in user: {Email}", loginDTO.Email);
                var user = _userRL.LoginnUserRL(loginDTO);

                if (user != null)
                {
                    _logger.LogInformation("Login successful for user: {Email}", loginDTO.Email);
                    var token = _jwtTokenHelper.GenerateToken(user);
                    return (user, token);
                }

                _logger.LogWarning("Login failed for user: {Email}", loginDTO.Email);
                return (null, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Email}", loginDTO.Email);
                throw;
            }
        }

        /// <summary>
        /// Updates the password of a user based on their email.
        /// </summary>
        /// <param name="email">User email</param>
        /// <param name="newPassword">New password to be updated</param>
        /// <returns>Boolean indicating whether the update was successful</returns>
        public bool UpdateUserPassword(string email, string newPassword)
        {
            _logger.LogInformation("Attempting to update password for user: {Email}", email);

            var user = _userRL.FindByEmail(email);
            if (user == null)
            {
                _logger.LogWarning("User not found for password update: {Email}", email);
                return false;
            }

            user.Password = newPassword;
            bool result = _userRL.Update(user);

            if (result)
                _logger.LogInformation("Password updated successfully for user: {Email}", email);
            else
                _logger.LogWarning("Password update failed for user: {Email}", email);

            return result;
        }

        /// <summary>
        /// Retrieves user details by email.
        /// </summary>
        /// <param name="email">Email of the user</param>
        /// <returns>User entity if found, otherwise null</returns>
        public UserEntity GetByEmail(string email)
        {
            _logger.LogInformation("Fetching user details by email: {Email}", email);
            return _userRL.FindByEmail(email);
        }

        /// <summary>
        /// Validates whether an email exists in the system.
        /// </summary>
        /// <param name="email">Email to be validated</param>
        /// <returns>Boolean indicating whether the email is valid</returns>
        public bool ValidateEmail(string email)
        {
            _logger.LogInformation("Validating email: {Email}", email);
            return _userRL.ValidateEmail(email);
        }
    }
}
