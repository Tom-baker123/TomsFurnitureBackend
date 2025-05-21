using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Extensions
{
    public static class ProductVariantMapper
    {
        public static ProductVariantGetVModel ToGetVModel(this ProductVariant variant)
        {
            return new ProductVariantGetVModel
            {
                Id = variant.Id,
                OriginalPrice = variant.OriginalPrice,
                DiscountedPrice = variant.DiscountedPrice,
                StockQty = variant.StockQty,
                ImageUrl = variant.ImageUrl,
                ProductId = variant.ProductId,
                ColorId = variant.ColorId,
                SizeId = variant.SizeId,
                MaterialId = variant.MaterialId,
                UnitId = variant.UnitId,
                IsActive = variant.IsActive,
                CreatedDate = variant.CreatedDate,
                UpdatedDate = variant.UpdatedDate,
                CreatedBy = variant.CreatedBy,
                UpdatedBy = variant.UpdatedBy,

                ProductName = variant.Product?.ProductName,
                ColorName = variant.Color?.ColorName,
                SizeName = variant.Size?.SizeName,
                MaterialName = variant.Material?.MaterialName,
                UnitName = variant.Unit?.UnitName
            };
        }

        public static ProductVariant ToEntity(this ProductVariantCreateVModel vm)
        {
            return new ProductVariant
            {
                OriginalPrice = vm.OriginalPrice,
                DiscountedPrice = vm.DiscountedPrice,
                StockQty = vm.StockQty,
                ImageUrl = vm.ImageUrl,
                ProductId = vm.ProductId,
                ColorId = vm.ColorId,
                SizeId = vm.SizeId,
                MaterialId = vm.MaterialId,
                UnitId = vm.UnitId,
                IsActive = vm.IsActive,
                CreatedDate = DateTime.UtcNow
            };
        }

        public static void UpdateEntity(this ProductVariant variant, ProductVariantUpdateVModel vm)
        {
            variant.OriginalPrice = vm.OriginalPrice;
            variant.DiscountedPrice = vm.DiscountedPrice;
            variant.StockQty = vm.StockQty;
            variant.ImageUrl = vm.ImageUrl;
            variant.ProductId = vm.ProductId;
            variant.ColorId = vm.ColorId;
            variant.SizeId = vm.SizeId;
            variant.MaterialId = vm.MaterialId;
            variant.UnitId = vm.UnitId;
            variant.IsActive = vm.IsActive;
            variant.UpdatedDate = DateTime.UtcNow;
        }
    }
}
