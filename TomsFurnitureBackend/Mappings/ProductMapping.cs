using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.VModels;
using static TomsFurnitureBackend.VModels.ProductVModel;

namespace TomsFurnitureBackend.Extensions
{
    public static class ProductMapping
    {
        // Chuyển từ ProductCreateVModel sang Entity Product
        public static Product ToEntity(this ProductCreateVModel model)
        {
            var product = new Product
            {
                ProductName = model.ProductName,
                SpecificationDescription = model.SpecificationDescription,
                BrandId = model.BrandId,
                CategoryId = model.CategoryId,
                CountriesId = model.CountriesId,
                SupplierId = model.SupplierId,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                ViewCount = 0,
                ProductVariants = model.ProductVariants.Select(pv => pv.ToEntity()).ToList()
            };
            return product;
        }

        // Chuyển từ ProductVariantCreateVModel sang Entity ProductVariant
        public static ProductVariant ToEntity(this ProductVariantCreateVModel model)
        {
            return new ProductVariant
            {
                OriginalPrice = model.OriginalPrice,
                DiscountedPrice = model.DiscountedPrice,
                StockQty = model.StockQty,
                ColorId = model.ColorId,
                SizeId = model.SizeId,
                MaterialId = model.MaterialId,
                UnitId = model.UnitId,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };
        }

        // Chuyển từ ProductVariantUpdateVModel sang Entity ProductVariant (dùng cho cả tạo mới và cập nhật)
        public static ProductVariant ToEntity(this ProductVariantUpdateVModel model)
        {
            return new ProductVariant
            {
                Id = model.Id,
                OriginalPrice = model.OriginalPrice,
                DiscountedPrice = model.DiscountedPrice,
                StockQty = model.StockQty,
                ColorId = model.ColorId,
                SizeId = model.SizeId,
                MaterialId = model.MaterialId,
                UnitId = model.UnitId,
                IsActive = model.IsActive ?? true,
                CreatedDate = model.Id == 0 ? DateTime.UtcNow : null, // Chỉ set CreatedDate cho biến thể mới
                UpdatedDate = model.Id > 0 ? DateTime.UtcNow : null // Set UpdatedDate khi cập nhật
            };
        }

        // Cập nhật Entity Product từ ProductUpdateVModel, bao gồm xử lý biến thể
        public static void UpdateEntity(this Product entity, ProductUpdateVModel model, TomfurnitureContext context)
        {
            // Cập nhật thông tin sản phẩm chính
            entity.ProductName = model.ProductName;
            entity.SpecificationDescription = model.SpecificationDescription;
            entity.BrandId = model.BrandId;
            entity.CategoryId = model.CategoryId;
            entity.CountriesId = model.CountriesId;
            entity.SupplierId = model.SupplierId;
            entity.IsActive = model.IsActive ?? entity.IsActive;
            entity.UpdatedDate = DateTime.UtcNow;

            // Xử lý các biến thể
            var existingVariantIds = entity.ProductVariants.Select(pv => pv.Id).ToList();
            var updatedVariantIds = model.ProductVariants.Where(v => v.Id > 0).Select(v => v.Id).ToList();

            //// Xóa các biến thể không còn trong danh sách cập nhật
            //var variantsToRemove = entity.ProductVariants.Where(pv => !updatedVariantIds.Contains(pv.Id)).ToList();
            //foreach (var variant in variantsToRemove)
            //{
            //    context.ProductVariants.Remove(variant);
            //}

            // Thêm hoặc cập nhật biến thể
            foreach (var variantModel in model.ProductVariants)
            {
                if (variantModel.Id == 0)
                {
                    // Thêm biến thể mới
                    var newVariant = variantModel.ToEntity();
                    newVariant.ProductId = entity.Id;
                    entity.ProductVariants.Add(newVariant);
                }
                else
                {
                    // Cập nhật biến thể hiện có
                    var existingVariant = entity.ProductVariants.FirstOrDefault(pv => pv.Id == variantModel.Id);
                    if (existingVariant != null)
                    {
                        existingVariant.UpdateVariantEntity(variantModel);
                    }
                }
            }
        }

        // Chuyển từ Entity Product sang ProductGetVModel
        public static ProductGetVModel ToGetVModel(this Product entity)
        {
            return new ProductGetVModel
            {
                Id = entity.Id,
                ProductName = entity.ProductName,
                SpecificationDescription = entity.SpecificationDescription,
                BrandId = entity.BrandId,
                CategoryId = entity.CategoryId,
                CountriesId = entity.CountriesId,
                SupplierId = entity.SupplierId,
                IsActive = entity.IsActive,
                ViewCount = entity.ViewCount,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate,
                CreatedBy = entity.CreatedBy,
                UpdatedBy = entity.UpdatedBy,
                BrandName = entity.Brand?.BrandName,
                CategoryName = entity.Category?.CategoryName,
                CountryName = entity.Countries?.CountryName,
                SupplierName = entity.Supplier?.SupplierName,
                ProductVariants = entity.ProductVariants?.Select(pv => new ProductVModel.ProductVariantGetVModel
                {
                    Id = pv.Id,
                    OriginalPrice = pv.OriginalPrice,
                    DiscountedPrice = pv.DiscountedPrice,
                    StockQty = pv.StockQty,
                    ColorId = pv.ColorId,
                    ColorName = pv.Color?.ColorName,
                    ColorCode = pv.Color?.ColorCode,
                    SizeId = pv.SizeId,
                    SizeName = pv.Size?.SizeName,
                    MaterialId = pv.MaterialId,
                    MaterialName = pv.Material?.MaterialName,
                    UnitId = pv.UnitId,
                    UnitName = pv.Unit?.UnitName,
                    Images = pv.ProductVariantImages?.Select(img => new ProductVModel.ProductVariantImageGetVModel
                    {
                        Id = img.Id,
                        ImageUrl = img.ImageUrl,
                        Attribute = img.Attribute,
                        DisplayOrder = img.DisplayOrder,
                        IsActive = img.IsActive,
                        CreatedDate = img.CreatedDate,
                        UpdatedDate = img.UpdatedDate,
                        CreatedBy = img.CreatedBy,
                        UpdatedBy = img.UpdatedBy,
                        ProVarId = img.ProVarId
                    }).ToList() ?? new List<ProductVModel.ProductVariantImageGetVModel>()
                }).ToList() ?? new List<ProductVModel.ProductVariantGetVModel>(),
                Sliders = entity.Sliders?.Where(s => s.IsActive == true).Select(s => new SliderGetVModel
                {
                    Id = s.Id,
                    Title = s.Title,
                    Description = s.Description,
                    ImageUrl = s.ImageUrl,
                    LinkUrl = s.LinkUrl,
                    IsPoster = s.IsPoster,
                    Position = s.Position,
                    DisplayOrder = s.DisplayOrder,
                    IsActive = s.IsActive,
                    CreatedDate = s.CreatedDate,
                    UpdatedDate = s.UpdatedDate,
                    CreatedBy = s.CreatedBy,
                    UpdatedBy = s.UpdatedBy,
                    ProductId = s.ProductId
                }).ToList() ?? new List<SliderGetVModel>()
            };
        }

        // Ánh xạ biến thể
        public static void UpdateVariantEntity(this ProductVariant entity, ProductVModel.ProductVariantUpdateVModel model)
        {
            entity.OriginalPrice = model.OriginalPrice;
            entity.DiscountedPrice = model.DiscountedPrice;
            entity.StockQty = model.StockQty;
            entity.ColorId = model.ColorId;
            entity.SizeId = model.SizeId;
            entity.MaterialId = model.MaterialId;
            entity.UnitId = model.UnitId;
            entity.IsActive = model.IsActive ?? entity.IsActive;
            entity.UpdatedDate = DateTime.UtcNow;
        }
    }
}