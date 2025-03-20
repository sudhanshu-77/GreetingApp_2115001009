using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ModelLayer.Model;
using ModelLayer.Entity;
using RepositoryLayer.Contexts;
using Middleware.HashingAlgo;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Service
{
    public class UserRL : IUserRL
    {
        private readonly ILogger<UserRL> _logger;
        private readonly GreetingAppContext _dbContext;

        public UserRL(ILogger<UserRL> logger, GreetingAppContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Registers a new user in the repository.
        /// </summary>
        /// <param name="registerModel">The model containing registration details.</param>
        /// <returns>Returns the created UserEntity object if registration is successful; otherwise, null.</returns>
        public UserEntity Registration(RegisterModel registerModel)
        {
            try
            {
                _logger.LogInformation("Attempting to register user: {Email}", registerModel.Email);

                var existingUser = _dbContext.Users.FirstOrDefault(e => e.Email == registerModel.Email);
                if (existingUser == null)
                {
                    var hashedPassword = HashingMethods.HashPassword(registerModel.password); // Hash password

                    var newUser = new UserEntity
                    {
                        FirstName = registerModel.firstName,
                        LastName = registerModel.lastName,
                        Password = hashedPassword, // Store hashed password
                        Email = registerModel.Email
                    };

                    _dbContext.Users.Add(newUser);
                    _dbContext.SaveChanges();

                    _logger.LogInformation("User registered successfully: {Email}", registerModel.Email);
                    return newUser;
                }

                _logger.LogWarning("User already exists: {Email}", registerModel.Email);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for {Email}", registerModel.Email);
                throw;
            }
        }

        /// <summary>
        /// Logs in a user and retrieves their information.
        /// </summary>
        /// <param name="loginModel">The model containing login credentials.</param>
        /// <returns>Returns the UserEntity object associated with the logged-in user if successful; otherwise, null.</returns>
        public UserEntity LoginnUserRL(LoginModel loginModel)
        {
            try
            {
                _logger.LogInformation("User attempting to log in: {Email}", loginModel.Email);

                var user = _dbContext.Users.FirstOrDefault(e => e.Email == loginModel.Email);
                if (user != null && HashingMethods.VerifyPassword(loginModel.Password, user.Password)) // Verify hashed password
                {
                    _logger.LogInformation("Login successful for user: {Email}", loginModel.Email);
                    return user;
                }

                _logger.LogWarning("Invalid login attempt for user: {Email}", loginModel.Email);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login for {Email}", loginModel.Email);
                throw;
            }
        }

        /// <summary>
        /// Validates if the provided email exists in the system.
        /// </summary>
        /// <param name="email">The email address to validate.</param>
        /// <returns>Returns true if the email exists; otherwise, false.</returns>
        public bool ValidateEmail(string email)
        {
            var data = _dbContext.Users.FirstOrDefault(e => e.Email == email);
            return data != null;
        }

        /// <summary>
        /// Finds a user by their email address.
        /// </summary>
        /// <param name="email">The email address of the user to find.</param>
        /// <returns>Returns the UserEntity object if found; otherwise, null.</returns>
        public UserEntity FindByEmail(string email)
        {
            return _dbContext.Users.FirstOrDefault(e => e.Email == email);
        }

        /// <summary>
        /// Updates the details of an existing user.
        /// </summary>
        /// <param name="user">The UserEntity object containing updated user information.</param>
        /// <returns>Returns true if the update was successful; otherwise, false.</returns>
        public bool Update(UserEntity user)
        {
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();
            return true;
        }
    }
}
