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
        private readonly ILogger<UserRL> _logger; 
        private readonly GreetingAppContext _dbContext;

        public UserRL(ILogger<UserRL> logger, GreetingAppContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Registration in Repo layer
        /// </summary>
        /// <param name="registerModel"></param>
        /// <returns></returns>
        public UserEntity Registration(RegisterModel registerModel)
        {
            try
            {
                _logger.LogInformation("Attempting to register user: {Email}", registerModel.Email);

                var existingUser = _dbContext.Users.FirstOrDefault(e => e.Email == registerModel.Email);
                if (existingUser == null)
                {
                    var hashedPassword = HashingMethods.HashPassword(registerModel.password); // ✅ Hash password

                    var newUser = new UserEntity
                    {
                        FirstName = registerModel.firstName,
                        LastName = registerModel.lastName,
                        Password = hashedPassword, // ✅ Store hashed password
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

        public UserEntity LoginnUserRL(LoginModel loginModel)
        {
            try
            {
                _logger.LogInformation("User attempting to log in: {Email}", loginModel.Email);

                var user = _dbContext.Users.FirstOrDefault(e => e.Email == loginModel.Email);
                if (user != null && HashingMethods.VerifyPassword(loginModel.Password, user.Password)) // ✅ Verify hashed password
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

        public bool ValidateEmail(string email)
        {
            var data = _dbContext.Users.FirstOrDefault(e => e.Email == email);

            if (data == null)
            {
                return false;
            }
            return true;
        }

        public UserEntity FindByEmail(string email)
        {
            return _dbContext.Users.FirstOrDefault(e => e.Email == email);
        }

        public bool Update(UserEntity user)
        {
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();
            return true;
        }
    }
}