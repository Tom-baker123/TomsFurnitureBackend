using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OA.Domain.Common.Models;
using System.Threading.Tasks;
using TomsFurnitureBackend.Services.Interfaces;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Maps user role to appropriate redirect URL.
        /// </summary>
        private static string GetRedirectUrl(string roleName)
        {
            return roleName switch
            {
                "Admin" => "/admin",
                "User" => "/",
                _ => "/"
            };
        }

        /// <summary>
        /// Checks the authentication status of the current user.
        /// </summary>
        [HttpGet("status")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAuthStatus()
        {
            try
            {
                _logger.LogInformation("Checking authentication status for user.");
                var result = await _authService.GetAuthStatusAsync(User, HttpContext);
                if (!result.IsAuthenticated)
                {
                    _logger.LogInformation("User is not authenticated: {Message}", result.Message);
                    return Ok(new
                    {
                        result.IsAuthenticated,
                        result.Message
                    });
                }

                _logger.LogInformation("User authenticated: {UserName}, Role: {Role}", result.UserName, result.Role);
                return Ok(new
                {
                    result.IsAuthenticated,
                    result.UserName,
                    result.Email,
                    result.Role,
                    RedirectUrl = GetRedirectUrl(result.Role)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking auth status for user.");
                return StatusCode(500, new { Message = "An error occurred while checking auth status.", Error = ex.Message });
            }
        }

        /// <summary>
        /// Authenticates a user with provided credentials.
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginVModel model)
        {
            try
            {
                if (model == null)
                {
                    _logger.LogWarning("Login attempt with null model.");
                    return BadRequest(new { Message = "Dữ liệu đăng nhập không hợp lệ." });
                }

                _logger.LogInformation("Login attempt for email: {Email}", model.Email);
                var result = await _authService.LoginAsync(model, HttpContext);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Login failed for {Email}: {Message}", model.Email, result.Message);
                    return BadRequest(new { Message = result.Message });
                }

                var successResult = (SuccessResponseResult)result;
                var userData = (LoginResultVModel)successResult.Data;
                _logger.LogInformation("Login successful for {Email}, Role: {Role}", model.Email, userData.Role);
                return Ok(new
                {
                    Message = successResult.Message,
                    UserName = userData.UserName,
                    Role = userData.Role,
                    RedirectUrl = GetRedirectUrl(userData.Role)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Email}.", model?.Email ?? "unknown");
                return StatusCode(500, new { Message = "An error occurred during login.", Error = ex.Message });
            }
        }

        /// <summary>
        /// Logs out the current user.
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                _logger.LogInformation("Logout attempt for user: {UserName}", User.Identity?.Name ?? "unknown");
                var result = await _authService.LogoutAsync(HttpContext);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Logout failed: {Message}", result.Message);
                    return BadRequest(new { Message = result.Message });
                }

                _logger.LogInformation("Logout successful for user: {UserName}", User.Identity?.Name ?? "unknown");
                return Ok(new { Message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout for user: {UserName}.", User.Identity?.Name ?? "unknown");
                return StatusCode(500, new { Message = "An error occurred during logout.", Error = ex.Message });
            }
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterVModel model)
        {
            try
            {
                if (model == null)
                {
                    _logger.LogWarning("Register attempt with null model.");
                    return BadRequest(new { Message = "Dữ liệu đăng ký không hợp lệ." });
                }

                _logger.LogInformation("Register attempt for email: {Email}", model.Email);
                var result = await _authService.RegisterAsync(model);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Register failed for {Email}: {Message}", model.Email, result.Message);
                    return BadRequest(new { Message = result.Message });
                }

                _logger.LogInformation("Register successful for {Email}", model.Email);
                return Ok(new { Message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during register for {Email}.", model?.Email ?? "unknown");
                return StatusCode(500, new { Message = "An error occurred during registration.", Error = ex.Message });
            }
        }

        /// <summary>
        /// Verifies OTP for user account activation.
        /// </summary>
        [HttpPost("verify-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyOtp([FromBody] ConfirmOtpVModel model)
        {
            try
            {
                if (model == null)
                {
                    _logger.LogWarning("Verify OTP attempt with null model.");
                    return BadRequest(new { Message = "Dữ liệu OTP không hợp lệ." });
                }

                _logger.LogInformation("Verify OTP attempt for email: {Email}", model.Email);
                var result = await _authService.VerifyOtpAsync(model);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Verify OTP failed for {Email}: {Message}", model.Email, result.Message);
                    return BadRequest(new { Message = result.Message });
                }

                _logger.LogInformation("Verify OTP successful for {Email}", model.Email);
                return Ok(new { Message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during OTP verification for {Email}.", model?.Email ?? "unknown");
                return StatusCode(500, new { Message = "An error occurred during OTP verification.", Error = ex.Message });
            }
        }

        /// <summary>
        /// Resends OTP to the specified email.
        /// </summary>
        [HttpPost("resend-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpVModel model)
        {
            try
            {
                if (model == null || string.IsNullOrWhiteSpace(model.Email))
                {
                    _logger.LogWarning("Resend OTP attempt with invalid email.");
                    return BadRequest(new { Message = "Email là bắt buộc." });
                }

                _logger.LogInformation("Resend OTP attempt for email: {Email}", model.Email);
                var result = await _authService.ResendOtpAsync(model.Email);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Resend OTP failed for {Email}: {Message}", model.Email, result.Message);
                    return BadRequest(new { Message = result.Message });
                }

                _logger.LogInformation("Resend OTP successful for {Email}", model.Email);
                return Ok(new { Message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during OTP resend for {Email}.", model?.Email ?? "unknown");
                return StatusCode(500, new { Message = "An error occurred during OTP resend.", Error = ex.Message });
            }
        }

        // Endpoint mới: Xem tất cả người dùng (chỉ admin)
        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                _logger.LogInformation("Yêu cầu lấy danh sách tất cả người dùng bởi admin: {AdminName}", User.Identity?.Name);
                var result = await _authService.GetAllUsersAsync();
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Lấy danh sách người dùng thất bại: {Message}", result.Message);
                    return BadRequest(new { Message = result.Message });
                }

                var successResult = (SuccessResponseResult)result;
                _logger.LogInformation("Lấy danh sách người dùng thành công.");
                return Ok(new { Message = successResult.Message, Users = successResult.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách người dùng.");
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi lấy danh sách người dùng.", Error = ex.Message });
            }
        }

        // Endpoint mới: Xem người dùng theo ID (chỉ admin)
        [HttpGet("users/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                _logger.LogInformation("Yêu cầu lấy thông tin người dùng {UserId} bởi admin: {AdminName}", id, User.Identity?.Name);
                var result = await _authService.GetUserByIdAsync(id);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Lấy thông tin người dùng {UserId} thất bại: {Message}", id, result.Message);
                    return BadRequest(new { Message = result.Message });
                }

                var successResult = (SuccessResponseResult)result;
                _logger.LogInformation("Lấy thông tin người dùng {UserId} thành công.", id);
                return Ok(new { Message = successResult.Message, User = successResult.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin người dùng {UserId}.", id);
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi lấy thông tin người dùng.", Error = ex.Message });
            }
        }

        // Endpoint mới: Xóa người dùng (chỉ admin)
        [HttpDelete("users/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                _logger.LogInformation("Yêu cầu xóa người dùng {UserId} bởi admin: {AdminName}", id, User.Identity?.Name);
                var result = await _authService.DeleteUserAsync(id);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Xóa người dùng {UserId} thất bại: {Message}", id, result.Message);
                    return BadRequest(new { Message = result.Message });
                }

                _logger.LogInformation("Xóa người dùng {UserId} thành công.", id);
                return Ok(new { Message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa người dùng {UserId}.", id);
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi xóa người dùng.", Error = ex.Message });
            }
        }

        /// <summary>
        /// Requests a password reset OTP for the specified email.
        /// </summary>
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordVModel model)
        {
            try
            {
                if (model == null || string.IsNullOrWhiteSpace(model.Email))
                {
                    _logger.LogWarning("Forgot password attempt with invalid email.");
                    return BadRequest(new { Message = "Email is required." });
                }

                _logger.LogInformation("Forgot password attempt for email: {Email}", model.Email);
                var result = await _authService.ForgotPasswordAsync(model);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Forgot password failed for {Email}: {Message}", model.Email, result.Message);
                    return BadRequest(new { Message = result.Message });
                }

                _logger.LogInformation("Forgot password successful for {Email}", model.Email);
                return Ok(new { Message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during forgot password for {Email}.", model?.Email ?? "unknown");
                return StatusCode(500, new { Message = "An error occurred during forgot password.", Error = ex.Message });
            }
        }

        /// <summary>
        /// Resets the user's password using the provided OTP and new password.
        /// </summary>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordVModel model)
        {
            try
            {
                if (model == null)
                {
                    _logger.LogWarning("Reset password attempt with null model.");
                    return BadRequest(new { Message = "Dữ liệu đặt lại mật khẩu không hợp lệ." });
                }

                _logger.LogInformation("Reset password attempt for email: {Email}", model.Email);
                var result = await _authService.ResetPasswordAsync(model);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Reset password failed for {Email}: {Message}", model.Email, result.Message);
                    return BadRequest(new { Message = result.Message });
                }

                _logger.LogInformation("Reset password successful for {Email}", model.Email);
                return Ok(new { Message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during reset password for {Email}.", model?.Email ?? "unknown");
                return StatusCode(500, new { Message = "An error occurred during reset password.", Error = ex.Message });
            }
        }

        /// <summary>
        /// Adds a new user (admin only).
        /// </summary>
        [HttpPost("users/add")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddUser([FromBody] AddUserVModel model)
        {
            try
            {
                if (model == null)
                {
                    _logger.LogWarning("Add user attempt with null model.");
                    return BadRequest(new { Message = "Dữ liệu người dùng không hợp lệ." });
                }

                _logger.LogInformation("Add user attempt for email: {Email} by admin: {AdminName}", model.Email, User.Identity?.Name);
                var result = await _authService.AddUserAsync(model);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Add user failed for {Email}: {Message}", model.Email, result.Message);
                    return BadRequest(new { Message = result.Message });
                }

                var successResult = (SuccessResponseResult)result;
                _logger.LogInformation("Add user successful for {Email}", model.Email);
                return Ok(new { Message = successResult.Message, User = successResult.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during add user for {Email}.", model?.Email ?? "unknown");
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi thêm người dùng.", Error = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing user (admin only).
        /// </summary>
        [HttpPut("users/update/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserVModel model)
        {
            try
            {
                if (model == null)
                {
                    _logger.LogWarning("Update user attempt with null model.");
                    return BadRequest(new { Message = "Dữ liệu người dùng không hợp lệ." });
                }

                _logger.LogInformation("Update user attempt for ID: {UserId} by admin: {AdminName}", id, User.Identity?.Name);
                var result = await _authService.UpdateUserAsync(id, model);
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Update user failed for ID {UserId}: {Message}", id, result.Message);
                    return BadRequest(new { Message = result.Message });
                }

                var successResult = (SuccessResponseResult)result;
                _logger.LogInformation("Update user successful for ID {UserId}", id);
                return Ok(new { Message = successResult.Message, User = successResult.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during update user for ID {UserId}.", id);
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi cập nhật người dùng.", Error = ex.Message });
            }
        }
    }
}