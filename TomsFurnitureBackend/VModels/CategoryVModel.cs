namespace TomsFurnitureBackend.VModels
{
    // CreateVModel: ViewModel để tạo mới danh mục sản phẩm, bao gồm các trường cần thiết
    public class CategoryCreateVModel
    {
        public string? CategoryName { get; set; }
        public string? Descriptions { get; set; } 
    }

    // UpdateVModel - CreateVModel: ViewModel để cập nhật danh mục sản phẩm, kế thừa từ CreateVModel và thêm Id
    public class CategoryUpdateVModel : CategoryCreateVModel
    {
        public int Id { get; set; }
        public bool? IsActive { get; set; }
    }

    // GetVModel - UpdateVModel: ViewModel để lấy thông tin danh mục, bao gồm các trường bổ sung
    public class CategoryGetVModel : CategoryUpdateVModel
    {
        public string? ImageUrl { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}


