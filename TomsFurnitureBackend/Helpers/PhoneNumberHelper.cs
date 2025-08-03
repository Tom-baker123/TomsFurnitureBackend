using System.Text.RegularExpressions;

namespace TomsFurnitureBackend.Helpers
{
    /// <summary>
    /// Helper class để xử lý số điện thoại Việt Nam
    /// </summary>
    public static class PhoneNumberHelper
    {
        /// <summary>
        /// Chuẩn hóa số điện thoại Việt Nam
        /// Loại bỏ khoảng trắng, dấu gạch ngang và chuyển +84 thành 0
        /// </summary>
        /// <param name="phoneNumber">Số điện thoại cần chuẩn hóa</param>
        /// <returns>Số điện thoại đã được chuẩn hóa</returns>
        public static string NormalizePhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber)) 
                return string.Empty;
            
            // Loại bỏ tất cả ký tự không phải số (trừ dấu + ở đầu)
            var cleaned = new string(phoneNumber.Where(c => char.IsDigit(c) || c == '+').ToArray());
            
            // Chuyển +84 thành 0
            if (cleaned.StartsWith("+84"))
            {
                cleaned = "0" + cleaned.Substring(3);
            }
            
            return cleaned;
        }

        /// <summary>
        /// Kiểm tra số điện thoại Việt Nam có hợp lệ không
        /// Regex: ^(0[3|5|7|8|9])[0-9]{8}$ (10 số, bắt đầu bằng 03, 05, 07, 08, 09)
        /// </summary>
        /// <param name="phoneNumber">Số điện thoại cần kiểm tra</param>
        /// <returns>True nếu hợp lệ, False nếu không hợp lệ</returns>
        public static bool IsValidVietnamesePhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;
            
            // Regex cho số điện thoại di động Việt Nam: 10 số, bắt đầu bằng 03, 05, 07, 08, 09
            var regex = new Regex(@"^(0[3|5|7|8|9])[0-9]{8}$");
            return regex.IsMatch(phoneNumber);
        }

        /// <summary>
        /// Validate và chuẩn hóa số điện thoại Việt Nam
        /// </summary>
        /// <param name="phoneNumber">Số điện thoại cần validate</param>
        /// <param name="normalizedPhone">Số điện thoại đã được chuẩn hóa (output)</param>
        /// <returns>Thông báo lỗi nếu có, string.Empty nếu hợp lệ</returns>
        public static string ValidateAndNormalizePhoneNumber(string phoneNumber, out string normalizedPhone)
        {
            normalizedPhone = string.Empty;
            
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return string.Empty; // Số điện thoại không bắt buộc
            }
            
            // Chuẩn hóa số điện thoại
            normalizedPhone = NormalizePhoneNumber(phoneNumber);
            
            // Kiểm tra định dạng hợp lệ
            if (!IsValidVietnamesePhoneNumber(normalizedPhone))
            {
                return "Số điện thoại không đúng định dạng Việt Nam (10 số, bắt đầu bằng 03, 05, 07, 08, 09).";
            }
            
            return string.Empty;
        }

        /// <summary>
        /// Validate số điện thoại bắt buộc
        /// </summary>
        /// <param name="phoneNumber">Số điện thoại cần validate</param>
        /// <param name="normalizedPhone">Số điện thoại đã được chuẩn hóa (output)</param>
        /// <returns>Thông báo lỗi nếu có, string.Empty nếu hợp lệ</returns>
        public static string ValidateRequiredPhoneNumber(string phoneNumber, out string normalizedPhone)
        {
            normalizedPhone = string.Empty;
            
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return "Số điện thoại là bắt buộc.";
            }
            
            return ValidateAndNormalizePhoneNumber(phoneNumber, out normalizedPhone);
        }

        /// <summary>
        /// Kiểm tra độ dài số điện thoại (legacy support cho các service khác)
        /// </summary>
        /// <param name="phoneNumber">Số điện thoại cần kiểm tra</param>
        /// <param name="maxLength">Độ dài tối đa cho phép (mặc định 20)</param>
        /// <returns>Thông báo lỗi nếu quá dài, string.Empty nếu hợp lệ</returns>
        public static string ValidatePhoneNumberLength(string phoneNumber, int maxLength = 20)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return string.Empty;
            
            if (phoneNumber.Length > maxLength)
            {
                return $"Số điện thoại không được vượt quá {maxLength} ký tự.";
            }
            
            return string.Empty;
        }
    }
}
