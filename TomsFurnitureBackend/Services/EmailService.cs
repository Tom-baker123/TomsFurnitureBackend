using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StoreLinhKien.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Security.Authentication;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TomsFurnitureBackend.Services.IServices;

namespace StoreLinhKien.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration; private readonly ILogger _logger;
        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SendEmailAsync(string toEmail, string subject, string messageBody)
        {
            // Kiểm tra tham số đầu vào
            if (string.IsNullOrWhiteSpace(toEmail))
            {
                _logger.LogError("Địa chỉ email không được để trống.");
                throw new ArgumentException("Địa chỉ email không được để trống.", nameof(toEmail));
            }

            if (string.IsNullOrWhiteSpace(subject))
            {
                _logger.LogError("Tiêu đề email không được để trống.");
                throw new ArgumentException("Tiêu đề email không được để trống.", nameof(subject));
            }

            if (string.IsNullOrWhiteSpace(messageBody))
            {
                _logger.LogError("Nội dung email không được để trống.");
                throw new ArgumentException("Nội dung email không được để trống.", nameof(messageBody));
            }

            // Kiểm tra định dạng email
            if (!Regex.IsMatch(toEmail, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                _logger.LogError("Địa chỉ email không hợp lệ: {Email}", toEmail);
                throw new ArgumentException("Địa chỉ email không hợp lệ.", nameof(toEmail));
            }

            try
            {
                // Đọc cấu hình SMTP
                var smtpServer = _configuration["SmtpSettings:Server"];
                var smtpPortStr = _configuration["SmtpSettings:Port"];
                var senderEmail = _configuration["SmtpSettings:SenderEmail"];
                var senderName = _configuration["SmtpSettings:SenderName"];
                var smtpUsername = _configuration["SmtpSettings:Username"];
                var smtpPassword = _configuration["SmtpSettings:Password"];
                var enableSslStr = _configuration["SmtpSettings:EnableSsl"] ?? "true";

                // Kiểm tra cấu hình
                var missingConfigs = new List<string>();
                if (string.IsNullOrEmpty(smtpServer)) missingConfigs.Add("Server");
                if (string.IsNullOrEmpty(smtpPortStr)) missingConfigs.Add("Port");
                if (string.IsNullOrEmpty(senderEmail)) missingConfigs.Add("SenderEmail");
                if (string.IsNullOrEmpty(smtpUsername)) missingConfigs.Add("Username");
                if (string.IsNullOrEmpty(smtpPassword)) missingConfigs.Add("Password");

                if (missingConfigs.Count > 0)
                {
                    var errorMsg = $"Thiếu cấu hình SMTP: {string.Join(", ", missingConfigs)}";
                    _logger.LogError(errorMsg);
                    throw new InvalidOperationException(errorMsg);
                }

                // Chuyển đổi port
                if (!int.TryParse(smtpPortStr, out var smtpPort) || smtpPort <= 0)
                {
                    var errorMsg = $"Cấu hình SMTP Port không hợp lệ: {smtpPortStr}";
                    _logger.LogError(errorMsg);
                    throw new InvalidOperationException(errorMsg);
                }

                // Chuyển đổi EnableSsl
                if (!bool.TryParse(enableSslStr, out var enableSsl))
                {
                    _logger.LogWarning("Cấu hình EnableSsl không hợp lệ: {EnableSsl}. Sử dụng mặc định: true", enableSslStr);
                    enableSsl = true;
                }

                _logger.LogInformation("Đang gửi email tới {Email} với SMTP Server: {SmtpServer}, Port: {SmtpPort}, SenderEmail: {SenderEmail}, EnableSsl: {EnableSsl}",
                    toEmail, smtpServer, smtpPort, senderEmail, enableSsl);

                // Cấu hình SmtpClient
                using var client = new SmtpClient(smtpServer, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                    EnableSsl = enableSsl,
                    Timeout = 30000, // Timeout 30 giây
                    UseDefaultCredentials = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                // Tạo email
                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName ?? "Store Linh Kiện"),
                    Subject = subject,
                    Body = messageBody,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(toEmail);

                // Gửi email
                await client.SendMailAsync(mailMessage);
                _logger.LogInformation("Gửi email tới {Email} thành công", toEmail);
            }
            catch (SmtpFailedRecipientException ex)
            {
                _logger.LogError(ex, "Không thể gửi email tới {Email}: Địa chỉ người nhận không hợp lệ. StatusCode: {StatusCode}", toEmail, ex.StatusCode);
                throw new Exception("Địa chỉ email người nhận không hợp lệ.", ex);
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, "Lỗi SMTP khi gửi email tới {Email}: StatusCode: {StatusCode}", toEmail, ex.StatusCode);
                throw new Exception($"Lỗi SMTP khi gửi email: {ex.Message}", ex);
            }
            catch (AuthenticationException ex)
            {
                _logger.LogError(ex, "Lỗi xác thực SMTP khi gửi email tới {Email}", toEmail);
                throw new Exception("Lỗi xác thực SMTP. Vui lòng kiểm tra thông tin đăng nhập.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định khi gửi email tới {Email}", toEmail);
                throw new Exception($"Lỗi khi gửi email: {ex.Message}", ex);
            }
        }
    }
}