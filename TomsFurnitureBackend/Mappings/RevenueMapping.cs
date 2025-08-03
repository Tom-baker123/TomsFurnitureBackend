using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Mappings
{
    public static class RevenueExtensions
    {
        // Chuyển dữ liệu thống kê sang RevenueDataPointVModel
        public static RevenueDataPointVModel ToDataPointVModel(
            this (string TimeLabel, decimal GrossRevenue, decimal NetRevenue, decimal DiscountAmount, int PaidOrderCount) data)
        {
            return new RevenueDataPointVModel
            {
                TimeLabel = data.TimeLabel,
                GrossRevenue = data.GrossRevenue,
                NetRevenue = data.NetRevenue,
                DiscountAmount = data.DiscountAmount,
                PaidOrderCount = data.PaidOrderCount
            };
        }
    }
}