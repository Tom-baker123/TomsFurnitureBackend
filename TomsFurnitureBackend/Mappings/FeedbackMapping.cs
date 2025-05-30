using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Extensions
{
    public static class FeedbackExtensions
    {
        // Chuyển từ FeedbackCreateVModel sang Entity Feedback
        public static Feedback ToEntity(this FeedbackCreateVModel model)
        {
            // Tạo mới Feedback entity với các giá trị từ ViewModel
            return new Feedback
            {
                Message = model.Message,
                ParentFeedbackId = model.ParentFeedbackId,
                UserId = model.UserId,
                IsActive = true, // Mặc định là true khi tạo mới
                CreatedDate = DateTime.UtcNow // Sử dụng UTC để nhất quán
            };
        }

        // Cập nhật thông tin Entity Feedback từ FeedbackUpdateVModel
        public static void UpdateEntity(this Feedback entity, FeedbackUpdateVModel model)
        {
            // Cập nhật các thuộc tính của entity
            entity.Message = model.Message;
            entity.ParentFeedbackId = model.ParentFeedbackId;
            entity.UserId = model.UserId;
            entity.IsActive = model.IsActive ?? entity.IsActive; // Giữ nguyên nếu không có giá trị mới
            entity.UpdatedDate = DateTime.UtcNow; // Cập nhật thời gian sửa đổi
        }

        // Chuyển từ Entity Feedback sang FeedbackGetVModel
        public static FeedbackGetVModel ToGetVModel(this Feedback entity)
        {
            return new FeedbackGetVModel
            {
                Id = entity.Id,
                Message = entity.Message,
                ParentFeedbackId = entity.ParentFeedbackId,
                UserId = entity.UserId ?? 0,
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