namespace TomsFurnitureBackend.VModels
{
    // ViewModel để Kiểm tra trạng thái đăng nhập
    public class AuthStatusVModel
    {
        public bool IsAuthenticated { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? Message { get; set; }
    }
    // ViewModel để xác thực người dùng đăng ký
    public class RegisterVModel
    {
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Gender { get; set; }
        public string? PhoneNumber { get; set; }
        public string? UserAddress { get; set; }
    }
    // ViewModel để xác thực OTP khi đăng ký
    public class ConfirmOtpVModel
    {
        public string Email { get; set; } = null!;
        public string Otp { get; set; } = null!;
    }
    // ViewModel để gửi lại OTP
    public class ResendOtpVModel
    {
        public string Email { get; set; } = null!;
    }

    // ViewModel mới để trả về thông tin người dùng
    public class UserVModel
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? UserAddress { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string RoleName { get; set; } = null!;
    }

    // ViewModel để yêu cầu quên mật khẩu
    public class ForgotPasswordVModel
    {
        public string Email { get; set; } = null!;
    }

    // ViewModel để đặt lại mật khẩu sau khi xác thực OTP
    public class ResetPasswordVModel
    {
        public string Email { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
    // ViewModel để Định nghĩa kết quả đăng nhập
    public class LoginResultVModel
    {
        public string UserName { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
    // ViewModel để đăng nhập
    public class LoginVModel
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
    // ViewModel để thêm người dùng mới
    public class AddUserVModel
    {
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool Gender { get; set; } // Sửa từ string? sang bool
        public string? PhoneNumber { get; set; }
        public string? UserAddress { get; set; }
        public bool IsActive { get; set; } = true;
        public int RoleId { get; set; }
    }

    // ViewModel để cập nhật người dùng
    public class UpdateUserVModel
    {
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool Gender { get; set; } // Sửa từ string? sang bool
        public string? PhoneNumber { get; set; }
        public string? UserAddress { get; set; }
        public bool? IsActive { get; set; }
        public int RoleId { get; set; }
    }
}