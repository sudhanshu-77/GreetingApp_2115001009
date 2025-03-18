using Microsoft.AspNetCore.Mvc;
using NLog;
using BusinessLayer.Interface;
using Middleware.HashingAlgo;
using Middleware.JwtHelper;
using ModelLayer.Model;
using System.Security.Claims;
using Middleware.Email;
using System.IdentityModel.Tokens.Jwt;

namespace HelloGreetingApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IUserBL _userBL;
        private readonly JwtTokenHelper _jwtTokenHelper;
        private readonly SMTP _smtp;
        public UserController(IUserBL userBL, JwtTokenHelper jwtTokenHelper, SMTP smtp)
        {
            _userBL = userBL;
            _jwtTokenHelper = jwtTokenHelper;
            _smtp = smtp;
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
        public IActionResult Login(LoginModel loginModel)
        {
            try
            {
                _logger.Info($"Login attempt for user: {loginModel.Email}");

                var (user, token) = _userBL.LoginnUserBL(loginModel);

                if (user == null || string.IsNullOrEmpty(token))
                {
                    _logger.Warn($"Invalid login attempt for user: {loginModel.Email}");
                    return Unauthorized(new { Success = false, Message = "Invalid username or password." });
                }

                _logger.Info($"User {loginModel.Email} logged in successfully.");
                return Ok(new
                {
                    Success = true,
                    Message = "Login Successful.",
                    Token = token
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Login failed for {loginModel.Email}");
                return BadRequest(new { Success = false, Message = "Login failed.", Error = ex.Message });
            }
        }

        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordModel request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email))
                {
                    return BadRequest(new { message = "Email is required." });
                }

                bool result = _userBL.ValidateEmail(request.Email);

                Console.WriteLine("result: " + result);

                if (!result)
                {
                    return Ok(new { message = "Not a valid email" });
                }

                string mail = request.Email;
                // Generate reset token
                var resetToken = _jwtTokenHelper.GeneratePasswordResetToken(mail);

                // Email details
                string subject = "Reset Your Password";
                string body = $"Click the link to reset your password: \n https://AdressBook.com/reset-password?token={resetToken}";

                // Send email using SMTP
                _smtp.SendEmailAsync(request.Email, subject, body);

                return Ok(new { message = "Password reset email has been sent." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error occurred while processing the password reset", error = ex.Message });
            }
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordModel model)
        {
            try
            {


                if (string.IsNullOrEmpty(model.Token))
                    return BadRequest(new ResponseModel<string> { Success = false, Message = "Invalid token." });

                var principal = _jwtTokenHelper.ValidateToken(model.Token);
                if (principal == null)
                    return BadRequest(new ResponseModel<string> { Success = false, Message = "Invalid or expired token." });

                // Extract email from token
                var emailClaim = principal.FindFirst(ClaimTypes.Email)?.Value
                              ?? principal.FindFirst(JwtRegisteredClaimNames.Email)?.Value;

                var isResetTokenClaim = principal.FindFirst("isPasswordReset")?.Value; // Assuming you store it in token

                if (string.IsNullOrEmpty(isResetTokenClaim) || isResetTokenClaim != "true")
                {
                    return BadRequest(new ResponseModel<string> { Success = false, Message = "Invalid reset token." });
                }

                if (emailClaim != model.Email)
                {
                    return BadRequest(new ResponseModel<string> { Success = false, Message = "Invalid Email." });
                }

                if (string.IsNullOrEmpty(emailClaim))
                    return BadRequest(new ResponseModel<string> { Success = false, Message = "Email not found in token." });

                var user = _userBL.GetByEmail(emailClaim);
                if (user == null)
                    return NotFound(new ResponseModel<string> { Success = false, Message = "User not found" });

                // Securely hash the new password and update it
                string password = HashingMethods.HashPassword(model.NewPassword);

                _userBL.UpdateUserPassword(model.Email, password);

                return Ok(new ResponseModel<string> { Success = true, Message = "Password reset successfully" });
            }
            catch (Exception e)
            {
                return BadRequest(new ResponseModel<string> { Success = false, Message = e.Message });
            }
        }

    }
}