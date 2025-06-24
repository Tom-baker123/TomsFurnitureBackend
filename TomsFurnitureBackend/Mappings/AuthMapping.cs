using System;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Extensions
{
    public static class AuthExtensions
    {
        /// <summary>
        /// Ánh xạ RegisterVModel sang User entity.
        /// </summary>
        /// <param name="model">RegisterVModel chứa dữ liệu đăng ký người dùng.</param>
        /// <param name="roleId">ID của vai trò được gán cho người dùng.</param>
        /// <returns>User entity mới.</returns>
        public static User ToUserEntity(this RegisterVModel model, int roleId)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "RegisterVModel không được null.");
            }

            return new User
            {
                UserName = model.UserName,
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Gender = model.Gender, // Sử dụng trực tiếp bool Gender
                PhoneNumber = model.PhoneNumber,
                UserAddress = model.UserAddress,
                IsActive = false, // Mặc định false, cần xác thực OTP
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "System",
                RoleId = roleId
            };
        }

        /// <summary>
        /// Tạo ConfirmOtp entity từ user ID và mã OTP.
        /// </summary>
        /// <param name="userId">ID của người dùng liên quan đến OTP.</param>
        /// <param name="otpCode">Mã OTP để lưu trữ.</param>
        /// <returns>ConfirmOtp entity mới.</returns>
        public static ConfirmOtp ToConfirmOtpEntity(int userId, string otpCode)
        {
            if (string.IsNullOrWhiteSpace(otpCode))
            {
                throw new ArgumentException("Mã OTP không được null hoặc rỗng.", nameof(otpCode));
            }

            return new ConfirmOtp
            {
                UserId = userId,
                Otpcode = otpCode,
                ExpiredDate = DateTime.UtcNow.AddMinutes(30),
                CreatedDate = DateTime.UtcNow,
                FailedAttempt = 0 // Khởi tạo số lần thất bại bằng 0
            };
        }

        /// <summary>
        /// Cập nhật ConfirmOtp entity với mã OTP mới và thời gian hết hạn.
        /// </summary>
        /// <param name="entity">ConfirmOtp entity cần cập nhật.</param>
        /// <param name="otpCode">Mã OTP mới.</param>
        public static void UpdateConfirmOtpEntity(this ConfirmOtp entity, string otpCode)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "ConfirmOtp entity không được null.");
            }

            if (string.IsNullOrWhiteSpace(otpCode))
            {
                throw new ArgumentException("Mã OTP không được null hoặc rỗng.", nameof(otpCode));
            }

            entity.Otpcode = otpCode;
            entity.ExpiredDate = DateTime.UtcNow.AddMinutes(30);
            entity.CreatedDate = DateTime.UtcNow;
            entity.FailedAttempt = 0; // Reset số lần thất bại khi tạo OTP mới
        }

        /// <summary>
        /// Ánh xạ User entity sang RegisterVModel để trả về dữ liệu.
        /// </summary>
        /// <param name="entity">User entity để ánh xạ.</param>
        /// <returns>RegisterVModel chứa dữ liệu người dùng.</returns>
        public static RegisterVModel ToRegisterVModel(this User entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "User entity không được null.");
            }

            return new RegisterVModel
            {
                UserName = entity.UserName,
                Email = entity.Email,
                Password = string.Empty, // Không trả về mật khẩu đã mã hóa
                Gender = entity.Gender, // Trả về bool Gender
                PhoneNumber = entity.PhoneNumber,
                UserAddress = entity.UserAddress
            };
        }

        /// <summary>
        /// Ánh xạ User và ConfirmOtp entity sang ConfirmOtpVModel để trả về dữ liệu.
        /// </summary>
        /// <param name="user">User entity.</param>
        /// <param name="otp">ConfirmOtp entity.</param>
        /// <returns>ConfirmOtpVModel chứa dữ liệu OTP.</returns>
        public static ConfirmOtpVModel ToConfirmOtpVModel(this User user, ConfirmOtp otp)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User entity không được null.");
            }

            if (otp == null)
            {
                throw new ArgumentNullException(nameof(otp), "ConfirmOtp entity không được null.");
            }

            return new ConfirmOtpVModel
            {
                Email = user.Email,
                Otp = otp.Otpcode
            };
        }

        /// <summary>
        /// Ánh xạ User entity sang UserVModel để trả về thông tin người dùng.
        /// </summary>
        /// <param name="entity">User entity để ánh xạ.</param>
        /// <returns>UserVModel chứa thông tin người dùng.</returns>
        public static UserVModel ToUserVModel(this User entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "User entity không được null.");
            }

            return new UserVModel
            {
                Id = entity.Id,
                UserName = entity.UserName,
                Email = entity.Email,
                Gender = entity.Gender, // Trả về bool Gender
                PhoneNumber = entity.PhoneNumber,
                UserAddress = entity.UserAddress,
                IsActive = entity.IsActive,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate,
                RoleName = entity.Role?.RoleName ?? "Unknown"
            };
        }

        /// <summary>
        /// Ánh xạ AddUserVModel sang User entity.
        /// </summary>
        /// <param name="model">AddUserVModel chứa dữ liệu người dùng.</param>
        /// <returns>User entity mới.</returns>
        public static User ToUserEntity(this AddUserVModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "AddUserVModel không được null.");
            }

            return new User
            {
                UserName = model.UserName,
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Gender = model.Gender, // Sử dụng trực tiếp bool Gender
                PhoneNumber = model.PhoneNumber,
                UserAddress = model.UserAddress,
                IsActive = model.IsActive,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "Admin",
                RoleId = model.RoleId
            };
        }

        /// <summary>
        /// Cập nhật User entity từ UpdateUserVModel.
        /// </summary>
        /// <param name="entity">User entity cần cập nhật.</param>
        /// <param name="model">UpdateUserVModel chứa dữ liệu cập nhật.</param>
        public static void UpdateUserEntity(this User entity, UpdateUserVModel model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "User entity không được null.");
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "UpdateUserVModel không được null.");
            }

            entity.UserName = model.UserName;
            entity.Email = model.Email;
            entity.Gender = model.Gender; // Sử dụng trực tiếp bool Gender
            entity.PhoneNumber = model.PhoneNumber;
            entity.UserAddress = model.UserAddress;
            entity.IsActive = model.IsActive ?? entity.IsActive;
            entity.RoleId = model.RoleId;
            entity.UpdatedDate = DateTime.UtcNow;
            entity.UpdatedBy = "Admin";
        }
    }
}