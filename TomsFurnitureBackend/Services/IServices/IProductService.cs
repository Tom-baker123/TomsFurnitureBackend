using Microsoft.AspNetCore.Mvc;
using OA.Domain.Common.Models;
using TomsFurnitureBackend.Common.Models;
using TomsFurnitureBackend.VModels;
using static TomsFurnitureBackend.VModels.ProductVModel;

namespace TomsFurnitureBackend.Services.IServices
{
    public interface IProductService
    {
        Task<PaginationModel<ProductGetVModel>> GetAllAsync(ProductFilterParams param); // Lấy tất cả sản phẩm
        Task<ProductGetVModel?> GetByIdAsync(int id); // Lấy sản phẩm theo ID
        Task<ProductVariantGetVModel?> GetVariantByIdAsync(int variantId); // Lấy biến thể sản phẩm theo ID
        Task<ResponseResult> CreateAsync(ProductCreateVModel model); // Tạo sản phẩm mới
        Task<ResponseResult> UpdateAsync(ProductUpdateVModel model); // Cập nhật sản phẩm
        Task<ResponseResult> DeleteAsync(int id); // Xóa sản phẩm
        Task<ResponseResult> UpdateVariantAsync(ProductVariantUpdateVModel model); // Cập nhật biến thể
        Task<ResponseResult> DeleteVariantAsync(int variantId); // Xóa biến thể sản phẩm theo ID
    }
}
