using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OA.Domain.Common.Models;
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
            // Bước 1: Khởi tạo các dependency
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Maps user role to appropriate redirect URL.
        /// </summary>
        private static string GetRedirectUrl(string? roleName)
        {
            // Bước 1: Kiểm tra roleName có null hoặc rỗng không, trả về URL mặc định nếu null
            if (string.IsNullOrEmpty(roleName))
                return "/";
            // Bước 2: Ánh xạ vai trò sang URL chuyển hướng tương ứng
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
                // Bước 1: Ghi log yêu cầu kiểm tra trạng thái xác thực
                _logger.LogInformation("Checking authentication status for user.");
                // Bước 2: Gọi service để lấy trạng thái xác thực
                var result = await _authService.GetAuthStatusAsync(User, HttpContext);
                // Bước 3: Kiểm tra xem người dùng đã xác thực chưa
                if (!result.IsAuthenticated)
                {
                    _logger.LogInformation("User is not authenticated: {Message}", result.Message);
                    // Bước 4: Trả về phản hồi cho người dùng chưa xác thực
                    return Ok(new
                    {
                        result.IsAuthenticated,
                        result.Message
                    });
                }
                // Bước 5: Ghi log thông tin người dùng đã xác thực
                _logger.LogInformation("User authenticated: {UserName}, Role: {Role}", result.UserName, result.Role);
                // Bước 6: Trả về phản hồi cho người dùng đã xác thực
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
                // Bước 7: Ghi log và trả về lỗi nếu có
                _logger.LogError(ex, "Error checking auth status for user.");
                return StatusCode(500, new { Message = "An error occurred while checking authentication status.", Error = ex.Message });
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
                // Bước 1: Kiểm tra dữ liệu đầu vào
                if (model == null)
                {
                    _logger.LogWarning("Login attempt with null model.");
                    return BadRequest(new { Message = "Invalid login data." });
                }
                // Bước 2: Ghi log yêu cầu đăng nhập
                _logger.LogInformation("Login attempt for email: {Email}", model.Email);
                // Bước 3: Gọi service để xử lý đăng nhập
                var result = await _authService.LoginAsync(model, HttpContext);
                // Bước 4: Kiểm tra kết quả đăng nhập
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Login failed for {Email}: {Message}", model.Email, result.Message);
                    return BadRequest(new { Message = result.Message });
                }
                // Bước 5: Lấy dữ liệu người dùng từ kết quả
                var successResult = (SuccessResponseResult)result;
                if (successResult.Data == null)
                {
                    _logger.LogWarning("Login failed for {Email}: No user data returned.", model.Email);
                    return BadRequest(new { Message = "No user data returned from login." });
                }
                var userData = (LoginResultVModel)successResult.Data;
                // Bước 6: Ghi log đăng nhập thành công
                _logger.LogInformation("Login successful for {Email}, Role: {Role}", model.Email, userData.Role);
                // Bước 7: Trả về phản hồi thành công
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
                // Bước 8: Ghi log và trả về lỗi nếu có
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
                // Bước 1: Ghi log yêu cầu đăng xuất
                _logger.LogInformation("Logout attempt for user: {UserName}", User.Identity?.Name ?? "unknown");
                // Bước 2: Gọi service để xử lý đăng xuất
                var result = await _authService.LogoutAsync(HttpContext);
                // Bước 3: Kiểm tra kết quả đăng xuất
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Logout failed: {Message}", result.Message);
                    return BadRequest(new { Message = result.Message });
                }
                // Bước 4: Ghi log đăng xuất thành công
                _logger.LogInformation("Logout successful for user: {UserName}", User.Identity?.Name ?? "unknown");
                // Bước 5: Trả về phản hồi thành công với Success = true
                return Ok(new { Success = true, Message = result.Message });
            }
            catch (Exception ex)
            {
                // Bước 6: Ghi log và trả về lỗi nếu có
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
                // Bước 1: Kiểm tra dữ liệu đầu vào
                if (model == null)
                {
                    _logger.LogWarning("Register attempt with null model.");
                    return BadRequest(new { Message = "Invalid registration data." });
                }
                // Bước 2: Ghi log yêu cầu đăng ký
                _logger.LogInformation("Register attempt for email: {Email}", model.Email);
                // Bước 3: Gọi service để xử lý đăng ký
                var result = await _authService.RegisterAsync(model);
                // Bước 4: Kiểm tra kết quả đăng ký
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Register failed for {Email}: {Message}", model.Email, result.Message);
                    return BadRequest(new { Message = result.Message });
                }
                // Bước 5: Ghi log đăng ký thành công
                _logger.LogInformation("Register successful for {Email}", model.Email);
                // Bước 6: Trả về phản hồi thành công với Success = true
                return Ok(new { Success = true, Message = result.Message });
            }
            catch (Exception ex)
            {
                // Bước 7: Ghi log và trả về lỗi nếu có
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
                // Bước 1: Kiểm tra dữ liệu đầu vào
                if (model == null)
                {
                    _logger.LogWarning("Verify OTP attempt with null model.");
                    return BadRequest(new { Message = "Invalid OTP data." });
                }
                // Bước 2: Ghi log yêu cầu xác nhận OTP
                _logger.LogInformation("Verify OTP attempt for email: {Email}", model.Email);
                // Bước 3: Gọi service để xử lý xác nhận OTP
                var result = await _authService.VerifyOtpAsync(model);
                // Bước 4: Kiểm tra kết quả xác nhận OTP
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Verify OTP failed for {Email}: {Message}", model.Email, result.Message);
                    return BadRequest(new { Message = result.Message });
                }
                // Bước 5: Ghi log xác nhận OTP thành công
                _logger.LogInformation("Verify OTP successful for {Email}", model.Email);
                // Bước 6: Trả về phản hồi thành công với Success = true
                return Ok(new { Success = true, Message = result.Message });
            }
            catch (Exception ex)
            {
                // Bước 7: Ghi log và trả về lỗi nếu có
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
                // Bước 1: Kiểm tra dữ liệu đầu vào
                if (model == null || string.IsNullOrWhiteSpace(model.Email))
                {
                    _logger.LogWarning("Resend OTP attempt with invalid email.");
                    return BadRequest(new { Message = "Email is required." });
                }
                // Bước 2: Ghi log yêu cầu gửi lại OTP
                _logger.LogInformation("Resend OTP attempt for email: {Email}", model.Email);
                // Bước 3: Gọi service để xử lý gửi lại OTP
                var result = await _authService.ResendOtpAsync(model.Email);
                // Bước 4: Kiểm tra kết quả gửi lại OTP
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Resend OTP failed for {Email}: {Message}", model.Email, result.Message);
                    return BadRequest(new { Message = result.Message });
                }
                // Bước 5: Ghi log gửi lại OTP thành công
                _logger.LogInformation("Resend OTP successful for {Email}", model.Email);
                // Bước 6: Trả về phản hồi thành công với Success = true
                return Ok(new { Success = true, Message = result.Message });
            }
            catch (Exception ex)
            {
                // Bước 7: Ghi log và trả về lỗi nếu có
                _logger.LogError(ex, "Error during OTP resend for {Email}.", model?.Email ?? "unknown");
                return StatusCode(500, new { Message = "An error occurred during OTP resend.", Error = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves all users (admin only).
        /// </summary>
        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                // Bước 1: Ghi log yêu cầu lấy danh sách người dùng
                _logger.LogInformation("Request to retrieve all users by admin: {AdminName}", User.Identity?.Name);
                // Bước 2: Gọi service để lấy danh sách người dùng
                var result = await _authService.GetAllUsersAsync();
                // Bước 3: Kiểm tra kết quả
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Failed to retrieve users: {Message}", result.Message);
                    return BadRequest(new { Message = result.Message });
                }
                // Bước 4: Lấy dữ liệu từ kết quả
                var successResult = (SuccessResponseResult)result;
                // Bước 5: Ghi log lấy danh sách thành công
                _logger.LogInformation("Successfully retrieved all users.");
                // Bước 6: Trả về phản hồi thành công
                return Ok(new { Message = successResult.Message, Users = successResult.Data });
            }
            catch (Exception ex)
            {
                // Bước 7: Ghi log và trả về lỗi nếu có
                _logger.LogError(ex, "Error while retrieving all users.");
                return StatusCode(500, new { Message = "An error occurred while retrieving all users.", Error = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves a user by ID (admin only).
        /// </summary>
        [HttpGet("users/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                // Bước 1: Ghi log yêu cầu lấy thông tin người dùng
                _logger.LogInformation("Request to retrieve user {UserId} by admin: {AdminName}", id, User.Identity?.Name);
                // Bước 2: Gọi service để lấy thông tin người dùng
                var result = await _authService.GetUserByIdAsync(id);
                // Bước 3: Kiểm tra kết quả
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Failed to retrieve user {UserId}: {Message}", id, result.Message);
                    return BadRequest(new { Message = result.Message });
                }
                // Bước 4: Lấy dữ liệu từ kết quả
                var successResult = (SuccessResponseResult)result;
                // Bước 5: Ghi log lấy thông tin thành công
                _logger.LogInformation("Successfully retrieved user {UserId}.", id);
                // Bước 6: Trả về phản hồi thành công
                return Ok(new { Message = successResult.Message, User = successResult.Data });
            }
            catch (Exception ex)
            {
                // Bước 7: Ghi log và trả về lỗi nếu có
                _logger.LogError(ex, "Error while retrieving user {UserId}.", id);
                return StatusCode(500, new { Message = "An error occurred while retrieving user.", Error = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a user (admin only).
        /// </summary>
        [HttpDelete("users/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                // Bước 1: Ghi log yêu cầu xóa người dùng
                _logger.LogInformation("Request to delete user {UserId} by admin: {AdminName}", id, User.Identity?.Name);
                // Bước 2: Gọi service để xóa người dùng
                var result = await _authService.DeleteUserAsync(id);
                // Bước 3: Kiểm tra kết quả
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Failed to delete user {UserId}: {Message}", id, result.Message);
                    return BadRequest(new { Message = result.Message });
                }
                // Bước 4: Ghi log xóa thành công
                _logger.LogInformation("Successfully deleted user {UserId}.", id);
                // Bước 5: Trả về phản hồi thành công với Success = true
                return Ok(new { Success = true, Message = result.Message });
            }
            catch (Exception ex)
            {
                // Bước 6: Ghi log và trả về lỗi nếu có
                _logger.LogError(ex, "Error while deleting user {UserId}.", id);
                return StatusCode(500, new { Message = "An error occurred while deleting user.", Error = ex.Message });
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
                // Bước 1: Kiểm tra dữ liệu đầu vào
                if (model == null || string.IsNullOrWhiteSpace(model.Email))
                {
                    _logger.LogWarning("Forgot password attempt with invalid email.");
                    return BadRequest(new { Message = "Email is required." });
                }
                // Bước 2: Ghi log yêu cầu quên mật khẩu
                _logger.LogInformation("Forgot password attempt for email: {Email}", model.Email);
                // Bước 3: Gọi service để xử lý quên mật khẩu
                var result = await _authService.ForgotPasswordAsync(model);
                // Bước 4: Kiểm tra kết quả
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Forgot password failed for {Email}: {Message}", model.Email, result.Message);
                    return BadRequest(new { Message = result.Message });
                }
                // Bước 5: Ghi log yêu cầu quên mật khẩu thành công
                _logger.LogInformation("Forgot password successful for {Email}", model.Email);
                // Bước 6: Trả về phản hồi thành công với Success = true
                return Ok(new { Success = true, Message = result.Message });
            }
            catch (Exception ex)
            {
                // Bước 7: Ghi log và trả về lỗi nếu có
                _logger.LogError(ex, "Error during forgot password for {Email}.", model?.Email ?? "unknown");
                return StatusCode(500, new { Message = "An error occurred during forgot password request.", Error = ex.Message });
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
                // Bước 1: Kiểm tra dữ liệu đầu vào
                if (model == null)
                {
                    _logger.LogWarning("Reset password attempt with null model.");
                    return BadRequest(new { Message = "Invalid password reset data." });
                }
                // Bước 2: Ghi log yêu cầu đặt lại mật khẩu
                _logger.LogInformation("Reset password attempt for email: {Email}", model.Email);
                // Bước 3: Gọi service để xử lý đặt lại mật khẩu
                var result = await _authService.ResetPasswordAsync(model);
                // Bước 4: Kiểm tra kết quả
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Reset password failed for {Email}: {Message}", model.Email, result.Message);
                    return BadRequest(new { Message = result.Message });
                }
                // Bước 5: Ghi log đặt lại mật khẩu thành công
                _logger.LogInformation("Reset password successful for {Email}", model.Email);
                // Bước 6: Trả về phản hồi thành công với Success = true
                return Ok(new { Success = true, Message = result.Message });
            }
            catch (Exception ex)
            {
                // Bước 7: Ghi log và trả về lỗi nếu có
                _logger.LogError(ex, "Error during reset password for {Email}.", model?.Email ?? "unknown");
                return StatusCode(500, new { Message = "An error occurred during password reset.", Error = ex.Message });
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
                // Bước 1: Kiểm tra dữ liệu đầu vào
                if (model == null)
                {
                    _logger.LogWarning("Add user attempt with null model.");
                    return BadRequest(new { Message = "Invalid user data." });
                }
                // Bước 2: Ghi log yêu cầu thêm người dùng
                _logger.LogInformation("Add user attempt for email: {Email} by admin: {AdminName}", model.Email, User.Identity?.Name);
                // Bước 3: Gọi service để thêm người dùng
                var result = await _authService.AddUserAsync(model);
                // Bước 4: Kiểm tra kết quả
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Add user failed for {Email}: {Message}", model.Email, result.Message);
                    return BadRequest(new { Message = result.Message });
                }
                // Bước 5: Lấy dữ liệu từ kết quả
                var successResult = (SuccessResponseResult)result;
                // Bước 6: Ghi log thêm người dùng thành công
                _logger.LogInformation("Add user successful for {Email}", model.Email);
                // Bước 7: Trả về phản hồi thành công
                return Ok(new { Message = successResult.Message, User = successResult.Data });
            }
            catch (Exception ex)
            {
                // Bước 8: Ghi log và trả về lỗi nếu có
                _logger.LogError(ex, "Error during add user for {Email}.", model?.Email ?? "unknown");
                return StatusCode(500, new { Message = "An error occurred while adding user.", Error = ex.Message });
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
                // Bước 1: Kiểm tra dữ liệu đầu vào
                if (model == null)
                {
                    _logger.LogWarning("Update user attempt with null model.");
                    return BadRequest(new { Message = "Invalid user data." });
                }
                // Bước 2: Ghi log yêu cầu cập nhật người dùng
                _logger.LogInformation("Update user attempt for ID: {UserId} by admin: {AdminName}", id, User.Identity?.Name);
                // Bước 3: Gọi service để cập nhật người dùng
                var result = await _authService.UpdateUserAsync(id, model);
                // Bước 4: Kiểm tra kết quả
                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Update user failed for ID {UserId}: {Message}", id, result.Message);
                    return BadRequest(new { Message = result.Message });
                }
                // Bước 5: Lấy dữ liệu từ kết quả
                var successResult = (SuccessResponseResult)result;
                // Bước 6: Ghi log cập nhật người dùng thành công
                _logger.LogInformation("Update user successful for ID {UserId}", id);
                // Bước 7: Trả về phản hồi thành công
                return Ok(new { Message = successResult.Message, User = successResult.Data });
            }
            catch (Exception ex)
            {
                // Bước 8: Ghi log và trả về lỗi nếu có
                _logger.LogError(ex, "Error during update user for ID {UserId}.", id);
                return StatusCode(500, new { Message = "An error occurred while updating user.", Error = ex.Message });
            }
        }
    }
}