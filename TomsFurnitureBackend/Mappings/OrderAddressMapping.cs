using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Extensions
{
    public static class OrderAddressMapping
    {
        public static OrderAddress ToEntity(this OrderAddressCreateVModel model)
        {
            return new OrderAddress
            {
                Recipient = model.Recipient,
                PhoneNumber = model.PhoneNumber,
                AddressDetailRecipient = model.AddressDetailRecipient,
                City = model.City,
                CityCode = model.CityCode,
                District = model.District,
                DistrictCode = model.DistrictCode,
                Ward = model.Ward,
                WardCode = model.WardCode,
                IsDeafaultAddress = model.IsDeafaultAddress,
                UserId = model.UserId,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };
        }

        public static void UpdateEntity(this OrderAddress entity, OrderAddressUpdateVModel model)
        {
            entity.Recipient = model.Recipient;
            entity.PhoneNumber = model.PhoneNumber;
            entity.AddressDetailRecipient = model.AddressDetailRecipient;
            entity.City = model.City;
            entity.CityCode = model.CityCode;
            entity.District = model.District;
            entity.DistrictCode = model.DistrictCode;
            entity.Ward = model.Ward;
            entity.WardCode = model.WardCode;
            entity.IsDeafaultAddress = model.IsDeafaultAddress;
            entity.IsActive = model.IsActive ?? entity.IsActive;
            entity.UpdatedDate = DateTime.UtcNow;
        }

        public static OrderAddressGetVModel ToGetVModel(this OrderAddress entity)
        {
            return new OrderAddressGetVModel
            {
                Id = entity.Id,
                Recipient = entity.Recipient,
                PhoneNumber = entity.PhoneNumber,
                AddressDetailRecipient = entity.AddressDetailRecipient,
                City = entity.City,
                CityCode = entity.CityCode,
                District = entity.District,
                DistrictCode = entity.DistrictCode,
                Ward = entity.Ward,
                WardCode = entity.WardCode,
                IsDeafaultAddress = entity.IsDeafaultAddress,
                IsActive = entity.IsActive,
                UserId = entity.UserId,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate,
                CreatedBy = entity.CreatedBy,
                UpdatedBy = entity.UpdatedBy
            };
        }
    }
}
