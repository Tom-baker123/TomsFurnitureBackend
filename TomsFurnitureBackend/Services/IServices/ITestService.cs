using OA.Domain.Common.Models;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services.IServices
{
    public interface ITestService
    {
        Task<List<TestGetVModel>> GetAllTestAsync();
        Task<TestGetVModel?> GetTestByIdAsync(int id);
        Task<ResponseResult> CreateTestAsync(TestCreateVModel model);
        Task<ResponseResult> UpdateTestAsync(int id, TestUpdateVModel model);
        Task<ResponseResult> DeleteTestAsync(int id);
    }
}
