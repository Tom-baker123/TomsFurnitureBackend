using OA.Domain.Common.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services.IServices
{
    public interface IProductVariantService
    {
        Task<ResponseResult> Create(ProductVariantCreateVModel vModel);
    }
}
