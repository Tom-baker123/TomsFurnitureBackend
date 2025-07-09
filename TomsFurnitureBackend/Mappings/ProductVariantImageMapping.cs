using System;
using System.Collections.Generic;
using TomsFurnitureBackend.VModels;
using static TomsFurnitureBackend.VModels.ProductVModel;
using TomsFurnitureBackend.Models;

namespace TomsFurnitureBackend.Extensions
{
    public static class ProductVariantImageExtensions
    {
        public static ProductVariantImage ToEntity(this ProductVariantImageCreateVModel model)
        {
            return new ProductVariantImage
            {
                Attribute = model.Attribute,
                DisplayOrder = model.DisplayOrder,
                ProVarId = model.ProVarId,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };
        }

        public static void UpdateEntity(this ProductVariantImage entity, ProductVariantImageUpdateVModel model)
        {
            entity.Attribute = model.Attribute;
            entity.DisplayOrder = model.DisplayOrder;
            entity.ProVarId = model.ProVarId;
            entity.IsActive = model.IsActive;
            entity.UpdatedDate = DateTime.UtcNow;
        }

        public static ProductVariantImageGetVModel ToGetVModel(this ProductVariantImage entity)
        {
            return new ProductVariantImageGetVModel
            {
                Id = entity.Id,
                ImageUrl = entity.ImageUrl,
                Attribute = entity.Attribute,
                DisplayOrder = entity.DisplayOrder,
                IsActive = entity.IsActive,
                ProVarId = entity.ProVarId,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate,
                CreatedBy = entity.CreatedBy,
                UpdatedBy = entity.UpdatedBy
            };
        }
    }
}
