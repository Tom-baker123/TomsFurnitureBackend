using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
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
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }
        // Xác thực thông tin đăng nhập của người dùng
        private static string ValidateLogin(LoginVModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                return "Email là bắt buộc.";
            }

            const string emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(model.Email.Trim(), emailRegex))
            {
                return "Định dạng email không hợp lệ.";
            }

            if (string.IsNullOrWhiteSpace(model.Password))
            {
                return "Mật khẩu là bắt buộc.";
            }

            return string.Empty;
        }
        // Xác thực thông tin đăng ký của người dùng
        private static string ValidateRegister(RegisterVModel model)
        {
            if (string.IsNullOrWhiteSpace(model.UserName))
            {
                return "Tên người dùng là bắt buộc.";
            }

            if (string.IsNullOrWhiteSpace(model.Email))
            {
                return "Email là bắt buộc.";
            }

            const string emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(model.Email.Trim(), emailRegex))
            {
                return "Định dạng email không hợp lệ.";
            }

            if (string.IsNullOrWhiteSpace(model.Password))
            {
                return "Mật khẩu là bắt buộc.";
            }

            if (model.Password.Length < 6)
            {
                return "Mật khẩu phải có ít nhất 6 ký tự.";
            }

            if (!string.IsNullOrWhiteSpace(model.Gender) &&
                model.Gender.Trim().ToLower() != "male" &&
                model.Gender.Trim().ToLower() != "female")
            {
                return "Giới tính phải là 'Male' hoặc 'Female'.";
            }

            return string.Empty;
        }
        // Xác thực email
        private static string ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return "Email là bắt buộc.";
            }

            const string emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email.Trim(), emailRegex))
            {
                return "Định dạng email không hợp lệ.";
            }

            return string.Empty;
        }
        // Xác thực thông tin OTP
        private static string ValidateOtp(ConfirmOtpVModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                return "Email là bắt buộc.";
            }

            const string emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(model.Email.Trim(), emailRegex))
            {
                return "Định dạng email không hợp lệ.";
            }

            if (string.IsNullOrWhiteSpace(model.Otp))
            {
                return "OTP là bắt buộc.";
            }

            return string.Empty;
        }
        // [1.] Đăng nhập người dùng
        public async Task<ResponseResult> LoginAsync(LoginVModel model, HttpContext httpContext)
        {
            try
            {
                var validationResult = ValidateLogin(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    return new ErrorResponseResult(validationResult);
                }

                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == model.Email.ToLower());

                if (user == null || !user.IsActive.GetValueOrDefault())
                {
                    return new ErrorResponseResult("Email không hợp lệ hoặc tài khoản chưa được kích hoạt.");
                }

                if (!BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    return new ErrorResponseResult("Mật khẩu không hợp lệ.");
                }

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

                await httpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return new SuccessResponseResult(
                    new LoginResultVModel
                    {
                        UserName = user.UserName,
                        Role = user.Role.RoleName
                    },
                    "Đăng nhập thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi trong quá trình đăng nhập: {Error}", ex.Message);
                return new ErrorResponseResult($"Đã xảy ra lỗi trong quá trình đăng nhập: {ex.Message}");
            }
        }
        // [2.] Đăng xuất người dùng
        public async Task<ResponseResult> LogoutAsync(HttpContext httpContext)
        {
            try
            {
                await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return new SuccessResponseResult(null, "Đăng xuất thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi trong quá trình đăng xuất: {Error}", ex.Message);
                return new ErrorResponseResult($"Đã xảy ra lỗi trong quá trình đăng xuất: {ex.Message}");
            }
        }
        // [3.] Lấy trạng thái xác thực của người dùng
        public async Task<AuthStatusVModel> GetAuthStatusAsync(ClaimsPrincipal user, HttpContext httpContext)
        {
            try
            {
                if (user.Identity?.IsAuthenticated != true)
                {
                    return new AuthStatusVModel { IsAuthenticated = false, Message = "Người dùng chưa được xác thực." };
                }

                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return new AuthStatusVModel { IsAuthenticated = false, Message = "Phiên người dùng không hợp lệ." };
                }

                var dbUser = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Id == int.Parse(userId));

                if (dbUser == null)
                {
                    return new AuthStatusVModel { IsAuthenticated = false, Message = "Không tìm thấy người dùng." };
                }

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
                _logger.LogError("Lỗi khi kiểm tra trạng thái xác thực: {Error}", ex.Message);
                return new AuthStatusVModel
                {
                    IsAuthenticated = false,
                    Message = $"Đã xảy ra lỗi khi kiểm tra trạng thái xác thực: {ex.Message}"
                };
            }
        }
        // [4.] Đăng ký người dùng mới
        public async Task<ResponseResult> RegisterAsync(RegisterVModel model)
        {
            try
            {
                _logger.LogInformation("Bắt đầu đăng ký cho email: {Email}", model.Email);
                var validationResult = ValidateRegister(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    _logger.LogWarning("Validate đăng ký thất bại cho {Email}: {Error}", model.Email, validationResult);
                    return new ErrorResponseResult(validationResult);
                }

                var normalizedEmail = model.Email.Trim().ToLower();
                _logger.LogInformation("Kiểm tra email đã tồn tại: {Email}", normalizedEmail);
                if (await _context.Users.AnyAsync(u => u.Email.ToLower() == normalizedEmail))
                {
                    _logger.LogWarning("Email đã được đăng ký: {Email}", normalizedEmail);
                    return new ErrorResponseResult("Email đã được đăng ký.");
                }

                _logger.LogInformation("Tìm vai trò 'User'");
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "User");
                if (role == null)
                {
                    _logger.LogError("Không tìm thấy vai trò 'User' trong cơ sở dữ liệu.");
                    return new ErrorResponseResult("Không tìm thấy vai trò 'User'.");
                }

                _logger.LogInformation("Tạo người dùng mới cho {Email}", normalizedEmail);
                var user = model.ToUserEntity(role.Id); // Sử dụng extension để ánh xạ
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Người dùng được tạo thành công: {Email}, UserId: {UserId}", normalizedEmail, user.Id);

                var otp = GenerateOtp();
                var otpEntity = AuthExtensions.ToConfirmOtpEntity(user.Id, otp); // Sử dụng extension để tạo OTP
                _context.ConfirmOtps.Add(otpEntity);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Tạo OTP cho {Email}: {Otp}, hết hạn lúc {Expiry}", normalizedEmail, otp, otpEntity.ExpiredDate);

                try
                {
                    var emailBody = $"<p>Mã OTP của bạn là: <strong>{otp}</strong>. Mã này có hiệu lực trong 30 phút.</p>";
                    await _emailService.SendEmailAsync(model.Email, "Mã OTP của bạn", emailBody);
                    _logger.LogInformation("Gửi email OTP thành công tới {Email}", normalizedEmail);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Gửi email OTP thất bại tới {Email}: {Error}", normalizedEmail, ex.Message);
                    _context.Users.Remove(user);
                    _context.ConfirmOtps.Remove(otpEntity);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Đã xóa người dùng {Email} và OTP do gửi email thất bại", normalizedEmail);
                    return new ErrorResponseResult("Gửi email OTP thất bại. Vui lòng thử lại.");
                }

                return new SuccessResponseResult(null, "Đăng ký thành công. Vui lòng kiểm tra email để nhận mã OTP.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi trong quá trình đăng ký cho {Email}: {Error}", model.Email, ex.Message);
                return new ErrorResponseResult($"Đã xảy ra lỗi trong quá trình đăng ký: {ex.Message}");
            }
        }
        // [5.] Xác nhận OTP để kích hoạt tài khoản người dùng
        public async Task<ResponseResult> VerifyOtpAsync(ConfirmOtpVModel model)
        {
            try
            {
                _logger.LogInformation("Bắt đầu xác nhận OTP cho email: {Email}", model.Email);

                var validationResult = ValidateOtp(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    _logger.LogWarning("Xác nhận OTP thất bại cho {Email}: {Error}", model.Email, validationResult);
                    return new ErrorResponseResult(validationResult);
                }

                var normalizedEmail = model.Email.Trim().ToLower();
                _logger.LogInformation("Chuẩn hóa email: {OriginalEmail} -> {NormalizedEmail}", model.Email, normalizedEmail);

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);

                if (user == null)
                {
                    _logger.LogWarning("Không tìm thấy người dùng cho email: {Email}", normalizedEmail);
                    return new ErrorResponseResult("Không tìm thấy người dùng.");
                }

                if (user.IsActive == true)
                {
                    _logger.LogWarning("Người dùng đã được kích hoạt: {Email}", normalizedEmail);
                    return new ErrorResponseResult("Tài khoản đã được kích hoạt.");
                }

                var otpData = await _context.ConfirmOtps
                    .FirstOrDefaultAsync(o => o.UserId == user.Id && o.CheckActive == false && o.ExpiredDate >= DateTime.UtcNow);

                if (otpData == null)
                {
                    _logger.LogWarning("Không tìm thấy OTP hợp lệ cho email: {Email}", normalizedEmail);
                    return new ErrorResponseResult("Mã OTP không tồn tại hoặc đã hết hạn. Vui lòng yêu cầu gửi lại OTP.");
                }

                if (otpData.Otpcode != model.Otp.Trim())
                {
                    _logger.LogWarning("OTP không hợp lệ cho email: {Email}, OTP cung cấp: {ProvidedOtp}, OTP mong đợi: {ExpectedOtp}",
                        normalizedEmail, model.Otp, otpData.Otpcode);
                    return new ErrorResponseResult("Mã OTP không hợp lệ.");
                }

                _logger.LogInformation("Kích hoạt người dùng cho {Email}", normalizedEmail);
                user.IsActive = true;
                user.UpdatedDate = DateTime.UtcNow;
                user.UpdatedBy = "System";
                otpData.CheckActive = true;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Người dùng được kích hoạt thành công: {Email}", normalizedEmail);

                return new SuccessResponseResult(null, "Kích hoạt tài khoản thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi trong quá trình xác nhận OTP cho {Email}: {Error}", model.Email, ex.Message);
                return new ErrorResponseResult($"Đã xảy ra lỗi trong quá trình xác nhận OTP: {ex.Message}");
            }
        }
        // [6.] Gửi lại mã OTP cho người dùng
        public async Task<ResponseResult> ResendOtpAsync(string email)
        {
            try
            {
                _logger.LogInformation("Yêu cầu gửi lại OTP cho email: {Email}", email);

                var validationResult = ValidateEmail(email);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    _logger.LogWarning("Yêu cầu gửi lại OTP thất bại cho {Email}: {Error}", email, validationResult);
                    return new ErrorResponseResult(validationResult);
                }

                var normalizedEmail = email.Trim().ToLower();
                _logger.LogInformation("Chuẩn hóa email: {OriginalEmail} -> {NormalizedEmail}", email, normalizedEmail);

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);

                if (user == null)
                {
                    _logger.LogWarning("Không tìm thấy người dùng cho email: {Email}", normalizedEmail);
                    return new ErrorResponseResult("Không tìm thấy người dùng.");
                }

                if (user.IsActive == true)
                {
                    _logger.LogWarning("Người dùng đã được kích hoạt: {Email}", normalizedEmail);
                    return new ErrorResponseResult("Tài khoản đã được kích hoạt.");
                }

                var oldOtps = await _context.ConfirmOtps
                    .Where(o => o.UserId == user.Id && o.CheckActive == false)
                    .ToListAsync();
                foreach (var oldOtp in oldOtps)
                {
                    oldOtp.CheckActive = true;
                }

                var otp = GenerateOtp();
                var otpEntity = AuthExtensions.ToConfirmOtpEntity(user.Id, otp); // Sử dụng extension để tạo OTP
                _context.ConfirmOtps.Add(otpEntity);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Tạo OTP mới cho {Email}: {Otp}, hết hạn lúc {Expiry}", normalizedEmail, otp, otpEntity.ExpiredDate);

                try
                {
                    var emailBody = $"<p>Mã OTP mới của bạn là: <strong>{otp}</strong>. Mã này có hiệu lực trong 30 phút.</p>";
                    await _emailService.SendEmailAsync(email, "Mã OTP mới của bạn", emailBody);
                    _logger.LogInformation("Gửi email OTP mới thành công tới {Email}", normalizedEmail);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Gửi email OTP mới thất bại tới {Email}: {Error}", normalizedEmail, ex.Message);
                    _context.ConfirmOtps.Remove(otpEntity);
                    await _context.SaveChangesAsync();
                    return new ErrorResponseResult("Gửi email OTP mới thất bại. Vui lòng thử lại.");
                }

                return new SuccessResponseResult(null, "Gửi mã OTP mới thành công. Vui lòng kiểm tra email của bạn.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi trong quá trình gửi lại OTP cho {Email}: {Error}", email, ex.Message);
                return new ErrorResponseResult($"Đã xảy ra lỗi trong quá trình gửi lại OTP: {ex.Message}");
            }
        }
        // Tạo mã OTP ngẫu nhiên
        private string GenerateOtp()
        {
            var random = new Random();
            var otp = random.Next(100000, 999999).ToString();
            _logger.LogInformation("Tạo OTP: {Otp}", otp);
            return otp;
        }
    }
}