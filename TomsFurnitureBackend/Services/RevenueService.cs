using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TomsFurnitureBackend.Mappings;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Services
{
    public class RevenueService : IRevenueService
    {
        private readonly TomfurnitureContext _context;

        public RevenueService(TomfurnitureContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<RevenueResponseVModel> GetRevenueAsync(RevenueRequestVModel request)
        {
            try
            {
                // Validate input
                if (request.StartDate > request.EndDate)
                    throw new ArgumentException("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc.");
                if (!new[] { "day", "week", "month", "year" }.Contains(request.TimeUnit.ToLower()))
                    throw new ArgumentException("Đơn vị thời gian không hợp lệ. Phải là 'day', 'week', 'month' hoặc 'year'.");

                // Chuẩn hóa thời gian (bỏ giờ phút giây)
                var startDate = request.StartDate.Date;
                var endDate = request.EndDate.Date.AddDays(1).AddTicks(-1); // Bao gồm cả ngày cuối

                // Truy vấn đơn hàng đã thanh toán
                var query = _context.Orders
                    .Where(o => o.IsPaid && o.OrderDate >= startDate && o.OrderDate <= endDate)
                    .Select(o => new
                    {
                        o.OrderDate,
                        o.Total,
                        o.PriceDiscount
                    });

                // Nhóm theo đơn vị thời gian
                IQueryable<object> groupedQuery;

                switch (request.TimeUnit.ToLower())
                {
                    case "day":
                        groupedQuery = query
                            .GroupBy(o => o.OrderDate.Date)
                            .Select(g => new
                            {
                                TimeLabel = g.Key.ToString("yyyy-MM-dd"),
                                GrossRevenue = g.Sum(o => o.Total ?? 0),
                                NetRevenue = g.Sum(o => (o.Total ?? 0) - o.PriceDiscount),
                                DiscountAmount = g.Sum(o => o.PriceDiscount),
                                PaidOrderCount = g.Count()
                            });
                        break;

                    case "week":
                        groupedQuery = query
                            .GroupBy(o => new
                            {
                                Year = o.OrderDate.Year,
                                Week = o.OrderDate.DayOfYear / 7 + 1 // Tính tuần gần đúng
                            })
                            .Select(g => new
                            {
                                TimeLabel = $"{g.Key.Year}-W{g.Key.Week:D2}",
                                GrossRevenue = g.Sum(o => o.Total ?? 0),
                                NetRevenue = g.Sum(o => (o.Total ?? 0) - o.PriceDiscount),
                                DiscountAmount = g.Sum(o => o.PriceDiscount),
                                PaidOrderCount = g.Count()
                            });
                        break;

                    case "month":
                        groupedQuery = query
                            .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                            .Select(g => new
                            {
                                TimeLabel = $"{g.Key.Year}-{g.Key.Month:D2}",
                                GrossRevenue = g.Sum(o => o.Total ?? 0),
                                NetRevenue = g.Sum(o => (o.Total ?? 0) - o.PriceDiscount),
                                DiscountAmount = g.Sum(o => o.PriceDiscount),
                                PaidOrderCount = g.Count()
                            });
                        break;

                    case "year":
                        groupedQuery = query
                            .GroupBy(o => o.OrderDate.Year)
                            .Select(g => new
                            {
                                TimeLabel = g.Key.ToString(),
                                GrossRevenue = g.Sum(o => o.Total ?? 0),
                                NetRevenue = g.Sum(o => (o.Total ?? 0) - o.PriceDiscount),
                                DiscountAmount = g.Sum(o => o.PriceDiscount),
                                PaidOrderCount = g.Count()
                            });
                        break;

                    default:
                        throw new ArgumentException("Đơn vị thời gian không được hỗ trợ.");
                }

                // Lấy dữ liệu từ database và sắp xếp trên client-side
                var dataPoints = (await groupedQuery.ToListAsync())
                    .OrderBy(x => x.GetType().GetProperty("TimeLabel").GetValue(x).ToString())
                    .ToList();

                // Tính tổng và ánh xạ sang RevenueDataPointVModel
                var response = new RevenueResponseVModel
                {
                    TotalGrossRevenue = dataPoints.Sum(x => (decimal)x.GetType().GetProperty("GrossRevenue").GetValue(x)),
                    TotalNetRevenue = dataPoints.Sum(x => (decimal)x.GetType().GetProperty("NetRevenue").GetValue(x)),
                    TotalDiscountAmount = dataPoints.Sum(x => (decimal)x.GetType().GetProperty("DiscountAmount").GetValue(x)),
                    TotalPaidOrderCount = dataPoints.Sum(x => (int)x.GetType().GetProperty("PaidOrderCount").GetValue(x)),
                    DataPoints = dataPoints.Select(x => new RevenueDataPointVModel
                    {
                        TimeLabel = (string)x.GetType().GetProperty("TimeLabel").GetValue(x),
                        GrossRevenue = (decimal)x.GetType().GetProperty("GrossRevenue").GetValue(x),
                        NetRevenue = (decimal)x.GetType().GetProperty("NetRevenue").GetValue(x),
                        DiscountAmount = (decimal)x.GetType().GetProperty("DiscountAmount").GetValue(x),
                        PaidOrderCount = (int)x.GetType().GetProperty("PaidOrderCount").GetValue(x)
                    }).ToList()
                };

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thống kê doanh thu: {ex.Message}", ex);
            }
        }
    }
}