using Microsoft.AspNetCore.Mvc;
using ModelLayer.Model;
using NLog;
using BusinessLayer.Interface;
using Middleware.HashingAlgo;


namespace HelloGreetingApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IUserBL _userBL;

        public UserController(IUserBL userBL)
        {
            _userBL = userBL;
        }

        [HttpPost("registerUser")]
        public IActionResult Register(RegisterModel registerModel)
        {
            try
            {
                _logger.Info($"Register attempt for email: {registerModel.Email}");

                var newUser = _userBL.RegistrationBL(registerModel);

                if (newUser == null)
                {
                    _logger.Warn($"Registration failed. Email already exists: {registerModel.Email}");
                    return Conflict(new { Success = false, Message = "User with this email already exists." });
                }

                _logger.Info($"User registered successfully: {registerModel.Email}");
                return Created("user registered", new { Success = true, Message = "User registered successfully." });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Registration failed for {registerModel.Email}");
                return BadRequest(new { Success = false, Message = "Registration failed.", Error = ex.Message });
            }
        }

        [HttpPost("loginUser")]
        public IActionResult PostData(LoginModel loginModel)
        {
            try
            {
                _logger.Info($"Login attempt for user: {loginModel.Email}");

                var user = _userBL.LoginnUserBL(loginModel);

                if (user == null)
                {
                    _logger.Warn($"Invalid login attempt for user: {loginModel.Email}");
                    return Unauthorized(new { Success = false, Message = "Invalid username or password." });
                }

                _logger.Info($"User {loginModel.Email} logged in successfully.");
                return Ok(new { Success = true, Message = "Login Successful." });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Login failed for {loginModel.Email}");
                return BadRequest(new { Success = false, Message = "Login failed.", Error = ex.Message });
            }
        }
    }
}
