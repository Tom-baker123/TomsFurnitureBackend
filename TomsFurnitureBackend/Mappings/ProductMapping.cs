using System.Linq;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.VModels;
using static TomsFurnitureBackend.VModels.ProductVModel;
using TomsFurnitureBackend.Mappings;

namespace TomsFurnitureBackend.Extensions
{
    public static class ProductMapping
    {
        // Chuyển từ ProductCreateVModel sang Entity Product
        public static Product ToEntity(this ProductCreateVModel model, string? slug = null)
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
                ProductVariants = model.ProductVariants.Select(pv => pv.ToEntity()).ToList(),
                Slug = slug
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
                ColorId = (model.ColorId.HasValue && model.ColorId.Value > 0) ? model.ColorId : null,
                SizeId = (model.SizeId.HasValue && model.SizeId.Value > 0) ? model.SizeId : null,
                MaterialId = (model.MaterialId.HasValue && model.MaterialId.Value > 0) ? model.MaterialId : null,
                UnitId = (model.UnitId.HasValue && model.UnitId.Value > 0) ? model.UnitId : null,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };
        }

        // Chuyển từ ProductVariantUpdateVModel sang Entity ProductVariant (dùng cho cả tạo mới và cập nhật)
        public static ProductVariant ToEntity(this ProductVariantUpdateVModel model)
        {
            return new ProductVariant
            {
                Id = model.Id ?? 0,
                OriginalPrice = model.OriginalPrice,
                DiscountedPrice = model.DiscountedPrice,
                StockQty = model.StockQty,
                ColorId = (model.ColorId.HasValue && model.ColorId.Value > 0) ? model.ColorId : null,
                SizeId = (model.SizeId.HasValue && model.SizeId.Value > 0) ? model.SizeId : null,
                MaterialId = (model.MaterialId.HasValue && model.MaterialId.Value > 0) ? model.MaterialId : null,
                UnitId = (model.UnitId.HasValue && model.UnitId.Value > 0) ? model.UnitId : null,
                IsActive = model.IsActive ?? true,
                CreatedDate = (model.Id ?? 0) == 0 ? DateTime.UtcNow : null,
                UpdatedDate = (model.Id ?? 0) > 0 ? DateTime.UtcNow : null
            };
        }

        // Cập nhật Entity Product từ ProductUpdateVModel, bao gồm xử lý biến thể
        public static void UpdateEntity(this Product entity, ProductUpdateVModel model, TomfurnitureContext context, string? slug = null)
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
            if (!string.IsNullOrEmpty(slug))
                entity.Slug = slug;

            // Xử lý các biến thể
            var existingVariantIds = entity.ProductVariants.Select(pv => pv.Id).ToList();
            var updatedVariantIds = model.ProductVariants.Where(v => (v.Id ?? 0) > 0).Select(v => v.Id ?? 0).ToList();

            // Thêm hoặc cập nhật biến thể
            foreach (var variantModel in model.ProductVariants)
            {
                if ((variantModel.Id ?? 0) == 0)
                {
                    // Thêm biến thể mới
                    var newVariant = variantModel.ToEntity();
                    newVariant.ProductId = entity.Id;
                    entity.ProductVariants.Add(newVariant);
                }
                else
                {
                    // Cập nhật biến thể hiện có
                    var existingVariant = entity.ProductVariants.FirstOrDefault(pv => pv.Id == (variantModel.Id ?? 0));
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
                Slug = entity.Slug,
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
            entity.ColorId = (model.ColorId.HasValue && model.ColorId.Value > 0) ? model.ColorId : null;
            entity.SizeId = (model.SizeId.HasValue && model.SizeId.Value > 0) ? model.SizeId : null;
            entity.MaterialId = (model.MaterialId.HasValue && model.MaterialId.Value > 0) ? model.MaterialId : null;
            entity.UnitId = (model.UnitId.HasValue && model.UnitId.Value > 0) ? model.UnitId : null;
            entity.IsActive = model.IsActive ?? entity.IsActive;
            entity.UpdatedDate = DateTime.UtcNow;
        }
    }

    public static class ProductVariantMapping
    {
        public static ProductVModel.ProductVariantGetVModel ToGetVModel(this ProductVariant entity)
        {
            return new ProductVModel.ProductVariantGetVModel
            {
                Id = entity.Id,
                OriginalPrice = entity.OriginalPrice,
                DiscountedPrice = entity.DiscountedPrice,
                StockQty = entity.StockQty,
                ColorId = entity.ColorId,
                ColorName = entity.Color?.ColorName,
                ColorCode = entity.Color?.ColorCode,
                SizeId = entity.SizeId,
                SizeName = entity.Size?.SizeName,
                MaterialId = entity.MaterialId,
                MaterialName = entity.Material?.MaterialName,
                UnitId = entity.UnitId,
                UnitName = entity.Unit?.UnitName,
                Images = entity.ProductVariantImages?.Select(img => new ProductVModel.ProductVariantImageGetVModel
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
            };
        }
    }
}