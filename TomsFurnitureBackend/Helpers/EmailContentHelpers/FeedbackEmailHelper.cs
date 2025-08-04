using System;
using System.Threading.Tasks;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.Interfaces;
using TomsFurnitureBackend.Services.IServices;

namespace TomsFurnitureBackend.Helpers
{
    public static class FeedbackEmailHelper
    {
        /// <summary>
        /// Gửi email thông báo về phản hồi cho user và admin
        /// </summary>
        /// <param name="emailService">Dịch vụ gửi email</param>
        /// <param name="feedback">Thông tin phản hồi</param>
        /// <param name="isNew">True nếu là phản hồi mới, False nếu là cập nhật</param>
        public static async Task SendFeedbackNotificationAsync(IEmailService emailService, Feedback feedback, bool isNew)
        {
            try
            {
                // Địa chỉ email admin (lấy từ cấu hình hoặc database)
                const string adminEmail = "freddy19831987@gmail.com"; // Có thể thay bằng cấu hình

                // Nội dung email cho user
                string userSubject = isNew ? "Phản hồi của bạn đã được ghi nhận" : "Phản hồi của bạn đã được cập nhật";
                string userBody = isNew
                    ? $"<p>Xin chào {feedback.UserName},</p><p>Chúng tôi đã nhận được phản hồi của bạn: <strong>{feedback.Message}</strong>.</p><p>Cảm ơn bạn đã đóng góp ý kiến!</p>"
                    : $"<p>Xin chào {feedback.UserName},</p><p>Phản hồi của bạn đã được cập nhật: <strong>{feedback.Message}</strong>.</p><p>Cảm ơn bạn đã tiếp tục đóng góp ý kiến!</p>";

                // Gửi email cho user nếu có email
                if (!string.IsNullOrWhiteSpace(feedback.Email))
                {
                    await emailService.SendEmailAsync(feedback.Email, userSubject, userBody);
                }

                // Nội dung email cho admin
                string adminSubject = isNew ? "Phản hồi mới từ khách hàng" : "Phản hồi được cập nhật";
                string adminBody = isNew
                    ? $"<p>Phản hồi mới từ {feedback.UserName} (Email: {feedback.Email ?? "N/A"}, SĐT: {feedback.PhoneNumber ?? "N/A"}):</p><p><strong>{feedback.Message}</strong></p>"
                    : $"<p>Phản hồi từ {feedback.UserName} (Email: {feedback.Email ?? "N/A"}, SĐT: {feedback.PhoneNumber ?? "N/A"}) đã được cập nhật:</p><p><strong>{feedback.Message}</strong></p>";

                // Gửi email cho admin
                await emailService.SendEmailAsync(adminEmail, adminSubject, adminBody);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nhưng không làm gián đoạn luồng chính
                Console.WriteLine($"Lỗi khi gửi email thông báo phản hồi: {ex.Message}");
            }
        }
    }
}