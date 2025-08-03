using System;
using System.Collections.Generic;

namespace TomsFurnitureBackend.VModels
{
    // Yêu cầu thống kê doanh thu
    public class RevenueRequestVModel
    {
        public DateTime StartDate { get; set; } // Ngày bắt đầu
        public DateTime EndDate { get; set; }   // Ngày kết thúc
        public string TimeUnit { get; set; } = null!; // Đơn vị thời gian: "day", "week", "month", "year"
    }

    // Dữ liệu doanh thu cho từng khoảng thời gian
    public class RevenueDataPointVModel
    {
        public string TimeLabel { get; set; } = null!; // Nhãn thời gian (VD: "2025-08-01")
        public decimal GrossRevenue { get; set; }      // Tổng doanh thu (Total)
        public decimal NetRevenue { get; set; }        // Doanh thu ròng (Total - PriceDiscount)
        public decimal DiscountAmount { get; set; }    // Số tiền giảm giá
        public int PaidOrderCount { get; set; }        // Số đơn hàng đã thanh toán
    }

    // Phản hồi thống kê doanh thu
    public class RevenueResponseVModel
    {
        public decimal TotalGrossRevenue { get; set; }    // Tổng doanh thu
        public decimal TotalNetRevenue { get; set; }      // Tổng doanh thu ròng
        public decimal TotalDiscountAmount { get; set; }  // Tổng tiền giảm giá
        public int TotalPaidOrderCount { get; set; }      // Tổng số đơn hàng đã thanh toán
        public List<RevenueDataPointVModel> DataPoints { get; set; } = new List<RevenueDataPointVModel>(); // Dữ liệu theo thời gian
    }
}