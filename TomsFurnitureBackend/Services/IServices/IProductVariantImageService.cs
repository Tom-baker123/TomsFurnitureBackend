using System;
using System.Collections.Generic;
using TomsFurnitureBackend.VModels;
using static TomsFurnitureBackend.VModels.ProductVModel;
using OA.Domain.Common.Models;
using System.Threading.Tasks;

namespace TomsFurnitureBackend.Services.Interfaces
{
    public interface IProductVariantImageService
    {
        Task<List<ProductVariantImageGetVModel>> GetAllAsync();
        Task<ProductVariantImageGetVModel?> GetByIdAsync(int id);
        Task<ResponseResult> CreateAsync(ProductVariantImageCreateVModel model, string imageUrl);
        Task<ResponseResult> UpdateAsync(ProductVariantImageUpdateVModel model, string? imageUrl = null);
        Task<ResponseResult> DeleteAsync(int id);
    }
}
