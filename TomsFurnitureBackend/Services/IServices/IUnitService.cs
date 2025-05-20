using OA.Domain.Common.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services.IServices
{
    public interface IUnitService
    {
        Task<ResponseResult> Create(UnitCreateVModel vModel);
    }
}
