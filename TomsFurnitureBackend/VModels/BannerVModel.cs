using System;

namespace TomsFurnitureBackend.VModels
{
    // ViewModel để tạo mới banner
    public class BannerCreateVModel
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string LinkUrl { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? DisplayOrder { get; set; }
        public string? Position { get; set; }
        public bool? IsActive { get; set; }
        public int? UserId { get; set; }
    }

    // ViewModel để cập nhật banner
    public class BannerUpdateVModel : BannerCreateVModel
    {
        public int Id { get; set; }
    }

    // ViewModel để lấy thông tin banner
    public class BannerGetVModel : BannerUpdateVModel
    {
        public string ImageUrl { get; set; } = null!;
        public string ImageUrlmobile { get; set; } = null!;
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}