using System;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Extensions
{
    public static class AuthExtensions
    {
        /// <summary>
        /// Maps RegisterVModel to User entity.
        /// </summary>
        /// <param name="model">The RegisterVModel containing user registration data.</param>
        /// <param name="roleId">The ID of the role to assign to the user.</param>
        /// <returns>A new User entity.</returns>
        /// <exception cref="ArgumentException">Thrown when Gender is invalid.</exception>
        public static User ToUserEntity(this RegisterVModel model, int roleId)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "RegisterVModel cannot be null.");
            }

            bool gender;
            if (string.IsNullOrWhiteSpace(model.Gender))
            {
                gender = false; // Default to false if Gender is null or empty
            }
            else
            {
                var normalizedGender = model.Gender.Trim().ToLower();
                if (normalizedGender != "male" && normalizedGender != "female")
                {
                    throw new ArgumentException("Gender must be 'male' or 'female'.", nameof(model.Gender));
                }
                gender = normalizedGender == "male";
            }

            return new User
            {
                UserName = model.UserName,
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password), // Hash password
                Gender = gender,
                PhoneNumber = model.PhoneNumber,
                UserAddress = model.UserAddress,
                IsActive = false, // Default for new users, requires OTP verification
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "System",
                RoleId = roleId
            };
        }

        /// <summary>
        /// Creates a ConfirmOtp entity from user ID and OTP code.
        /// </summary>
        /// <param name="userId">The ID of the user associated with the OTP.</param>
        /// <param name="otpCode">The OTP code to store.</param>
        /// <returns>A new ConfirmOtp entity.</returns>
        public static ConfirmOtp ToConfirmOtpEntity(int userId, string otpCode)
        {
            if (string.IsNullOrWhiteSpace(otpCode))
            {
                throw new ArgumentException("OTP code cannot be null or empty.", nameof(otpCode));
            }

            return new ConfirmOtp
            {
                UserId = userId,
                Otpcode = otpCode,
                ExpiredDate = DateTime.UtcNow.AddMinutes(30),
                CheckActive = false,
                CreatedDate = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Updates ConfirmOtp entity with new OTP code and expiration.
        /// </summary>
        /// <param name="entity">The ConfirmOtp entity to update.</param>
        /// <param name="otpCode">The new OTP code.</param>
        public static void UpdateConfirmOtpEntity(this ConfirmOtp entity, string otpCode)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "ConfirmOtp entity cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(otpCode))
            {
                throw new ArgumentException("OTP code cannot be null or empty.", nameof(otpCode));
            }

            entity.Otpcode = otpCode;
            entity.ExpiredDate = DateTime.UtcNow.AddMinutes(30);
            entity.CheckActive = false;
            entity.CreatedDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Maps User entity to RegisterVModel for response purposes.
        /// </summary>
        /// <param name="entity">The User entity to map.</param>
        /// <returns>A RegisterVModel with user data.</returns>
        public static RegisterVModel ToRegisterVModel(this User entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "User entity cannot be null.");
            }

            return new RegisterVModel
            {
                UserName = entity.UserName,
                Email = entity.Email,
                Password = string.Empty, // Do not return password hash
                Gender = entity.Gender ? "male" : "female",
                PhoneNumber = entity.PhoneNumber,
                UserAddress = entity.UserAddress
            };
        }

        /// <summary>
        /// Maps User and ConfirmOtp entities to ConfirmOtpVModel for response purposes.
        /// </summary>
        /// <param name="user">The User entity.</param>
        /// <param name="otp">The ConfirmOtp entity.</param>
        /// <returns>A ConfirmOtpVModel with OTP data.</returns>
        public static ConfirmOtpVModel ToConfirmOtpVModel(this User user, ConfirmOtp otp)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User entity cannot be null.");
            }

            if (otp == null)
            {
                throw new ArgumentNullException(nameof(otp), "ConfirmOtp entity cannot be null.");
            }

            return new ConfirmOtpVModel
            {
                Email = user.Email,
                Otp = otp.Otpcode
            };
        }

        // Ánh xạ mới: User entity sang UserVModel
        public static UserVModel ToUserVModel(this User entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "User entity cannot be null.");
            }

            return new UserVModel
            {
                Id = entity.Id,
                UserName = entity.UserName,
                Email = entity.Email,
                Gender = entity.Gender ? "male" : "female",
                PhoneNumber = entity.PhoneNumber,
                UserAddress = entity.UserAddress,
                IsActive = entity.IsActive,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate,
                RoleName = entity.Role?.RoleName ?? "Unknown"
            };
        }
        // Ánh xạ AddUserVModel sang User entity
        public static User ToUserEntity(this AddUserVModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "AddUserVModel cannot be null.");
            }

            bool gender;
            if (string.IsNullOrWhiteSpace(model.Gender))
            {
                gender = false; // Mặc định là false nếu không có Gender
            }
            else
            {
                var normalizedGender = model.Gender.Trim().ToLower();
                if (normalizedGender != "male" && normalizedGender != "female")
                {
                    throw new ArgumentException("Gender must be 'male' or 'female'.", nameof(model.Gender));
                }
                gender = normalizedGender == "male";
            }

            return new User
            {
                UserName = model.UserName,
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Gender = gender,
                PhoneNumber = model.PhoneNumber,
                UserAddress = model.UserAddress,
                IsActive = model.IsActive,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "Admin",
                RoleId = model.RoleId
            };
        }

        // Cập nhật User entity từ UpdateUserVModel
        public static void UpdateUserEntity(this User entity, UpdateUserVModel model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "User entity cannot be null.");
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "UpdateUserVModel cannot be null.");
            }

            bool gender;
            if (string.IsNullOrWhiteSpace(model.Gender))
            {
                gender = entity.Gender; // Giữ nguyên nếu không cung cấp
            }
            else
            {
                var normalizedGender = model.Gender.Trim().ToLower();
                if (normalizedGender != "male" && normalizedGender != "female")
                {
                    throw new ArgumentException("Gender must be 'male' or 'female'.", nameof(model.Gender));
                }
                gender = normalizedGender == "male";
            }

            entity.UserName = model.UserName;
            entity.Email = model.Email;
            entity.Gender = gender;
            entity.PhoneNumber = model.PhoneNumber;
            entity.UserAddress = model.UserAddress;
            entity.IsActive = model.IsActive ?? entity.IsActive;
            entity.RoleId = model.RoleId;
            entity.UpdatedDate = DateTime.UtcNow;
            entity.UpdatedBy = "Admin";
        }
    }
}