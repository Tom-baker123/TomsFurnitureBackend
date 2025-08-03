using System.Threading.Tasks;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services.IServices
{
    public interface IRevenueService
    {
        // Thống kê doanh thu theo khoảng thời gian
        Task<RevenueResponseVModel> GetRevenueAsync(RevenueRequestVModel request);
    }
}