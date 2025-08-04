using Microsoft.EntityFrameworkCore;
using OA.Domain.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TomsFurnitureBackend.Extensions;
using TomsFurnitureBackend.Helpers;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.Interfaces;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly TomfurnitureContext _context;
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;

        public FeedbackService(TomfurnitureContext context, IAuthService authService, IEmailService emailService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        /// <summary>
        /// Validate dữ liệu phản hồi
        /// </summary>
        private string ValidateFeedback(FeedbackCreateVModel model, bool isAuthenticated)
        {
            if (string.IsNullOrWhiteSpace(model.Message))
            {
                return "Nội dung phản hồi là bắt buộc.";
            }

            if (model.Message.Length > 1000)
            {
                return "Nội dung phản hồi phải dưới 1000 ký tự.";
            }

            if (!isAuthenticated)
            {
                if (string.IsNullOrWhiteSpace(model.UserName))
                {
                    return "Tên người dùng là bắt buộc khi không đăng nhập.";
                }
                if (string.IsNullOrWhiteSpace(model.Email))
                {
                    return "Email là bắt buộc khi không đăng nhập.";
                }
                const string emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!Regex.IsMatch(model.Email.Trim(), emailRegex))
                {
                    return "Định dạng email không hợp lệ.";
                }
                if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
                {
                    var phoneValidation = PhoneNumberHelper.ValidateAndNormalizePhoneNumber(model.PhoneNumber, out string normalizedPhone);
                    if (!string.IsNullOrEmpty(phoneValidation))
                    {
                        return phoneValidation;
                    }
                    model.PhoneNumber = normalizedPhone;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Validate khi tạo mới phản hồi
        /// </summary>
        private async Task<string> ValidateCreate(FeedbackCreateVModel model, HttpContext httpContext)
        {
            var authStatus = await _authService.GetAuthStatusAsync(httpContext.User, httpContext);
            var validationResult = ValidateFeedback(model, authStatus.IsAuthenticated);
            if (!string.IsNullOrEmpty(validationResult))
            {
                return validationResult;
            }

            if (model.ParentFeedbackId.HasValue)
            {
                var parentFeedback = await _context.Feedbacks
                    .FirstOrDefaultAsync(f => f.Id == model.ParentFeedbackId.Value && f.IsActive == true);
                if (parentFeedback == null)
                {
                    return "Phản hồi cha không tồn tại hoặc không hoạt động.";
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Validate khi cập nhật phản hồi
        /// </summary>
        private async Task<string> ValidateUpdate(FeedbackUpdateVModel model, HttpContext httpContext)
        {
            if (model.Id <= 0)
            {
                return "ID phản hồi không hợp lệ.";
            }

            var authStatus = await _authService.GetAuthStatusAsync(httpContext.User, httpContext);
            var validationResult = ValidateFeedback(model, authStatus.IsAuthenticated);
            if (!string.IsNullOrEmpty(validationResult))
            {
                return validationResult;
            }

            return string.Empty;
        }

        /// <summary>
        /// Tạo mới phản hồi
        /// </summary>
        public async Task<ResponseResult> CreateAsync(FeedbackCreateVModel model, HttpContext httpContext)
        {
            try
            {
                // B1: Kiểm tra trạng thái đăng nhập
                var authStatus = await _authService.GetAuthStatusAsync(httpContext.User, httpContext);
                int? userId = authStatus.IsAuthenticated ? authStatus.UserId : null;
                string createdBy = authStatus.IsAuthenticated ? authStatus.UserName ?? "System" : "Guest";

                // B2: Validate dữ liệu
                var validationResult = await ValidateCreate(model, httpContext);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    return new ErrorResponseResult(validationResult);
                }

                // B3: Nếu đăng nhập, sử dụng thông tin từ user
                if (authStatus.IsAuthenticated && userId.HasValue)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId.Value);
                    if (user == null)
                    {
                        return new ErrorResponseResult("Người dùng không tồn tại.");
                    }
                    model.UserName = user.UserName;
                    model.Email = user.Email;
                    model.PhoneNumber = user.PhoneNumber;
                }

                // B4: Chuyển ViewModel sang Entity
                var feedback = model.ToEntity(userId, createdBy);

                // B5: Thêm phản hồi vào DbContext
                _context.Feedbacks.Add(feedback);
                await _context.SaveChangesAsync();

                // B6: Gửi email thông báo
                await FeedbackEmailHelper.SendFeedbackNotificationAsync(_emailService, feedback, true);

                // B7: Trả về kết quả thành công
                var feedbackVM = feedback.ToGetVModel();
                return new SuccessResponseResult(feedbackVM, "Tạo phản hồi thành công.");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"Lỗi khi tạo phản hồi: {ex.Message}");
            }
        }

        /// <summary>
        /// Xóa phản hồi
        /// </summary>
        public async Task<ResponseResult> DeleteAsync(int id)
        {
            try
            {
                var feedback = await _context.Feedbacks
                    .Include(f => f.InverseParentFeedback)
                    .FirstOrDefaultAsync(f => f.Id == id);
                if (feedback == null)
                {
                    return new ErrorResponseResult($"Không tìm thấy phản hồi với ID: {id}.");
                }

                if (feedback.InverseParentFeedback.Any(f => f.IsActive == true))
                {
                    return new ErrorResponseResult("Không thể xóa phản hồi vì có phản hồi con đang hoạt động.");
                }

                _context.Feedbacks.Remove(feedback);
                await _context.SaveChangesAsync();

                return new SuccessResponseResult(null, "Xóa phản hồi thành công.");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"Lỗi khi xóa phản hồi: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy tất cả phản hồi
        /// </summary>
        public async Task<List<FeedbackGetVModel>> GetAllAsync()
        {
            var feedbacks = await _context.Feedbacks
                .Include(f => f.InverseParentFeedback)
                .OrderByDescending(f => f.CreatedDate)
                .ToListAsync();
            return feedbacks.Select(f => f.ToGetVModel()).ToList();
        }

        /// <summary>
        /// Lấy phản hồi theo ID
        /// </summary>
        public async Task<FeedbackGetVModel?> GetByIdAsync(int id)
        {
            var feedback = await _context.Feedbacks
                .Include(f => f.InverseParentFeedback)
                .FirstOrDefaultAsync(f => f.Id == id);
            return feedback?.ToGetVModel();
        }

        /// <summary>
        /// Cập nhật phản hồi
        /// </summary>
        public async Task<ResponseResult> UpdateAsync(FeedbackUpdateVModel model, HttpContext httpContext)
        {
            try
            {
                // B1: Kiểm tra trạng thái đăng nhập
                var authStatus = await _authService.GetAuthStatusAsync(httpContext.User, httpContext);
                string updatedBy = authStatus.IsAuthenticated ? authStatus.UserName ?? "System" : "Guest";

                // B2: Validate dữ liệu
                var validationResult = await ValidateUpdate(model, httpContext);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    return new ErrorResponseResult(validationResult);
                }

                // B3: Tìm phản hồi
                var feedback = await _context.Feedbacks
                    .Include(f => f.InverseParentFeedback)
                    .FirstOrDefaultAsync(f => f.Id == model.Id);
                if (feedback == null)
                {
                    return new ErrorResponseResult($"Không tìm thấy phản hồi với ID: {model.Id}.");
                }

                // B4: Nếu đăng nhập, sử dụng thông tin từ user
                if (authStatus.IsAuthenticated && feedback.UserId.HasValue)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == feedback.UserId.Value);
                    if (user == null)
                    {
                        return new ErrorResponseResult("Người dùng không tồn tại.");
                    }
                    model.UserName = user.UserName;
                    model.Email = user.Email;
                    model.PhoneNumber = user.PhoneNumber;
                }

                // B5: Cập nhật thông tin phản hồi
                feedback.UpdateEntity(model, updatedBy);

                // B6: Lưu thay đổi
                await _context.SaveChangesAsync();

                // B7: Gửi email thông báo
                await FeedbackEmailHelper.SendFeedbackNotificationAsync(_emailService, feedback, false);

                // B8: Trả về ViewModel
                var feedbackVM = feedback.ToGetVModel();
                return new SuccessResponseResult(feedbackVM, "Cập nhật phản hồi thành công.");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"Lỗi khi cập nhật phản hồi: {ex.Message}");
            }
        }
    }
}