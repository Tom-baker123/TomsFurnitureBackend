using Microsoft.EntityFrameworkCore;
using TomsFurnitureBackend.Models;
using System.Linq;
using System.Threading.Tasks;

namespace TomsFurnitureBackend.Helpers
{
    public static class ProductVariantImageHelper
    {
        // Hàm điều chỉnh DisplayOrder cho các ProductVariantImage có cùng ProVarId
        // Nếu DisplayOrder mới trùng với ảnh khác, sẽ hoán đổi DisplayOrder giữa hai ảnh
        public static async Task AdjustDisplayOrder(TomfurnitureContext context, int? proVarId, int newDisplayOrder, int? currentImageId = null, int? oldDisplayOrder = null)
        {
            if (proVarId == null || newDisplayOrder <= 0)
                return;

            // Lấy ảnh hiện tại (nếu có)
            ProductVariantImage? currentImage = null;
            if (currentImageId.HasValue)
            {
                currentImage = await context.ProductVariantImages.FirstOrDefaultAsync(x => x.Id == currentImageId.Value);
            }

            // Tìm ảnh khác cùng ProVarId có DisplayOrder = newDisplayOrder
            var swapImage = await context.ProductVariantImages
                .FirstOrDefaultAsync(x => x.ProVarId == proVarId && x.DisplayOrder == newDisplayOrder && (!currentImageId.HasValue || x.Id != currentImageId.Value));

            if (swapImage != null && currentImage != null && oldDisplayOrder.HasValue)
            {
                // Hoán đổi đúng nghĩa: currentImage lấy displayOrder mới, swapImage lấy displayOrder cũ của currentImage
                swapImage.DisplayOrder = oldDisplayOrder;
                currentImage.DisplayOrder = newDisplayOrder;
                await context.SaveChangesAsync();
                return;
            }

            // Nếu không có ảnh để swap, thực hiện logic sắp xếp lại như cũ
            var images = await context.ProductVariantImages
                .Where(img => img.ProVarId == proVarId && (!currentImageId.HasValue || img.Id != currentImageId.Value))
                .OrderBy(img => img.DisplayOrder)
                .ToListAsync();

            if (newDisplayOrder > images.Count + 1)
            {
                newDisplayOrder = images.Count + 1;
            }

            int currentOrder = 1;
            foreach (var img in images)
            {
                if (currentOrder == newDisplayOrder)
                {
                    currentOrder++;
                }
                img.DisplayOrder = currentOrder++;
            }

            await context.SaveChangesAsync();
        }
    }
}
