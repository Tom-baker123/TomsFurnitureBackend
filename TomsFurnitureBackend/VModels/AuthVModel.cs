namespace TomsFurnitureBackend.VModels
{
    public class RegisterVModel
    {
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Gender { get; set; }
        public string? PhoneNumber { get; set; }
        public string? UserAddress { get; set; }
    }

    public class ConfirmOtpVModel
    {
        public string Email { get; set; } = null!;
        public string Otp { get; set; } = null!;
    }

    public class ResendOtpVModel
    {
        public string Email { get; set; } = null!;
    }
}