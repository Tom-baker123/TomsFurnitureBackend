using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OA.Domain.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TomsFurnitureBackend.Extensions;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.Interfaces;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services
{
    public class AuthService : IAuthService
    {
        private readonly TomfurnitureContext _context;
        private readonly ILogger<AuthService> _logger;
        private readonly IEmailService _emailService;

        public AuthService(TomfurnitureContext context, ILogger<AuthService> logger, IEmailService emailService)
        {
            // Khởi tạo các dependency: database context, logger, và email service
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        // Hàm validate thông tin đăng nhập
        private static string ValidateLogin(LoginVModel model)
        {
            // Kiểm tra email có rỗng hoặc null không
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                return "Email is required.";
            }
            // Kiểm tra định dạng email bằng regex
            const string emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(model.Email.Trim(), emailRegex))
            {
                return "Invalid email format.";
            }
            // Kiểm tra mật khẩu có rỗng hoặc null không
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                return "Password is required.";
            }
            return string.Empty;
        }

        // Hàm validate thông tin đăng ký
        private static string ValidateRegister(RegisterVModel model)
        {
            // Kiểm tra tên người dùng có rỗng hoặc null không
            if (string.IsNullOrWhiteSpace(model.UserName))
            {
                return "Username is required.";
            }
            // Kiểm tra email có rỗng hoặc null không
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                return "Email is required.";
            }
            // Kiểm tra định dạng email bằng regex
            const string emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(model.Email.Trim(), emailRegex))
            {
                return "Invalid email format.";
            }
            // Kiểm tra mật khẩu có rỗng hoặc null không
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                return "Password is required.";
            }
            // Kiểm tra độ dài mật khẩu
            if (model.Password.Length < 6)
            {
                return "Password must be at least 6 characters long.";
            }
            return string.Empty;
        }

        // Hàm validate email
        private static string ValidateEmail(string email)
        {
            // Kiểm tra email có rỗng hoặc null không
            if (string.IsNullOrWhiteSpace(email))
            {
                return "Email is required.";
            }
            // Kiểm tra định dạng email bằng regex
            const string emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email.Trim(), emailRegex))
            {
                return "Invalid email format.";
            }
            return string.Empty;
        }

        // Hàm validate OTP
        private static string ValidateOtp(ConfirmOtpVModel model)
        {
            // Kiểm tra email có rỗng hoặc null không
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                return "Email is required.";
            }
            // Kiểm tra định dạng email bằng regex
            const string emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(model.Email.Trim(), emailRegex))
            {
                return "Invalid email format.";
            }
            // Kiểm tra OTP có rỗng hoặc null không
            if (string.IsNullOrWhiteSpace(model.Otp))
            {
                return "OTP is required.";
            }
            return string.Empty;
        }

        // Hàm validate yêu cầu đặt lại mật khẩu
        private static string ValidateResetPassword(ResetPasswordVModel model)
        {
            // Kiểm tra email có rỗng hoặc null không
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                return "Email is required.";
            }
            // Kiểm tra định dạng email bằng regex
            const string emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(model.Email.Trim(), emailRegex))
            {
                return "Invalid email format.";
            }
            // Kiểm tra mật khẩu mới có rỗng hoặc null không
            if (string.IsNullOrWhiteSpace(model.NewPassword))
            {
                return "New password is required.";
            }
            // Kiểm tra độ dài mật khẩu mới
            if (model.NewPassword.Length < 6)
            {
                return "New password must be at least 6 characters long.";
            }
            return string.Empty;
        }

        // Hàm validate thông tin người dùng
        private static string ValidateUser(string userName, string email, int roleId, bool isAdd = false, string? password = null)
        {
            // Kiểm tra tên người dùng có rỗng hoặc null không
            if (string.IsNullOrWhiteSpace(userName))
            {
                return "Username is required.";
            }
            // Kiểm tra email có rỗng hoặc null không
            if (string.IsNullOrWhiteSpace(email))
            {
                return "Email is required.";
            }
            // Kiểm tra định dạng email bằng regex
            const string emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email.Trim(), emailRegex))
            {
                return "Invalid email format.";
            }
            // Kiểm tra mật khẩu khi thêm người dùng mới
            if (isAdd)
            {
                if (string.IsNullOrWhiteSpace(password))
                {
                    return "Password is required.";
                }
                if (password.Length < 6)
                {
                    return "Password must be at least 6 characters long.";
                }
            }
            // Kiểm tra ID vai trò hợp lệ
            if (roleId <= 0)
            {
                return "Role is required.";
            }
            return string.Empty;
        }

        // Đăng nhập người dùng
        public async Task<ResponseResult> LoginAsync(LoginVModel model, HttpContext httpContext)
        {
            try
            {
                // Bước 1: Validate thông tin đăng nhập
                var validationResult = ValidateLogin(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    _logger.LogWarning("Login validation failed for {Email}: {Error}", model.Email, validationResult);
                    return new ErrorResponseResult(validationResult);
                }
                // Bước 2: Tìm người dùng trong database
                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == model.Email.ToLower());
                if (user == null || !user.IsActive.GetValueOrDefault())
                {
                    _logger.LogWarning("Invalid email or account not activated for {Email}", model.Email);
                    return new ErrorResponseResult("Invalid email or account not activated.");
                }
                // Bước 3: Kiểm tra mật khẩu
                if (!BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Invalid password for {Email}", model.Email);
                    return new ErrorResponseResult("Invalid password.");
                }
                // Bước 4: Tạo claims cho xác thực
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.RoleName),
                    new Claim("Email", user.Email)
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                };
                // Bước 5: Đăng nhập người dùng bằng cookie
                await httpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);
                _logger.LogInformation("Login successful for {Email}", model.Email);
                return new SuccessResponseResult(
                    new LoginResultVModel
                    {
                        UserName = user.UserName,
                        Role = user.Role.RoleName
                    },
                    "Login successful.");
            }
            catch (Exception ex)
            {
                // Bước 6: Ghi log và trả về lỗi nếu có
                _logger.LogError("Error during login for {Email}: {Error}", model.Email, ex.Message);
                return new ErrorResponseResult($"An error occurred during login: {ex.Message}");
            }
        }

        // Đăng xuất người dùng
        public async Task<ResponseResult> LogoutAsync(HttpContext httpContext)
        {
            try
            {
                // Bước 1: Đăng xuất người dùng bằng cách xóa cookie xác thực
                await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                _logger.LogInformation("Logout successful.");
                return new SuccessResponseResult(null, "Logout successful.");
            }
            catch (Exception ex)
            {
                // Bước 2: Ghi log và trả về lỗi nếu có
                _logger.LogError("Error during logout: {Error}", ex.Message);
                return new ErrorResponseResult($"An error occurred during logout: {ex.Message}");
            }
        }

        // Kiểm tra trạng thái xác thực của người dùng
        public async Task<AuthStatusVModel> GetAuthStatusAsync(ClaimsPrincipal user, HttpContext httpContext)
        {
            try
            {
                // Bước 1: Kiểm tra xem người dùng đã xác thực chưa
                if (user.Identity?.IsAuthenticated != true)
                {
                    return new AuthStatusVModel { IsAuthenticated = false, Message = "User is not authenticated." };
                }
                // Bước 2: Lấy ID người dùng từ claims
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return new AuthStatusVModel { IsAuthenticated = false, Message = "Invalid user session." };
                }
                // Bước 3: Tìm thông tin người dùng trong database
                var dbUser = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Id == int.Parse(userId));
                if (dbUser == null)
                {
                    return new AuthStatusVModel { IsAuthenticated = false, Message = "User not found." };
                }
                // Bước 4: Trả về trạng thái xác thực
                return new AuthStatusVModel
                {
                    IsAuthenticated = true,
                    UserName = dbUser.UserName,
                    Email = dbUser.Email,
                    Role = dbUser.Role.RoleName
                };
            }
            catch (Exception ex)
            {
                // Bước 5: Ghi log và trả về lỗi nếu có
                _logger.LogError("Error while checking authentication status: {Error}", ex.Message);
                return new AuthStatusVModel
                {
                    IsAuthenticated = false,
                    Message = $"An error occurred while checking authentication status: {ex.Message}"
                };
            }
        }

        // Đăng ký người dùng mới
        public async Task<ResponseResult> RegisterAsync(RegisterVModel model)
        {
            try
            {
                // Bước 1: Ghi log bắt đầu quá trình đăng ký
                _logger.LogInformation("Starting registration for email: {Email}", model.Email);
                // Bước 2: Validate thông tin đăng ký
                var validationResult = ValidateRegister(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    _logger.LogWarning("Registration validation failed for {Email}: {Error}", model.Email, validationResult);
                    return new ErrorResponseResult(validationResult);
                }
                // Bước 3: Chuẩn hóa email
                var normalizedEmail = model.Email.Trim().ToLower();
                _logger.LogInformation("Normalized email: {OriginalEmail} -> {NormalizedEmail}", model.Email, normalizedEmail);
                // Bước 4: Kiểm tra email đã tồn tại chưa
                if (await _context.Users.AnyAsync(u => u.Email.ToLower() == normalizedEmail))
                {
                    _logger.LogWarning("Email already registered: {Email}", normalizedEmail);
                    return new ErrorResponseResult("Email is already registered.");
                }
                // Bước 5: Tìm vai trò 'User'
                _logger.LogInformation("Finding 'User' role");
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "User");
                if (role == null)
                {
                    _logger.LogError("User role not found in the database.");
                    return new ErrorResponseResult("User role not found.");
                }
                // Bước 6: Tạo entity người dùng mới
                _logger.LogInformation("Creating new user for {Email}", normalizedEmail);
                var user = model.ToUserEntity(role.Id);
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("User created successfully: {Email}, UserId: {UserId}", normalizedEmail, user.Id);
                // Bước 7: Tạo OTP và lưu vào database
                var otp = GenerateOtp();
                var otpEntity = AuthExtensions.ToConfirmOtpEntity(user.Id, otp);
                _context.ConfirmOtps.Add(otpEntity);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Created OTP for {Email}: {Otp}, expires at {Expiry}", normalizedEmail, otp, otpEntity.ExpiredDate);
                // Bước 8: Gửi email OTP
                try
                {
                    var emailBody = $"<p>Your OTP code is: <strong>{otp}</strong>. This code is valid for 30 minutes.</p>";
                    await _emailService.SendEmailAsync(model.Email, "Your OTP Code", emailBody);
                    _logger.LogInformation("Successfully sent OTP email to {Email}", normalizedEmail);
                }
                catch (Exception ex)
                {
                    // Bước 9: Xử lý lỗi gửi email, xóa user và OTP
                    _logger.LogError("Failed to send OTP email to {Email}: {Error}", normalizedEmail, ex.Message);
                    _context.Users.Remove(user);
                    _context.ConfirmOtps.Remove(otpEntity);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Deleted user {Email} and OTP due to email sending failure", normalizedEmail);
                    return new ErrorResponseResult("Failed to send OTP email. Please try again.");
                }
                // Bước 10: Trả về kết quả thành công
                return new SuccessResponseResult(null, "Registration successful. Please check your email for the OTP code.");
            }
            catch (Exception ex)
            {
                // Bước 11: Ghi log và trả về lỗi nếu có
                _logger.LogError("Error during registration for {Email}: {Error}", model.Email, ex.Message);
                return new ErrorResponseResult($"An error occurred during registration: {ex.Message}");
            }
        }

        // Xác nhận OTP để kích hoạt tài khoản
        public async Task<ResponseResult> VerifyOtpAsync(ConfirmOtpVModel model)
        {
            try
            {
                // Bước 1: Ghi log bắt đầu xác nhận OTP
                _logger.LogInformation("Starting OTP verification for email: {Email}", model.Email);
                // Bước 2: Validate thông tin OTP
                var validationResult = ValidateOtp(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    _logger.LogWarning("OTP verification failed for {Email}: {Error}", model.Email, validationResult);
                    return new ErrorResponseResult(validationResult);
                }
                // Bước 3: Chuẩn hóa email
                var normalizedEmail = model.Email.Trim().ToLower();
                _logger.LogInformation("Normalized email: {OriginalEmail} -> {NormalizedEmail}", model.Email, normalizedEmail);
                // Bước 4: Tìm người dùng và OTP liên quan
                var user = await _context.Users
                    .Include(u => u.ConfirmOtp)
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);
                if (user == null)
                {
                    _logger.LogWarning("User not found for email: {Email}", normalizedEmail);
                    return new ErrorResponseResult("User not found.");
                }
                // Bước 5: Kiểm tra tài khoản đã kích hoạt chưa
                if (user.IsActive == true)
                {
                    _logger.LogWarning("Account already activated for {Email}", normalizedEmail);
                    return new ErrorResponseResult("Account is already activated.");
                }
                // Bước 6: Kiểm tra OTP có tồn tại và còn hiệu lực không
                var otpData = user.ConfirmOtp;
                if (otpData == null || otpData.ExpiredDate < DateTime.UtcNow)
                {
                    _logger.LogWarning("No valid OTP found for email: {Email}", normalizedEmail);
                    return new ErrorResponseResult("OTP does not exist or has expired. Please request a new OTP.");
                }
                // Bước 7: Kiểm tra số lần nhập sai OTP
                if (otpData.FailedAttempt >= 3)
                {
                    _logger.LogWarning("Exceeded 3 failed OTP attempts for email: {Email}", normalizedEmail);
                    return new ErrorResponseResult("You have entered the wrong OTP more than 3 times. Please request a new OTP.");
                }
                // Bước 8: Kiểm tra OTP có khớp không
                if (otpData.Otpcode != model.Otp.Trim())
                {
                    _logger.LogWarning("Invalid OTP for email: {Email}, Provided OTP: {ProvidedOtp}, Expected OTP: {ExpectedOtp}",
                        normalizedEmail, model.Otp, otpData.Otpcode);
                    otpData.FailedAttempt++;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Recorded failed OTP attempt for {Email}, Current failed attempts: {FailedAttempts}", normalizedEmail, otpData.FailedAttempt);
                    return new ErrorResponseResult("Invalid OTP.");
                }
                // Bước 9: Kích hoạt tài khoản và xóa OTP
                _logger.LogInformation("Activating user for {Email}", normalizedEmail);
                user.IsActive = true;
                user.UpdatedDate = DateTime.UtcNow;
                user.UpdatedBy = "System";
                _context.ConfirmOtps.Remove(otpData);
                await _context.SaveChangesAsync();
                _logger.LogInformation("User activated successfully: {Email}", normalizedEmail);
                // Bước 10: Trả về kết quả thành công
                return new SuccessResponseResult(null, "Account activated successfully.");
            }
            catch (Exception ex)
            {
                // Bước 11: Ghi log và trả về lỗi nếu có
                _logger.LogError("Error during OTP verification for {Email}: {Error}", model.Email, ex.Message);
                return new ErrorResponseResult($"An error occurred during OTP verification: {ex.Message}");
            }
        }

        // Gửi lại OTP
        public async Task<ResponseResult> ResendOtpAsync(string email)
        {
            try
            {
                // Bước 1: Ghi log yêu cầu gửi lại OTP
                _logger.LogInformation("Request to resend OTP for email: {Email}", email);
                // Bước 2: Validate email
                var validationResult = ValidateEmail(email);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    _logger.LogWarning("Resend OTP failed for {Email}: {Error}", email, validationResult);
                    return new ErrorResponseResult(validationResult);
                }
                // Bước 3: Chuẩn hóa email
                var normalizedEmail = email.Trim().ToLower();
                _logger.LogInformation("Normalized email: {OriginalEmail} -> {NormalizedEmail}", email, normalizedEmail);
                // Bước 4: Tìm người dùng và OTP liên quan
                var user = await _context.Users
                    .Include(u => u.ConfirmOtp)
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);
                if (user == null)
                {
                    _logger.LogWarning("User not found for email: {Email}", normalizedEmail);
                    return new ErrorResponseResult("User not found.");
                }
                // Bước 5: Kiểm tra tài khoản đã kích hoạt chưa
                if (user.IsActive == true)
                {
                    _logger.LogWarning("Account already activated for {Email}", normalizedEmail);
                    return new ErrorResponseResult("Account is already activated.");
                }
                // Bước 6: Kiểm tra thời gian gửi lại OTP
                var latestOtp = user.ConfirmOtp;
                if (latestOtp != null && latestOtp.CreatedDate.HasValue && latestOtp.CreatedDate.Value.AddMinutes(1) > DateTime.UtcNow)
                {
                    _logger.LogWarning("Resend OTP request too soon for {Email}. Please wait.", normalizedEmail);
                    return new ErrorResponseResult("You have recently requested an OTP. Please wait 1 minute before requesting a new OTP.");
                }
                // Bước 7: Xóa OTP cũ nếu có
                if (latestOtp != null)
                {
                    _context.ConfirmOtps.Remove(latestOtp);
                    _logger.LogInformation("Removed old OTP for {Email}", normalizedEmail);
                }
                // Bước 8: Tạo và lưu OTP mới
                var otp = GenerateOtp();
                var otpEntity = AuthExtensions.ToConfirmOtpEntity(user.Id, otp);
                _context.ConfirmOtps.Add(otpEntity);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Created new OTP for {Email}: {Otp}, expires at {Expiry}", normalizedEmail, otp, otpEntity.ExpiredDate);
                // Bước 9: Gửi email OTP
                try
                {
                    var emailBody = $"<p>Your new OTP code is: <strong>{otp}</strong>. This code is valid for 30 minutes.</p>";
                    await _emailService.SendEmailAsync(email, "Your New OTP Code", emailBody);
                    _logger.LogInformation("Successfully sent new OTP email to {Email}", normalizedEmail);
                }
                catch (Exception ex)
                {
                    // Bước 10: Xử lý lỗi gửi email, xóa OTP
                    _logger.LogError("Failed to send new OTP email to {Email}: {Error}", normalizedEmail, ex.Message);
                    _context.ConfirmOtps.Remove(otpEntity);
                    await _context.SaveChangesAsync();
                    return new ErrorResponseResult("Failed to send new OTP email. Please try again.");
                }
                // Bước 11: Trả về kết quả thành công
                return new SuccessResponseResult(null, "New OTP code sent successfully. Please check your email.");
            }
            catch (Exception ex)
            {
                // Bước 12: Ghi log và trả về lỗi nếu có
                _logger.LogError("Error during resend OTP for {Email}: {Error}", email, ex.Message);
                return new ErrorResponseResult($"An error occurred during resend OTP: {ex.Message}");
            }
        }

        // Tạo mã OTP ngẫu nhiên
        private string GenerateOtp()
        {
            // Bước 1: Tạo mã OTP 6 chữ số ngẫu nhiên
            var random = new Random();
            var otp = random.Next(100000, 999999).ToString();
            _logger.LogInformation("Generated OTP: {Otp}", otp);
            return otp;
        }

        // Lấy danh sách tất cả người dùng
        public async Task<ResponseResult> GetAllUsersAsync()
        {
            try
            {
                // Bước 1: Lấy danh sách người dùng từ database
                var users = await _context.Users
                    .Include(u => u.Role)
                    .ToListAsync();
                // Bước 2: Ánh xạ sang UserVModel
                var userVModels = users.Select(u => u.ToUserVModel()).ToList();
                // Bước 3: Trả về kết quả thành công
                return new SuccessResponseResult(userVModels, "Successfully retrieved all users.");
            }
            catch (Exception ex)
            {
                // Bước 4: Ghi log và trả về lỗi nếu có
                return new ErrorResponseResult($"An error occurred while retrieving all users: {ex.Message}");
            }
        }

        // Lấy thông tin người dùng theo ID
        public async Task<ResponseResult> GetUserByIdAsync(int id)
        {
            try
            {
                // Bước 1: Tìm người dùng theo ID
                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    return new ErrorResponseResult("User not found.");
                }
                // Bước 2: Ánh xạ sang UserVModel
                var userVModel = user.ToUserVModel();
                // Bước 3: Trả về kết quả thành công
                return new SuccessResponseResult(userVModel, "Successfully retrieved user.");
            }
            catch (Exception ex)
            {
                // Bước 4: Ghi log và trả về lỗi nếu có
                return new ErrorResponseResult($"An error occurred while retrieving user: {ex.Message}");
            }
        }

        // Xóa người dùng
        public async Task<ResponseResult> DeleteUserAsync(int id)
        {
            try
            {
                // Bước 1: Ghi log bắt đầu xóa người dùng
                _logger.LogInformation("Starting deletion of user with ID: {UserId}", id);
                // Bước 2: Tìm người dùng theo ID
                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    _logger.LogWarning("User not found with ID: {UserId}", id);
                    return new ErrorResponseResult("User not found.");
                }
                // Bước 3: Kiểm tra xem người dùng có phải admin không
                if (user.Role.RoleName == "Admin")
                {
                    _logger.LogWarning("Cannot delete admin account with ID: {UserId}", id);
                    return new ErrorResponseResult("Cannot delete admin account.");
                }
                // Bước 4: Bắt đầu transaction để đảm bảo tính toàn vẹn dữ liệu
                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Bước 5: Xóa bản ghi ConfirmOtp nếu có
                    var confirmOtp = await _context.ConfirmOtps
                        .Where(o => o.UserId == id)
                        .FirstOrDefaultAsync();
                    if (confirmOtp != null)
                    {
                        _context.ConfirmOtps.Remove(confirmOtp);
                        _logger.LogInformation("Removed ConfirmOtp for user {UserId}.", id);
                    }
                    // Bước 6: Xóa bản ghi Carts
                    var carts = await _context.Carts
                        .Where(c => c.UserId == id)
                        .ToListAsync();
                    if (carts.Any())
                    {
                        _context.Carts.RemoveRange(carts);
                        _logger.LogInformation("Removed {Count} Cart records for user {UserId}.", carts.Count, id);
                    }
                    // Bước 7: Xóa bản ghi Comments
                    var comments = await _context.Comments
                        .Where(c => c.UserId == id)
                        .ToListAsync();
                    if (comments.Any())
                    {
                        _context.Comments.RemoveRange(comments);
                        _logger.LogInformation("Removed {Count} Comment records for user {UserId}.", comments.Count, id);
                    }
                    // Bước 8: Xóa bản ghi Feedbacks
                    var feedbacks = await _context.Feedbacks
                        .Where(f => f.UserId == id)
                        .ToListAsync();
                    if (feedbacks.Any())
                    {
                        _context.Feedbacks.RemoveRange(feedbacks);
                        _logger.LogInformation("Removed {Count} Feedback records for user {UserId}.", feedbacks.Count, id);
                    }
                    // Bước 9: Xóa bản ghi ProductReviews
                    var productReviews = await _context.ProductReviews
                        .Where(pr => pr.UserId == id)
                        .ToListAsync();
                    if (productReviews.Any())
                    {
                        _context.ProductReviews.RemoveRange(productReviews);
                        _logger.LogInformation("Removed {Count} ProductReview records for user {UserId}.", productReviews.Count, id);
                    }
                    // Bước 10: Vô hiệu hóa Orders
                    var orders = await _context.Orders
                        .Where(o => o.UserId == id)
                        .ToListAsync();
                    foreach (var order in orders)
                    {
                        order.IsActive = false;
                        _logger.LogInformation("Deactivated order {OrderId} for user {UserId}.", order.Id, id);
                    }
                    // Bước 11: Vô hiệu hóa OrderAddresses
                    var orderAddresses = await _context.OrderAddresses
                        .Where(oa => oa.UserId == id)
                        .ToListAsync();
                    foreach (var address in orderAddresses)
                    {
                        address.IsActive = false;
                        _logger.LogInformation("Deactivated address {AddressId} for user {UserId}.", address.Id, id);
                    }
                    // Bước 12: Vô hiệu hóa Shippings
                    var shippings = await _context.Shippings
                        .Where(s => s.UserId == id)
                        .ToListAsync();
                    foreach (var shipping in shippings)
                    {
                        shipping.IsActive = false;
                        _logger.LogInformation("Deactivated shipping {ShippingId} for user {UserId}.", shipping.Id, id);
                    }
                    // Bước 13: Kiểm tra banner liên quan
                    var banners = await _context.Banners
                        .Where(b => b.UserId == id)
                        .ToListAsync();
                    if (banners.Any())
                    {
                        _logger.LogWarning("User {UserId} has {Count} banners. Cannot delete.", id, banners.Count);
                        await transaction.RollbackAsync();
                        return new ErrorResponseResult("Cannot delete user due to associated banners.");
                    }
                    // Bước 14: Kiểm tra tin tức liên quan
                    var news = await _context.News
                        .Where(n => n.UserId == id)
                        .ToListAsync();
                    if (news.Any())
                    {
                        _logger.LogWarning("User {UserId} has {Count} news items. Cannot delete.", id, news.Count);
                        await transaction.RollbackAsync();
                        return new ErrorResponseResult("Cannot delete user due to associated news.");
                    }
                    // Bước 15: Xóa người dùng và commit transaction
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    _logger.LogInformation("Successfully deleted user {UserId}.", id);
                    return new SuccessResponseResult(null, "User deleted successfully.");
                }
                catch (Exception ex)
                {
                    // Bước 16: Rollback transaction và trả về lỗi nếu có
                    await transaction.RollbackAsync();
                    _logger.LogError("Error while deleting related records for user {UserId}: {Error}", id, ex.Message);
                    return new ErrorResponseResult($"An error occurred while deleting related records: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                // Bước 17: Ghi log và trả về lỗi nếu có
                _logger.LogError("Error while deleting user {UserId}: {Error}", id, ex.Message);
                return new ErrorResponseResult($"An error occurred while deleting user: {ex.Message}");
            }
        }

        // Yêu cầu quên mật khẩu
        public async Task<ResponseResult> ForgotPasswordAsync(ForgotPasswordVModel model)
        {
            try
            {
                // Bước 1: Ghi log yêu cầu quên mật khẩu
                _logger.LogInformation("Forgot password request for email: {Email}", model.Email);
                // Bước 2: Validate email
                var validationResult = ValidateEmail(model.Email);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    _logger.LogWarning("Validation failed for {Email}: {Error}", model.Email, validationResult);
                    return new ErrorResponseResult(validationResult);
                }
                // Bước 3: Chuẩn hóa email
                var normalizedEmail = model.Email.Trim().ToLower();
                // Bước 4: Tìm người dùng và OTP liên quan
                var user = await _context.Users
                    .Include(u => u.ConfirmOtp)
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);
                if (user == null)
                {
                    _logger.LogWarning("User not found for email: {Email}", normalizedEmail);
                    return new ErrorResponseResult("User not found.");
                }
                // Bước 5: Vô hiệu hóa tài khoản
                user.IsActive = false;
                user.UpdatedDate = DateTime.UtcNow;
                user.UpdatedBy = "System";
                // Bước 6: Xóa OTP cũ nếu có
                if (user.ConfirmOtp != null)
                {
                    _context.ConfirmOtps.Remove(user.ConfirmOtp);
                    _logger.LogInformation("Removed old OTP for {Email}", normalizedEmail);
                }
                // Bước 7: Kiểm tra thời gian gửi lại OTP
                var latestOtp = user.ConfirmOtp;
                if (latestOtp != null && latestOtp.CreatedDate.HasValue && latestOtp.CreatedDate.Value.AddMinutes(1) > DateTime.UtcNow)
                {
                    _logger.LogWarning("OTP request too soon for {Email}. Please wait.", normalizedEmail);
                    return new ErrorResponseResult("Please wait 1 minute before requesting a new OTP.");
                }
                // Bước 8: Tạo và lưu OTP mới
                var otp = GenerateOtp();
                var otpEntity = AuthExtensions.ToConfirmOtpEntity(user.Id, otp);
                _context.ConfirmOtps.Add(otpEntity);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Created new OTP for {Email}: {Otp}, expires at {Expiry}", normalizedEmail, otp, otpEntity.ExpiredDate);
                // Bước 9: Gửi email OTP
                try
                {
                    var emailBody = $"<p>Your OTP code to reset your password is: <strong>{otp}</strong>. This code is valid for 30 minutes.</p>";
                    await _emailService.SendEmailAsync(model.Email, "OTP Code to Reset Password", emailBody);
                    _logger.LogInformation("Successfully sent OTP email to {Email}", normalizedEmail);
                }
                catch (Exception ex)
                {
                    // Bước 10: Xử lý lỗi gửi email, xóa OTP và khôi phục trạng thái
                    _logger.LogError("Failed to send OTP email to {Email}: {Error}", normalizedEmail, ex.Message);
                    _context.ConfirmOtps.Remove(otpEntity);
                    user.IsActive = true;
                    await _context.SaveChangesAsync();
                    return new ErrorResponseResult("Failed to send OTP email. Please try again.");
                }
                // Bước 11: Trả về kết quả thành công
                return new SuccessResponseResult(null, "OTP code sent successfully. Please check your email.");
            }
            catch (Exception ex)
            {
                // Bước 12: Ghi log và trả về lỗi nếu có
                _logger.LogError("Error during forgot password request for {Email}: {Error}", model.Email, ex.Message);
                return new ErrorResponseResult($"An error occurred during forgot password request: {ex.Message}");
            }
        }

        // Đặt lại mật khẩu
        public async Task<ResponseResult> ResetPasswordAsync(ResetPasswordVModel model)
        {
            try
            {
                // Bước 1: Ghi log yêu cầu đặt lại mật khẩu
                _logger.LogInformation("Password reset request for email: {Email}", model.Email);
                // Bước 2: Validate thông tin đặt lại mật khẩu
                var validationResult = ValidateResetPassword(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    _logger.LogWarning("Password reset validation failed for {Email}: {Error}", model.Email, validationResult);
                    return new ErrorResponseResult(validationResult);
                }
                // Bước 3: Chuẩn hóa email
                var normalizedEmail = model.Email.Trim().ToLower();
                _logger.LogInformation("Normalized email: {OriginalEmail} -> {NormalizedEmail}", model.Email, normalizedEmail);
                // Bước 4: Tìm người dùng
                var user = await _context.Users
                    .Include(u => u.ConfirmOtp)
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);
                if (user == null)
                {
                    _logger.LogWarning("User not found for email: {Email}", normalizedEmail);
                    return new ErrorResponseResult("User not found.");
                }
                // Bước 5: Cập nhật mật khẩu và kích hoạt tài khoản
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                user.IsActive = true;
                user.UpdatedDate = DateTime.UtcNow;
                user.UpdatedBy = "System";
                // Bước 6: Xóa OTP nếu có
                if (user.ConfirmOtp != null)
                {
                    _context.ConfirmOtps.Remove(user.ConfirmOtp);
                    _logger.LogInformation("Removed OTP for {Email}", normalizedEmail);
                }
                // Bước 7: Lưu thay đổi vào database
                await _context.SaveChangesAsync();
                _logger.LogInformation("Password reset successfully for {Email}", normalizedEmail);
                // Bước 8: Trả về kết quả thành công
                return new SuccessResponseResult(null, "Password reset successfully.");
            }
            catch (Exception ex)
            {
                // Bước 9: Ghi log và trả về lỗi nếu có
                _logger.LogError("Error during password reset for {Email}: {Error}", model.Email, ex.Message);
                return new ErrorResponseResult($"An error occurred during password reset: {ex.Message}");
            }
        }

        // Thêm người dùng mới
        public async Task<ResponseResult> AddUserAsync(AddUserVModel model)
        {
            try
            {
                // Bước 1: Ghi log bắt đầu thêm người dùng
                _logger.LogInformation("Starting to add new user with email: {Email}", model.Email);
                // Bước 2: Validate thông tin người dùng
                var validationResult = ValidateUser(model.UserName, model.Email, model.RoleId, isAdd: true, model.Password);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    _logger.LogWarning("Add user failed for {Email}: {Error}", model.Email, validationResult);
                    return new ErrorResponseResult(validationResult);
                }
                // Bước 3: Chuẩn hóa email
                var normalizedEmail = model.Email.Trim().ToLower();
                _logger.LogInformation("Normalized email: {OriginalEmail} -> {NormalizedEmail}", model.Email, normalizedEmail);
                // Bước 4: Kiểm tra email đã tồn tại chưa
                if (await _context.Users.AnyAsync(u => u.Email.ToLower() == normalizedEmail))
                {
                    _logger.LogWarning("Email already registered: {Email}", normalizedEmail);
                    return new ErrorResponseResult("Email is already registered.");
                }
                // Bước 5: Kiểm tra vai trò tồn tại
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == model.RoleId);
                if (role == null)
                {
                    _logger.LogWarning("Role not found with ID: {RoleId}", model.RoleId);
                    return new ErrorResponseResult("Role not found.");
                }
                // Bước 6: Tạo và lưu người dùng mới
                var user = model.ToUserEntity();
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully added user: {Email}, UserId: {UserId}", normalizedEmail, user.Id);
                // Bước 7: Trả về kết quả thành công
                return new SuccessResponseResult(user.ToUserVModel(), "User added successfully.");
            }
            catch (Exception ex)
            {
                // Bước 8: Ghi log và trả về lỗi nếu có
                _logger.LogError("Error while adding user {Email}: {Error}", model.Email, ex.Message);
                return new ErrorResponseResult($"An error occurred while adding user: {ex.Message}");
            }
        }

        // Cập nhật thông tin người dùng
        public async Task<ResponseResult> UpdateUserAsync(int id, UpdateUserVModel model)
        {
            try
            {
                // Bước 1: Ghi log bắt đầu cập nhật người dùng
                _logger.LogInformation("Starting to update user with ID: {UserId}", id);
                // Bước 2: Validate thông tin người dùng
                var validationResult = ValidateUser(model.UserName, model.Email, model.RoleId);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    _logger.LogWarning("Update user failed for ID {UserId}: {Error}", id, validationResult);
                    return new ErrorResponseResult(validationResult);
                }
                // Bước 3: Tìm người dùng theo ID
                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    _logger.LogWarning("User not found with ID: {UserId}", id);
                    return new ErrorResponseResult("User not found.");
                }
                // Bước 4: Chuẩn hóa email
                var normalizedEmail = model.Email.Trim().ToLower();
                _logger.LogInformation("Normalized email: {OriginalEmail} -> {NormalizedEmail}", model.Email, normalizedEmail);
                // Bước 5: Kiểm tra email đã được sử dụng bởi người dùng khác chưa
                if (await _context.Users.AnyAsync(u => u.Email.ToLower() == normalizedEmail && u.Id != id))
                {
                    _logger.LogWarning("Email already used by another user: {Email}", normalizedEmail);
                    return new ErrorResponseResult("Email is already used by another user.");
                }
                // Bước 6: Kiểm tra vai trò tồn tại
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == model.RoleId);
                if (role == null)
                {
                    _logger.LogWarning("Role not found with ID: {RoleId}", model.RoleId);
                    return new ErrorResponseResult("Role not found.");
                }
                // Bước 7: Cập nhật thông tin người dùng
                user.UpdateUserEntity(model);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully updated user: {UserId}", id);
                // Bước 8: Trả về kết quả thành công
                return new SuccessResponseResult(user.ToUserVModel(), "User updated successfully.");
            }
            catch (Exception ex)
            {
                // Bước 9: Ghi log và trả về lỗi nếu có
                _logger.LogError("Error while updating user {UserId}: {Error}", id, ex.Message);
                return new ErrorResponseResult($"An error occurred while updating user: {ex.Message}");
            }
        }
    }
}