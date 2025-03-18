using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ModelLayer.Model;
using RepositoryLayer.Entity;
using RepositoryLayer.Contexts;
using Middleware.HashingAlgo;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Service
{
    public class UserRL : IUserRL
    {
        private readonly GreetingAppContext _dbContext;
        private readonly ILogger<UserRL> _logger; 

        public UserRL(ILogger<UserRL> logger, GreetingAppContext dbContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); 
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext)); 
        }

        /// <summary>
        /// Registration in Repo layer
        /// </summary>
        /// <param name="registerModel"></param>
        /// <returns></returns>
        public UserEntity Registration(RegisterModel registerModel)
        {
            if (registerModel == null)
            {
                _logger.LogError("Registration failed: registerModel is null.");
                throw new ArgumentNullException(nameof(registerModel));
            }

            try
            {
                _logger.LogInformation("Attempting to register user: {Email}", registerModel.Email);

                if (_dbContext.Users.Any(e => e.Email == registerModel.Email)) 
                {
                    _logger.LogWarning("User already exists: {Email}", registerModel.Email);
                    return null;
                }

                var hashedPassword = HashingMethods.HashPassword(registerModel.password);

                var newUser = new UserEntity
                {
                    FirstName = registerModel.firstName,
                    LastName = registerModel.lastName,
                    Password = hashedPassword,
                    Email = registerModel.Email
                };

                _dbContext.Users.Add(newUser);
                _dbContext.SaveChanges();

                _logger.LogInformation("User registered successfully: {Email}", registerModel.Email);
                return newUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for {Email}", registerModel.Email ?? "Unknown");
                throw;
            }
        }

        /// <summary>
        /// Login user in Repo layer
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        public UserEntity LoginnUserRL(LoginModel loginModel)
        {
            if (loginModel == null)
            {
                _logger.LogError("Login failed: loginModel is null.");
                throw new ArgumentNullException(nameof(loginModel));
            }

            try
            {
                _logger.LogInformation("User attempting to log in: {Email}", loginModel.Email);

                var user = _dbContext.Users
                    .Where(e => e.Email == loginModel.Email)
                    .Select(e => new { e.UserId, e.Email, e.Password, e.FirstName, e.LastName }) 
                    .FirstOrDefault();

                if (user != null && HashingMethods.VerifyPassword(loginModel.Password, user.Password))
                {
                    _logger.LogInformation("Login successful for user: {Email}", loginModel.Email);

                    // Returning a mapped entity instead of an anonymous type
                    return new UserEntity
                    {
                        UserId = user.UserId,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName
                    };
                }

                _logger.LogWarning("Invalid login attempt for user: {Email}", loginModel.Email);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login for {Email}", loginModel.Email ?? "Unknown");
                throw;
            }
        }
    }
}
