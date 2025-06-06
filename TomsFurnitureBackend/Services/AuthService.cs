using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;
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
    // Lớp AuthService triển khai giao diện IAuthService để xử lý các chức năng xác thực
    public class AuthService : IAuthService
    {
        private readonly TomfurnitureContext _context; // Đối tượng context để truy cập cơ sở dữ liệu
        private readonly ILogger<AuthService> _logger; // Logger để ghi log các sự kiện và lỗi
        private readonly IEmailService _emailService; // Dịch vụ gửi email để gửi OTP

        // Constructor: Khởi tạo các dependency cần thiết
        public AuthService(TomfurnitureContext context, ILogger<AuthService> logger, IEmailService emailService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context)); // Kiểm tra context không null
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); // Kiểm tra logger không null
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService)); // Kiểm tra emailService không null
        }
        // Hàm ValidateLogin: Xác thực dữ liệu đầu vào cho đăng nhập
        private static string ValidateLogin(LoginVModel model)
        {
            // B1: Kiểm tra email có rỗng hoặc chỉ chứa khoảng trắng
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                return "Email là bắt buộc.";
            }
            // B2: Kiểm tra định dạng email bằng regex
            const string emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(model.Email.Trim(), emailRegex))
            {
                return "Định dạng email không hợp lệ.";
            }
            // B3: Kiểm tra mật khẩu có rỗng hoặc chỉ chứa khoảng trắng
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                return "Mật khẩu là bắt buộc.";
            }
            // B4: Nếu tất cả hợp lệ, trả về chuỗi rỗng
            return string.Empty;
        }
        // Hàm ValidateRegister: Xác thực dữ liệu đầu vào cho đăng ký
        private static string ValidateRegister(RegisterVModel model)
        {
            // B1: Kiểm tra tên người dùng có rỗng
            if (string.IsNullOrWhiteSpace(model.UserName))
            {
                return "Tên người dùng là bắt buộc.";
            }
            // B2: Kiểm tra email có rỗng
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                return "Email là bắt buộc.";
            }
            // B3: Kiểm tra định dạng email bằng regex
            const string emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(model.Email.Trim(), emailRegex))
            {
                return "Định dạng email không hợp lệ.";
            }
            // B4: Kiểm tra mật khẩu có rỗng
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                return "Mật khẩu là bắt buộc.";
            }
            // B5: Kiểm tra độ dài mật khẩu tối thiểu
            if (model.Password.Length < 6)
            {
                return "Mật khẩu phải có ít nhất 6 ký tự.";
            }
            // B6: Kiểm tra giới tính (nếu có) phải là 'male' hoặc 'female'
            if (!string.IsNullOrWhiteSpace(model.Gender) &&
                model.Gender.Trim().ToLower() != "male" &&
                model.Gender.Trim().ToLower() != "female")
            {
                return "Giới tính phải là 'Male' hoặc 'Female'.";
            }
            // B7: Nếu tất cả hợp lệ, trả về chuỗi rỗng
            return string.Empty;
        }
        // Hàm ValidateEmail: Xác thực email đầu vào
        private static string ValidateEmail(string email)
        {
            // B1: Kiểm tra email có rỗng
            if (string.IsNullOrWhiteSpace(email))
            {
                return "Email là bắt buộc.";
            }
            // B2: Kiểm tra định dạng email bằng regex
            const string emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email.Trim(), emailRegex))
            {
                return "Định dạng email không hợp lệ.";
            }
            // B3: Nếu hợp lệ, trả về chuỗi rỗng
            return string.Empty;
        }
        // Hàm ValidateOtp: Xác thực dữ liệu OTP đầu vào 
        private static string ValidateOtp(ConfirmOtpVModel model)
        {
            // B1: Kiểm tra email có rỗng
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                return "Email là bắt buộc.";
            }
            // B2: Kiểm tra định dạng email bằng regex
            const string emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(model.Email.Trim(), emailRegex))
            {
                return "Định dạng email không hợp lệ.";
            }
            // B3: Kiểm tra OTP có rỗng
            if (string.IsNullOrWhiteSpace(model.Otp))
            {
                return "OTP là bắt buộc.";
            }
            // B4: Nếu hợp lệ, trả về chuỗi rỗng
            return string.Empty;
        }
        // [1.] Hàm LoginAsync: Xử lý đăng nhập người dùng
        public async Task<ResponseResult> LoginAsync(LoginVModel model, HttpContext httpContext)
        {
            try
            {
                // B1: Xác thực dữ liệu đầu vào
                var validationResult = ValidateLogin(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    return new ErrorResponseResult(validationResult);
                }
                // B2: Tìm người dùng trong cơ sở dữ liệu dựa trên email
                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == model.Email.ToLower());

                // B3: Kiểm tra người dùng đã kích hoạt chưa
                if (user == null || !user.IsActive.GetValueOrDefault())
                {
                    return new ErrorResponseResult("Email không hợp lệ hoặc tài khoản chưa được kích hoạt.");
                }

                // B4: Kiểm tra mật khẩu bằng BCrypt
                if (!BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    return new ErrorResponseResult("Mật khẩu không hợp lệ.");
                }

                // B5: Tạo danh sách claims cho xác thực
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.RoleName),
                    new Claim("Email", user.Email)
                };

                // B6: Tạo danh tính xác thực và thuộc tính xác thực
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                };

                // B7: Đăng nhập người dùng bằng cookie authentication
                await httpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // B8: Trả về kết quả thành công với thông tin người dùng
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
                // B9: Ghi log lỗi và trả về kết quả lỗi
                _logger.LogError("Lỗi trong quá trình đăng nhập: {Error}", ex.Message);
                return new ErrorResponseResult($"Đã xảy ra lỗi trong quá trình đăng nhập: {ex.Message}");
            }
        }
        // [2.] Hàm LogoutAsync: Xử lý đăng xuất người dùng
        public async Task<ResponseResult> LogoutAsync(HttpContext httpContext)
        {
            try
            {
                // B1: Đăng xuất người dùng bằng cookie authentication
                await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                // B2: Trả về kết quả thành công
                return new SuccessResponseResult(null, "Đăng xuất thành công.");
            }
            catch (Exception ex)
            {
                // B3: Ghi log lỗi và trả về kết quả lỗi
                _logger.LogError("Lỗi trong quá trình đăng xuất: {Error}", ex.Message);
                return new ErrorResponseResult($"Đã xảy ra lỗi trong quá trình đăng xuất: {ex.Message}");
            }
        }

        // [3.] Hàm GetAuthStatusAsync: Kiểm tra trạng thái xác thực của người dùng
        public async Task<AuthStatusVModel> GetAuthStatusAsync(ClaimsPrincipal user, HttpContext httpContext)
        {
            try
            {
                // B1: Kiểm tra người dùng đã xác thực chưa
                if (user.Identity?.IsAuthenticated != true)
                {
                    return new AuthStatusVModel { IsAuthenticated = false, Message = "Người dùng chưa được xác thực." };
                }

                // B2: Lấy ID người dùng từ claims
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return new AuthStatusVModel { IsAuthenticated = false, Message = "Phiên người dùng không hợp lệ." };
                }
                // B3: Tìm người dùng trong cơ sở dữ liệu dựa trên ID
                var dbUser = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Id == int.Parse(userId));
                // B4: Kiểm tra người dùng có tồn tại không
                if (dbUser == null)
                {
                    return new AuthStatusVModel { IsAuthenticated = false, Message = "Không tìm thấy người dùng." };
                }
                // B5: Trả về trạng thái xác thực với thông tin người dùng
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
                // B6: Ghi log lỗi và trả về trạng thái lỗi
                _logger.LogError("Lỗi khi kiểm tra trạng thái xác thực: {Error}", ex.Message);
                return new AuthStatusVModel
                {
                    IsAuthenticated = false,
                    Message = $"Đã xảy ra lỗi khi kiểm tra trạng thái xác thực: {ex.Message}"
                };
            }
        }

        // [4.] Hàm RegisterAsync: Xử lý đăng ký người dùng mới
        public async Task<ResponseResult> RegisterAsync(RegisterVModel model)
        {
            try
            {
                // B1: Ghi log bắt đầu quá trình đăng ký
                _logger.LogInformation("Bắt đầu đăng ký cho email: {Email}", model.Email);
                // B2: Xác thực dữ liệu đầu vào
                var validationResult = ValidateRegister(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    _logger.LogWarning("Validate đăng ký thất bại cho {Email}: {Error}", model.Email, validationResult);
                    return new ErrorResponseResult(validationResult);
                }
                // B3: Chuẩn hóa email
                var normalizedEmail = model.Email.Trim().ToLower();
                _logger.LogInformation("Kiểm tra email đã tồn tại: {Email}", normalizedEmail);
                // B4: Kiểm tra email đã được đăng ký chưa
                if (await _context.Users.AnyAsync(u => u.Email.ToLower() == normalizedEmail))
                {
                    _logger.LogWarning("Email đã được đăng ký: {Email}", normalizedEmail);
                    return new ErrorResponseResult("Email đã được đăng ký.");
                }
                // B5: Tìm vai trò 'User' trong cơ sở dữ liệu
                _logger.LogInformation("Tìm vai trò 'User'");
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "User");
                if (role == null)
                {
                    _logger.LogError("Không tìm thấy vai trò 'User' trong cơ sở dữ liệu.");
                    return new ErrorResponseResult("Không tìm thấy vai trò 'User'.");
                }
                // B6: Tạo entity người dùng từ dữ liệu đăng ký
                _logger.LogInformation("Tạo người dùng mới cho {Email}", normalizedEmail);
                var user = model.ToUserEntity(role.Id);
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Người dùng được tạo thành công: {Email}, UserId: {UserId}", normalizedEmail, user.Id);

                // B7: Tạo và lưu OTP cho người dùng
                var otp = GenerateOtp();
                var otpEntity = AuthExtensions.ToConfirmOtpEntity(user.Id, otp);
                _context.ConfirmOtps.Add(otpEntity);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Tạo OTP cho {Email}: {Otp}, hết hạn lúc {Expiry}", normalizedEmail, otp, otpEntity.ExpiredDate);

                try
                {
                    // B8: Gửi email chứa OTP
                    var emailBody = $"<p>Mã OTP của bạn là: <strong>{otp}</strong>. Mã này có hiệu lực trong 30 phút.</p>";
                    await _emailService.SendEmailAsync(model.Email, "Mã OTP của bạn", emailBody);
                    _logger.LogInformation("Gửi email OTP thành công tới {Email}", normalizedEmail);
                }
                catch (Exception ex)
                {
                    // B9: Nếu gửi email thất bại, xóa người dùng và OTP đã tạo
                    _logger.LogError("Gửi email OTP thất bại tới {Email}: {Error}", normalizedEmail, ex.Message);
                    _context.Users.Remove(user);
                    _context.ConfirmOtps.Remove(otpEntity);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Đã xóa người dùng {Email} và OTP do gửi email thất bại", normalizedEmail);
                    return new ErrorResponseResult("Gửi email OTP thất bại. Vui lòng thử lại.");
                }

                // B10: Trả về kết quả thành công
                return new SuccessResponseResult(null, "Đăng ký thành công. Vui lòng kiểm tra email để nhận mã OTP.");
            }
            catch (Exception ex)
            {
                // B11: Ghi log lỗi và trả về kết quả lỗi
                _logger.LogError("Lỗi trong quá trình đăng ký cho {Email}: {Error}", model.Email, ex.Message);
                return new ErrorResponseResult($"Đã xảy ra lỗi trong quá trình đăng ký: {ex.Message}");
            }
        }
        // [5.] Hàm VerifyOtpAsync: Xác nhận mã OTP để kích hoạt tài khoản
        public async Task<ResponseResult> VerifyOtpAsync(ConfirmOtpVModel model)
        {
            try
            {
                // B1: Ghi log bắt đầu xác nhận OTP
                _logger.LogInformation("Bắt đầu xác nhận OTP cho email: {Email}", model.Email);

                // B2: Xác thực dữ liệu OTP đầu vào
                var validationResult = ValidateOtp(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    _logger.LogWarning("Xác nhận OTP thất bại cho {Email}: {Error}", model.Email, validationResult);
                    return new ErrorResponseResult(validationResult);
                }

                // B3: Chuẩn hóa email
                var normalizedEmail = model.Email.Trim().ToLower();
                _logger.LogInformation("Chuẩn hóa email: {OriginalEmail} -> {NormalizedEmail}", model.Email, normalizedEmail);
                // B4: Tìm người dùng dựa trên email
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);
                // B5: Kiểm tra tài khoản đã được kích hoạt chưa
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
                // B6: Tìm OTP hợp lệ (chưa kích hoạt và chưa hết hạn)
                var otpData = await _context.ConfirmOtps
                    .Where(o => o.UserId == user.Id && o.CheckActive == false && o.ExpiredDate >= DateTime.UtcNow)
                    .OrderByDescending(o => o.CreatedDate)
                    .FirstOrDefaultAsync();

                if (otpData == null)
                {
                    _logger.LogWarning("Không tìm thấy OTP hợp lệ cho email: {Email}", normalizedEmail);
                    return new ErrorResponseResult("Mã OTP không tồn tại hoặc đã hết hạn. Vui lòng yêu cầu gửi lại OTP.");
                }

                // B7: Đếm số lần nhập sai OTP trong 30 phút gần nhất
                var failedAttempts = await _context.ConfirmOtps
                    .Where(o => o.UserId == user.Id && o.CheckActive == false && o.ExpiredDate >= DateTime.UtcNow)
                    .CountAsync();

                // B8: Kiểm tra nếu số lần nhập sai vượt quá 3
                if (failedAttempts >= 3)
                {
                    _logger.LogWarning("Vượt quá 3 lần nhập sai OTP cho email: {Email}", normalizedEmail);
                    return new ErrorResponseResult("Bạn đã nhập sai mã OTP quá 3 lần, hãy bấm gửi lại mã OTP để hoàn tất việc đăng ký.");
                }
                // B9: Kiểm tra OTP có khớp không
                if (otpData.Otpcode != model.Otp.Trim())
                {
                    _logger.LogWarning("OTP không hợp lệ cho email: {Email}, OTP cung cấp: {ProvidedOtp}, OTP mong đợi: {ExpectedOtp}",
                        normalizedEmail, model.Otp, otpData.Otpcode);

                    // B10: Ghi nhận lần nhập sai bằng cách tạo bản ghi OTP mới với cùng mã OTP
                    var failedOtpEntity = AuthExtensions.ToConfirmOtpEntity(user.Id, otpData.Otpcode);
                    _context.ConfirmOtps.Add(failedOtpEntity);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Ghi nhận lần nhập sai OTP cho {Email}, số lần sai hiện tại: {FailedAttempts}", normalizedEmail, failedAttempts + 1);

                    return new ErrorResponseResult("Mã OTP không hợp lệ.");
                }

                // B11: Kích hoạt tài khoản nếu OTP khớp
                _logger.LogInformation("Kích hoạt người dùng cho {Email}", normalizedEmail);
                user.IsActive = true;
                user.UpdatedDate = DateTime.UtcNow;
                user.UpdatedBy = "System";
                otpData.CheckActive = true;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Người dùng được kích hoạt thành công: {Email}", normalizedEmail);
                // B12: Trả về kết quả thành công
                return new SuccessResponseResult(null, "Kích hoạt tài khoản thành công.");
            }
            catch (Exception ex)
            {
                // B13: Ghi log lỗi và trả về kết quả lỗi
                _logger.LogError("Lỗi trong quá trình xác nhận OTP cho {Email}: {Error}", model.Email, ex.Message);
                return new ErrorResponseResult($"Đã xảy ra lỗi trong quá trình xác nhận OTP: {ex.Message}");
            }
        }
        // [6.] Hàm ResendOtpAsync: Gửi lại mã OTP cho người dùng
        public async Task<ResponseResult> ResendOtpAsync(string email)
        {
            try
            {
                // B1: Ghi log yêu cầu gửi lại OTP
                _logger.LogInformation("Yêu cầu gửi lại OTP cho email: {Email}", email);
                // B2: Xác thực email đầu vào
                var validationResult = ValidateEmail(email);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    _logger.LogWarning("Yêu cầu gửi lại OTP thất bại cho {Email}: {Error}", email, validationResult);
                    return new ErrorResponseResult(validationResult);
                }
                // B3: Chuẩn hóa email
                var normalizedEmail = email.Trim().ToLower();
                _logger.LogInformation("Chuẩn hóa email: {OriginalEmail} -> {NormalizedEmail}", email, normalizedEmail);
                // B4: Tìm người dùng dựa trên email
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);

                if (user == null)
                {
                    _logger.LogWarning("Không tìm thấy người dùng cho email: {Email}", normalizedEmail);
                    return new ErrorResponseResult("Không tìm thấy người dùng.");
                }
                // B5: Kiểm tra tài khoản đã được kích hoạt chưa
                if (user.IsActive == true)
                {
                    _logger.LogWarning("Người dùng đã được kích hoạt: {Email}", normalizedEmail);
                    return new ErrorResponseResult("Tài khoản đã được kích hoạt.");
                }

                // B6: Kiểm tra thời gian gửi OTP gần nhất
                var latestOtp = await _context.ConfirmOtps
                    .Where(o => o.UserId == user.Id)
                    .OrderByDescending(o => o.CreatedDate)
                    .FirstOrDefaultAsync();
                // B7: Nếu OTP gần nhất được gửi trong vòng 2 phút, yêu cầu đợi
                if (latestOtp != null && latestOtp.CreatedDate.HasValue && latestOtp.CreatedDate.Value.AddMinutes(1) > DateTime.UtcNow)
                {
                    _logger.LogWarning("Yêu cầu gửi lại OTP quá sớm cho {Email}. Vui lòng đợi thêm.", normalizedEmail);
                    return new ErrorResponseResult("Bạn đã gửi mã OTP thành công, vui lòng đợi sau 1 phút để lấy lại OTP.");
                }

                // B8: Vô hiệu hóa các OTP cũ (chưa kích hoạt)
                var oldOtps = await _context.ConfirmOtps
                    .Where(o => o.UserId == user.Id && o.CheckActive == false)
                    .ToListAsync();
                foreach (var oldOtp in oldOtps)
                {
                    oldOtp.CheckActive = true;
                }
                // B9: Tạo OTP mới và lưu vào cơ sở dữ liệu
                var otp = GenerateOtp();
                var otpEntity = AuthExtensions.ToConfirmOtpEntity(user.Id, otp);
                _context.ConfirmOtps.Add(otpEntity);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Tạo OTP mới cho {Email}: {Otp}, hết hạn lúc {Expiry}", normalizedEmail, otp, otpEntity.ExpiredDate);

                try
                {
                    // B10: Gửi email chứa OTP mới
                    var emailBody = $"<p>Mã OTP mới của bạn là: <strong>{otp}</strong>. Mã này có hiệu lực trong 30 phút.</p>";
                    await _emailService.SendEmailAsync(email, "Mã OTP mới của bạn", emailBody);
                    _logger.LogInformation("Gửi email OTP mới thành công tới {Email}", normalizedEmail);
                }
                catch (Exception ex)
                {
                    // B11: Nếu gửi email thất bại, xóa OTP mới
                    _logger.LogError("Gửi email OTP mới thất bại tới {Email}: {Error}", normalizedEmail, ex.Message);
                    _context.ConfirmOtps.Remove(otpEntity);
                    await _context.SaveChangesAsync();
                    return new ErrorResponseResult("Gửi email OTP mới thất bại. Vui lòng thử lại.");
                }

                // B12: Trả về kết quả thành công
                return new SuccessResponseResult(null, "Gửi mã OTP mới thành công. Vui lòng kiểm tra email của bạn.");
            }
            catch (Exception ex)
            {
                // B13: Ghi log lỗi và trả về kết quả lỗi
                _logger.LogError("Lỗi trong quá trình gửi lại OTP cho {Email}: {Error}", email, ex.Message);
                return new ErrorResponseResult($"Đã xảy ra lỗi trong quá trình gửi lại OTP: {ex.Message}");
            }
        }
        // [7.] Hàm GenerateOtp: Tạo mã OTP ngẫu nhiên 6 chữ số
        private string GenerateOtp()
        {
            // B1: Tạo số ngẫu nhiên 6 chữ số
            var random = new Random();
            var otp = random.Next(100000, 999999).ToString();
            // B2: Ghi log mã OTP đã tạo
            _logger.LogInformation("Tạo OTP: {Otp}", otp);

            // B3: Trả về mã OTP
            return otp;
        }
        // [8.] Phương thức lấy tất cả người dùng
        public async Task<ResponseResult> GetAllUsersAsync()
        {
            try
            {
                // B1: Lấy tất cả người dùng từ cơ sở dữ liệu + Role
                var users = await _context.Users
                    .Include(u => u.Role)
                    .ToListAsync();
                // B2: Chuyển đổi danh sách người dùng sang danh sách UserVModel
                var userVModels = users.Select(u => u.ToUserVModel()).ToList();

                // B3: Trả về kết quả thành công với danh sách người dùng
                return new SuccessResponseResult(userVModels, "Get all user methods success!");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"An error occured when get all user with message: {ex.Message}");
            }
        }
        // [9.] Phương thức lấy tất người dùng theo ID
        public async Task<ResponseResult> GetUserByIdAsync(int id)
        {
            try
            {
                // B1: Tìm người dùng theo ID với Role info trong DB
                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    return new ErrorResponseResult("User not found."); // Nếu không tìm thấy người dùng, trả về lỗi
                }

                // B2: Chuyển đổi người dùng sang UserVModel
                var userVModel = user.ToUserVModel();

                // B3: Trả về kết quả thành công với thông tin người dùng
                return new SuccessResponseResult(userVModel, "Get user success");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"An error occured when get user by id with message: {ex.Message}"); // Xử lý lỗi và trả về thông báo lỗi
            }
        }
        // [10.] Phương thức xóa người dùng theo ID
        public async Task<ResponseResult> DeleteUserAsync(int id)
        {
            try
            {
                _logger.LogInformation("Bắt đầu xóa người dùng với ID: {UserId}", id);

                // B1: Tìm người dùng theo ID
                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    _logger.LogWarning("Không tìm thấy người dùng với ID: {UserId}", id);
                    return new ErrorResponseResult("Không tìm thấy người dùng.");
                }

                // B2: Không cho phép xóa tài khoản admin
                if (user.Role.RoleName == "Admin")
                {
                    _logger.LogWarning("Không thể xóa tài khoản admin với ID: {UserId}", id);
                    return new ErrorResponseResult("Không thể xóa tài khoản admin.");
                }

                // B3: Sử dụng transaction để đảm bảo tính toàn vẹn dữ liệu
                await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // B3.1: Xóa các bản ghi liên quan không quan trọng
                    var confirmOtps = await _context.ConfirmOtps
                        .Where(o => o.UserId == id)
                        .ToListAsync();
                    if (confirmOtps.Any())
                    {
                        _context.ConfirmOtps.RemoveRange(confirmOtps);
                        _logger.LogInformation("Xóa {Count} bản ghi ConfirmOtps cho người dùng {UserId}.", confirmOtps.Count, id);
                    }

                    var carts = await _context.Carts
                        .Where(c => c.UserId == id)
                        .ToListAsync();
                    if (carts.Any())
                    {
                        _context.Carts.RemoveRange(carts);
                        _logger.LogInformation("Xóa {Count} bản ghi Carts cho người dùng {UserId}.", carts.Count, id);
                    }

                    var comments = await _context.Comments
                        .Where(c => c.UserId == id)
                        .ToListAsync();
                    if (comments.Any())
                    {
                        _context.Comments.RemoveRange(comments);
                        _logger.LogInformation("Xóa {Count} bản ghi Comments cho người dùng {UserId}.", comments.Count, id);
                    }

                    var feedbacks = await _context.Feedbacks
                        .Where(f => f.UserId == id)
                        .ToListAsync();
                    if (feedbacks.Any())
                    {
                        _context.Feedbacks.RemoveRange(feedbacks);
                        _logger.LogInformation("Xóa {Count} bản ghi Feedbacks cho người dùng {UserId}.", feedbacks.Count, id);
                    }

                    var productReviews = await _context.ProductReviews
                        .Where(pr => pr.UserId == id)
                        .ToListAsync();
                    if (productReviews.Any())
                    {
                        _context.ProductReviews.RemoveRange(productReviews);
                        _logger.LogInformation("Xóa {Count} bản ghi ProductReviews cho người dùng {UserId}.", productReviews.Count, id);
                    }

                    // B3.2: Vô hiệu hóa các bản ghi quan trọng thay vì xóa
                    var orders = await _context.Orders
                        .Where(o => o.UserId == id)
                        .ToListAsync();
                    foreach (var order in orders)
                    {
                        order.IsActive = false; // Vô hiệu hóa đơn hàng
                        _logger.LogInformation("Vô hiệu hóa đơn hàng {OrderId} cho người dùng {UserId}.", order.Id, id);
                    }

                    var orderAddresses = await _context.OrderAddresses
                        .Where(oa => oa.UserId == id)
                        .ToListAsync();
                    foreach (var address in orderAddresses)
                    {
                        address.IsActive = false; // Vô hiệu hóa địa chỉ
                        _logger.LogInformation("Vô hiệu hóa địa chỉ {AddressId} cho người dùng {UserId}.", address.Id, id);
                    }

                    var shippings = await _context.Shippings
                        .Where(s => s.UserId == id)
                        .ToListAsync();
                    foreach (var shipping in shippings)
                    {
                        shipping.IsActive = false; // Vô hiệu hóa thông tin vận chuyển
                        _logger.LogInformation("Vô hiệu hóa thông tin vận chuyển {ShippingId} cho người dùng {UserId}.", shipping.Id, id);
                    }

                    // B3.3: Kiểm tra các bản ghi không được xóa (Banners, News)
                    var banners = await _context.Banners
                        .Where(b => b.UserId == id)
                        .ToListAsync();
                    if (banners.Any())
                    {
                        _logger.LogWarning("Người dùng {UserId} có {Count} banner. Không thể xóa.", id, banners.Count);
                        await transaction.RollbackAsync();
                        return new ErrorResponseResult("Không thể xóa người dùng vì có banner liên quan.");
                    }

                    var news = await _context.News
                        .Where(n => n.UserId == id)
                        .ToListAsync();
                    if (news.Any())
                    {
                        _logger.LogWarning("Người dùng {UserId} có {Count} tin tức. Không thể xóa.", id, news.Count);
                        await transaction.RollbackAsync();
                        return new ErrorResponseResult("Không thể xóa người dùng vì có tin tức liên quan.");
                    }

                    // B3.4: Xóa người dùng
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation("Xóa người dùng {UserId} thành công.", id);
                    return new SuccessResponseResult(null, "Xóa người dùng thành công.");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError("Lỗi khi xóa các bản ghi liên quan cho người dùng {UserId}: {Error}", id, ex.Message);
                    return new ErrorResponseResult($"Lỗi khi xóa các bản ghi liên quan: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Lỗi khi xóa người dùng {UserId}: {Error}", id, ex.Message);
                return new ErrorResponseResult($"Đã xảy ra lỗi khi xóa người dùng: {ex.Message}");
            }
        }
    }
}