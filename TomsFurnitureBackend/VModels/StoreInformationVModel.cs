using System;

namespace TomsFurnitureBackend.VModels
{
    // ViewModel để tạo mới thông tin cửa hàng
    public class StoreInformationCreateVModel
    {
        // Tên cửa hàng, không bắt buộc
        public string? StoreName { get; set; }

        // Địa chỉ cửa hàng, không bắt buộc
        public string? StoreAddress { get; set; }

        // Số điện thoại, không bắt buộc
        public string? PhoneNumber { get; set; }

        // Email, không bắt buộc
        public string? Email { get; set; }

        // Link website, không bắt buộc
        public string? LinkWebsite { get; set; }

        // Vĩ độ, không bắt buộc
        public decimal? Latitude { get; set; }

        // Kinh độ, không bắt buộc
        public decimal? Longitude { get; set; }

        // Tên chủ sở hữu, không bắt buộc
        public string? OwnerName { get; set; }

        // Loại hình kinh doanh, bắt buộc
        public string BusinessType { get; set; } = null!;

        // Giờ hoạt động, không bắt buộc
        public string? OperatingHours { get; set; }

        // Mô tả cửa hàng, không bắt buộc
        public string? StoreDescription { get; set; }

        // Ngày thành lập, không bắt buộc
        public DateOnly? EstablishmentDate { get; set; }

        // Mã số thuế, không bắt buộc
        public string? TaxId { get; set; }

        // Mã chi nhánh, không bắt buộc
        public string? BranchCode { get; set; }

        // Link Facebook, không bắt buộc
        public string? LinkSocialFacebook { get; set; }

        // Link Twitter, không bắt buộc
        public string? LinkSocialTwitter { get; set; }

        // Link Instagram, không bắt buộc
        public string? LinkSocialInstagram { get; set; }

        // Link TikTok, không bắt buộc
        public string? LinkSocialTiktok { get; set; }

        // Link YouTube, không bắt buộc
        public string? LinkSocialYoutube { get; set; }
    }

    // ViewModel để cập nhật thông tin cửa hàng
    public class StoreInformationUpdateVModel : StoreInformationCreateVModel
    {
        // Id của thông tin cửa hàng
        public int Id { get; set; }

        // Trạng thái hoạt động
        public bool? IsActive { get; set; }
    }

    // ViewModel để lấy thông tin cửa hàng
    public class StoreInformationGetVModel : StoreInformationUpdateVModel
    {
        // URL logo của cửa hàng
        public string? Logo { get; set; }

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