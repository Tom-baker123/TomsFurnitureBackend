using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Extensions
{
    public static class FeedbackExtensions
    {
        /// <summary>
        /// Chuyển từ FeedbackCreateVModel sang Entity Feedback
        /// </summary>
        public static Feedback ToEntity(this FeedbackCreateVModel model, int? userId, string createdBy)
        {
            return new Feedback
            {
                Message = model.Message,
                ParentFeedbackId = model.ParentFeedbackId,
                UserId = userId, // Có thể null nếu không đăng nhập
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = createdBy
            };
        }

        /// <summary>
        /// Cập nhật thông tin Entity Feedback từ FeedbackUpdateVModel
        /// </summary>
        public static void UpdateEntity(this Feedback entity, FeedbackUpdateVModel model, string updatedBy)
        {
            entity.Message = model.Message;
            entity.ParentFeedbackId = model.ParentFeedbackId;
            entity.UserName = model.UserName;
            entity.Email = model.Email;
            entity.PhoneNumber = model.PhoneNumber;
            entity.IsActive = model.IsActive ?? entity.IsActive;
            entity.UpdatedDate = DateTime.UtcNow;
            entity.UpdatedBy = updatedBy;
        }

        /// <summary>
        /// Chuyển từ Entity Feedback sang FeedbackGetVModel
        /// </summary>
        public static FeedbackGetVModel ToGetVModel(this Feedback entity)
        {
            return new FeedbackGetVModel
            {
                Id = entity.Id,
                Message = entity.Message,
                ParentFeedbackId = entity.ParentFeedbackId,
                UserId = entity.UserId,
                UserName = entity.UserName,
                Email = entity.Email,
                PhoneNumber = entity.PhoneNumber,
                IsActive = entity.IsActive,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate,
                CreatedBy = entity.CreatedBy,
                UpdatedBy = entity.UpdatedBy,
                ChildFeedbackIds = entity.InverseParentFeedback
                    .Where(f => f.IsActive == true)
                    .Select(f => f.Id)
                    .ToList()
            };
        }
    }
}