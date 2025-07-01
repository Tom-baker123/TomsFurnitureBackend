using Microsoft.EntityFrameworkCore;
using TomsFurnitureBackend.Models;

namespace TomsFurnitureBackend.Helpers
{
    public static class SliderHelper
    {
        // Hàm điều chỉnh DisplayOrder cho các Slider có cùng ProductId
        public static async Task AdjustDisplayOrder(TomfurnitureContext context, int? productId, int newDisplayOrder, int? excludeSliderId = null)
        {
            // Bước 1: Lấy danh sách Slider có cùng ProductId, ngoại trừ Slider có excludeSliderId
            var sliders = await context.Sliders
                .Where(s => s.ProductId == productId && s.Id != excludeSliderId)
                .OrderBy(s => s.DisplayOrder)
                .ToListAsync();

            // Bước 2: Kiểm tra và giới hạn newDisplayOrder
            // Nếu newDisplayOrder <= 0 hoặc lớn hơn số lượng Slider + 1, đặt thành số lượng Slider + 1
            if (newDisplayOrder <= 0 || newDisplayOrder > sliders.Count + 1)
            {
                newDisplayOrder = sliders.Count + 1; // Đặt Slider mới ở cuối danh sách
            }

            // Bước 3: Điều chỉnh DisplayOrder của các Slider hiện có
            int currentOrder = 1;
            foreach (var slider in sliders)
            {
                if (currentOrder == newDisplayOrder)
                {
                    currentOrder++; // Nhường chỗ cho newDisplayOrder
                }
                slider.DisplayOrder = currentOrder++;
            }

            // Bước 4: Lưu thay đổi vào cơ sở dữ liệu
            await context.SaveChangesAsync();
        }
    }
}