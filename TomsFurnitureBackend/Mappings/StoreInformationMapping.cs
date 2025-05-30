using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Extensions
{
    public static class StoreInformationExtensions
    {
        // Chuyển từ StoreInformationCreateVModel sang Entity StoreInformation
        public static StoreInformation ToEntity(this StoreInformationCreateVModel model, string? logoUrl)
        {
            // Tạo mới StoreInformation entity với các giá trị từ ViewModel
            return new StoreInformation
            {
                StoreName = model.StoreName,
                StoreAddress = model.StoreAddress,
                Logo = logoUrl,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                LinkWebsite = model.LinkWebsite,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                OwnerName = model.OwnerName,
                BusinessType = model.BusinessType,
                OperatingHours = model.OperatingHours,
                StoreDescription = model.StoreDescription,
                EstablishmentDate = model.EstablishmentDate,
                TaxId = model.TaxId,
                BranchCode = model.BranchCode,
                LinkSocialFacebook = model.LinkSocialFacebook,
                LinkSocialTwitter = model.LinkSocialTwitter,
                LinkSocialInstagram = model.LinkSocialInstagram,
                LinkSocialTiktok = model.LinkSocialTiktok,
                LinkSocialYoutube = model.LinkSocialYoutube,
                IsActive = true, // Mặc định là true khi tạo mới
                CreatedDate = DateTime.UtcNow // Sử dụng UTC để nhất quán
            };
        }

        // Cập nhật thông tin Entity StoreInformation từ StoreInformationUpdateVModel
        public static void UpdateEntity(this StoreInformation entity, StoreInformationUpdateVModel model, string? logoUrl)
        {
            // Cập nhật các thuộc tính của entity
            entity.StoreName = model.StoreName;
            entity.StoreAddress = model.StoreAddress;
            entity.Logo = logoUrl ?? entity.Logo; // Giữ nguyên Logo nếu không có giá trị mới
            entity.PhoneNumber = model.PhoneNumber;
            entity.Email = model.Email;
            entity.LinkWebsite = model.LinkWebsite;
            entity.Latitude = model.Latitude;
            entity.Longitude = model.Longitude;
            entity.OwnerName = model.OwnerName;
            entity.BusinessType = model.BusinessType;
            entity.OperatingHours = model.OperatingHours;
            entity.StoreDescription = model.StoreDescription;
            entity.EstablishmentDate = model.EstablishmentDate;
            entity.TaxId = model.TaxId;
            entity.BranchCode = model.BranchCode;
            entity.LinkSocialFacebook = model.LinkSocialFacebook;
            entity.LinkSocialTwitter = model.LinkSocialTwitter;
            entity.LinkSocialInstagram = model.LinkSocialInstagram;
            entity.LinkSocialTiktok = model.LinkSocialTiktok;
            entity.LinkSocialYoutube = model.LinkSocialYoutube;
            entity.IsActive = model.IsActive ?? entity.IsActive; // Giữ nguyên nếu không có giá trị mới
            entity.UpdatedDate = DateTime.UtcNow; // Cập nhật thời gian sửa đổi
        }

        // Chuyển từ Entity StoreInformation sang StoreInformationGetVModel
        public static StoreInformationGetVModel ToGetVModel(this StoreInformation entity)
        {
            return new StoreInformationGetVModel
            {
                Id = entity.Id,
                StoreName = entity.StoreName,
                StoreAddress = entity.StoreAddress,
                Logo = entity.Logo,
                PhoneNumber = entity.PhoneNumber,
                Email = entity.Email,
                LinkWebsite = entity.LinkWebsite,
                Latitude = entity.Latitude,
                Longitude = entity.Longitude,
                OwnerName = entity.OwnerName,
                BusinessType = entity.BusinessType,
                OperatingHours = entity.OperatingHours,
                StoreDescription = entity.StoreDescription,
                EstablishmentDate = entity.EstablishmentDate,
                TaxId = entity.TaxId,
                BranchCode = entity.BranchCode,
                LinkSocialFacebook = entity.LinkSocialFacebook,
                LinkSocialTwitter = entity.LinkSocialTwitter,
                LinkSocialInstagram = entity.LinkSocialInstagram,
                LinkSocialTiktok = entity.LinkSocialTiktok,
                LinkSocialYoutube = entity.LinkSocialYoutube,
                IsActive = entity.IsActive,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate,
                CreatedBy = entity.CreatedBy,
                UpdatedBy = entity.UpdatedBy
            };
        }
    }
}