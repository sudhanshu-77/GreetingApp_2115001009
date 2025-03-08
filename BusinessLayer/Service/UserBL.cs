using System;
using ModelLayer.Model;
using Microsoft.Extensions.Logging;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using BusinessLayer.Interface;

namespace BusinessLayer.Service
{
    public class UserBL : IUserBL
    {
        private readonly ILogger<UserBL> _logger;
        private readonly IUserRL _userRL;

        public UserBL(IUserRL userRL, ILogger<UserBL> logger)
        {
            _logger = logger;
            _userRL = userRL;
        }

        public UserEntity RegistrationBL(RegisterModel registerModel)
        {
            try
            {
                _logger.LogInformation("Attempting to register user: {Email}", registerModel.Email);

                var result = _userRL.Registration(registerModel);
                if (result != null)
                {
                    _logger.LogInformation("User registration successful for: {Email}", registerModel.Email);
                }
                else
                {
                    _logger.LogWarning("User registration failed for: {Email}", registerModel.Email);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration for {Email}", registerModel.Email);
                throw;
            }
        }

        public UserEntity LoginnUserBL(LoginModel loginModel)
        {
            try
            {
                _logger.LogInformation("Attempting to log in user: {Email}", loginModel.Email);

                var result = _userRL.LoginnUserRL(loginModel);
                if (result != null)
                {
                    _logger.LogInformation("Login successful for user: {Email}", loginModel.Email);
                }
                else
                {
                    _logger.LogWarning("Login failed for user: {Email}", loginModel.Email);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Email}", loginModel.Email);
                throw;
            }
        }
    }
}
