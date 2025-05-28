namespace TomsFurnitureBackend.VModels
{

    // ViewModel để tạo mới màu sắc
    public class ColorCreateVModel
    {
        public string ColorName { get; set; } = null!;

        public string? ColorCode { get; set; }

    }

    // ViewModel để cập nhật màu sắc, kế thừa từ CreateVModel và thêm Id
    public class ColorUpdateVModel : ColorCreateVModel
    {
        public int Id { get; set; }
        public bool? IsActive { get; set; }

    }

    // ViewModel để lấy thông tin màu sắc, bao gồm các trường bổ sung
    public class ColorGetVModel : ColorUpdateVModel
    {
        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string? CreatedBy { get; set; }

        public string? UpdatedBy { get; set; }
    }

}
