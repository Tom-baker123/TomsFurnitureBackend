using Microsoft.EntityFrameworkCore;
using OA.Domain.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TomsFurnitureBackend.Extensions;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.Interfaces;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services
{
    public class FeedbackService : IFeedbackService
    {
        // Context để truy cập cơ sở dữ liệu
        private readonly TomfurnitureContext _context;

        // Constructor nhận DbContext qua DI
        public FeedbackService(TomfurnitureContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Validation cho phản hồi
        public static string ValidateFeedback(FeedbackCreateVModel model)
        {
            // Kiểm tra Message không được để trống
            if (string.IsNullOrWhiteSpace(model.Message))
            {
                return "Feedback message is required.";
            }

            // Kiểm tra Message không quá 1000 ký tự
            if (model.Message.Length > 1000)
            {
                return "Feedback message must be less than 1000 characters.";
            }

            // Kiểm tra UserId hợp lệ
            if (model.UserId <= 0)
            {
                return "Invalid user ID.";
            }

            return string.Empty; // Trả về chuỗi rỗng nếu không có lỗi
        }

        // Validation cho phương thức tạo mới
        public static string ValidateCreate(FeedbackCreateVModel model)
        {
            return ValidateFeedback(model);
        }

        // Validation cho phương thức cập nhật
        public static string ValidateUpdate(FeedbackUpdateVModel model)
        {
            // Kiểm tra Id hợp lệ
            if (model.Id <= 0)
            {
                return "Invalid feedback ID.";
            }

            // Áp dụng các validation của Create
            return ValidateFeedback(model);
        }

        // [1.] Tạo mới phản hồi
        public async Task<ResponseResult> CreateAsync(FeedbackCreateVModel model)
        {
            try
            {
                // B1: Validate dữ liệu đầu vào
                var validationResult = ValidateCreate(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    return new ErrorResponseResult(validationResult);
                }

                // B2: Kiểm tra UserId tồn tại
                var userExists = await _context.Users.AnyAsync(u => u.Id == model.UserId);
                if (!userExists)
                {
                    return new ErrorResponseResult("User not found.");
                }

                // B3: Kiểm tra ParentFeedbackId nếu có
                if (model.ParentFeedbackId.HasValue)
                {
                    var parentFeedback = await _context.Feedbacks
                        .FirstOrDefaultAsync(f => f.Id == model.ParentFeedbackId.Value && f.IsActive == true);
                    if (parentFeedback == null)
                    {
                        return new ErrorResponseResult("Parent feedback not found or inactive.");
                    }
                }

                // B4: Chuyển ViewModel sang Entity
                var feedback = model.ToEntity();

                // B5: Thêm phản hồi vào DbContext
                _context.Feedbacks.Add(feedback);
                await _context.SaveChangesAsync();

                // B6: Trả về kết quả thành công
                var feedbackVM = feedback.ToGetVModel();
                return new SuccessResponseResult(feedbackVM, "Feedback created successfully.");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"An error occurred while creating the feedback: {ex.Message}");
            }
        }

        // [2.] Xóa phản hồi
        public async Task<ResponseResult> DeleteAsync(int id)
        {
            try
            {
                // B1: Tìm phản hồi theo ID
                var feedback = await _context.Feedbacks
                    .Include(f => f.InverseParentFeedback)
                    .FirstOrDefaultAsync(f => f.Id == id);
                if (feedback == null)
                {
                    return new ErrorResponseResult($"Feedback not found with ID: {id}.");
                }

                // B2: Kiểm tra xem phản hồi có phản hồi con không
                if (feedback.InverseParentFeedback.Any(f => f.IsActive == true))
                {
                    return new ErrorResponseResult("Feedback cannot be deleted because it has active child feedbacks.");
                }

                // B3: Xóa phản hồi
                _context.Feedbacks.Remove(feedback);
                await _context.SaveChangesAsync();

                // B4: Trả về kết quả thành công
                return new SuccessResponseResult(null, "Feedback deleted successfully.");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"An error occurred while deleting the feedback: {ex.Message}");
            }
        }

        // [3.] Lấy tất cả phản hồi
        public async Task<List<FeedbackGetVModel>> GetAllAsync()
        {
            // Lấy tất cả phản hồi từ database và sắp xếp theo CreatedDate (mới nhất trước)
            var feedbacks = await _context.Feedbacks
                .Include(f => f.InverseParentFeedback)
                .OrderByDescending(f => f.CreatedDate)
                .ToListAsync();
            return feedbacks.Select(f => f.ToGetVModel()).ToList();
        }

        // [4.] Lấy phản hồi theo ID
        public async Task<FeedbackGetVModel?> GetByIdAsync(int id)
        {
            // Tìm phản hồi theo ID
            var feedback = await _context.Feedbacks
                .Include(f => f.InverseParentFeedback)
                .FirstOrDefaultAsync(f => f.Id == id);
            return feedback?.ToGetVModel();
        }

        // [5.] Cập nhật phản hồi
        public async Task<ResponseResult> UpdateAsync(FeedbackUpdateVModel model)
        {
            try
            {
                // B1: Kiểm tra dữ liệu đầu vào
                var validationResult = ValidateUpdate(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    return new ErrorResponseResult(validationResult);
                }

                // B2: Tìm phản hồi theo ID
                var feedback = await _context.Feedbacks
                    .Include(f => f.InverseParentFeedback)
                    .FirstOrDefaultAsync(f => f.Id == model.Id);
                if (feedback == null)
                {
                    return new ErrorResponseResult($"Feedback not found with ID: {model.Id}.");
                }

                // B3: Kiểm tra UserId tồn tại
                var userExists = await _context.Users.AnyAsync(u => u.Id == model.UserId);
                if (!userExists)
                {
                    return new ErrorResponseResult("User not found.");
                }

                // B4: Kiểm tra ParentFeedbackId nếu có
                if (model.ParentFeedbackId.HasValue)
                {
                    var parentFeedback = await _context.Feedbacks
                        .FirstOrDefaultAsync(f => f.Id == model.ParentFeedbackId.Value && f.IsActive == true);
                    if (parentFeedback == null)
                    {
                        return new ErrorResponseResult("Parent feedback not found or inactive.");
                    }
                }

                // B5: Cập nhật thông tin phản hồi
                feedback.UpdateEntity(model);

                // B6: Lưu thay đổi
                await _context.SaveChangesAsync();

                // B7: Trả về ViewModel
                var feedbackVM = feedback.ToGetVModel();
                return new SuccessResponseResult(feedbackVM, "Feedback updated successfully.");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"An error occurred while updating the feedback: {ex.Message}");
            }
        }
    }
}