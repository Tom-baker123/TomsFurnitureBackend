using TomsFurnitureBackend.Models;

namespace TomsFurnitureBackend.VModels
{
    // ViewModel để tạo mới thương hiệu
    public class BrandCreateVModel
    {
        // Tên thương hiệu
        public string? BrandName { get; set; }        
    }
    public class BrandUpdateVModel : BrandCreateVModel
    {
        // Id của thương hiệu
        public int Id { get; set; }
        // Trạng thái hoạt động của thương hiệu
        public bool? IsActive { get; set; }
    }
    public class BrandGetVModel : BrandUpdateVModel
    {
        // Ảnh của thương hiệu
        public string? ImageUrl { get; set; }
        // Ngày tạo
        public DateTime? CreatedDate { get; set; }
        // Ngày cập nhật
        public DateTime? UpdatedDate { get; set; }
        // Người tạo
        public string? CreatedBy { get; set; }
        // Người cập nhật
        public string? UpdatedBy { get; set; }
    }
}
