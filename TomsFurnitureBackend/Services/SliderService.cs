using Microsoft.EntityFrameworkCore;
using TomsFurnitureBackend.Extensions;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.Interfaces;
using TomsFurnitureBackend.VModels;
using OA.Domain.Common.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using TomsFurnitureBackend.Helpers; // Thêm namespace cho SliderHelper

namespace TomsFurnitureBackend.Services
{
    public class SliderService : ISliderService
    {
        private readonly TomfurnitureContext _context;

        public SliderService(TomfurnitureContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ResponseResult> CreateAsync(SliderCreateVModel model, string imageUrl)
        {
            var response = new ResponseResult();
            try
            {
                // Bước 1: Tạo entity từ model
                var slider = model.ToEntity();

                // Bước 2: Set đường dẫn ảnh
                slider.ImageUrl = imageUrl;

                // Bước 3: Tính toán DisplayOrder cho Slider mới
                // Nếu DisplayOrder không được chỉ định hoặc không hợp lệ, đặt nó thành số lớn nhất + 1
                int sliderCount = await _context.Sliders
                    .Where(s => s.ProductId == slider.ProductId)
                    .CountAsync();
                if (slider.DisplayOrder <= 0 || slider.DisplayOrder > sliderCount + 1)
                {
                    slider.DisplayOrder = sliderCount + 1; // Đặt ở cuối danh sách
                }

                // Bước 4: Điều chỉnh DisplayOrder của các Slider hiện có
                await SliderHelper.AdjustDisplayOrder(_context, slider.ProductId, slider.DisplayOrder);

                // Bước 5: Thêm Slider mới vào DbContext
                _context.Sliders.Add(slider);
                await _context.SaveChangesAsync();

                // Bước 6: Chuyển đổi sang ViewModel để trả về
                var sliderVm = slider.ToGetVModel();

                response = new SuccessResponseResult(sliderVm, "Slider created successfully");
                return response;
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult("Failed to create slider: " + ex.Message);
            }
        }

        public async Task<ResponseResult> DeleteAsync(int id)
        {
            try
            {
                // Bước 1: Tìm Slider theo ID
                var slider = await _context.Sliders.FirstOrDefaultAsync(s => s.Id == id);
                if (slider == null)
                {
                    return new ErrorResponseResult("Slider not found");
                }

                // Bước 2: Lưu ProductId để điều chỉnh DisplayOrder
                var productId = slider.ProductId;

                // Bước 3: Xóa Slider
                _context.Sliders.Remove(slider);
                await _context.SaveChangesAsync();

                // Bước 4: Điều chỉnh DisplayOrder của các Slider còn lại bằng SliderHelper
                await SliderHelper.AdjustDisplayOrder(_context, productId, 0);

                return new SuccessResponseResult(null, "Slider deleted successfully");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult("Failed to delete slider: " + ex.Message);
            }
        }

        public async Task<List<SliderGetVModel>> GetAllAsync()
        {
            // Bước 1: Lấy tất cả Slider từ database
            var sliders = await _context.Sliders
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync();

            // Bước 2: Chuyển đổi từ model sang VModel
            var result = sliders.Select(x => SliderExtensions.ToGetVModel(x)).ToList();
            return result;
        }

        public async Task<SliderGetVModel>? GetByIdAsync(int id)
        {
            // Bước 1: Tìm Slider theo ID
            var slider = await _context.Sliders
                .FirstOrDefaultAsync(s => s.Id == id);

            if (slider == null)
            {
                return null;
            }

            // Bước 2: Chuyển đổi sang ViewModel
            var viewModel = SliderExtensions.ToGetVModel(slider);
            return viewModel;
        }

        public async Task<ResponseResult> UpdateAsync(SliderUpdateVModel model, string? imageUrl = null)
        {
            try
            {
                // Bước 1: Tìm Slider theo ID
                var slider = await _context.Sliders
                    .FirstOrDefaultAsync(t => t.Id == model.Id);
                if (slider == null)
                {
                    return new ErrorResponseResult("Slider not found");
                }

                // Bước 2: Lưu ProductId cũ để kiểm tra thay đổi
                var oldProductId = slider.ProductId;

                // Bước 3: Cập nhật thông tin từ model
                slider.UpdateEntity(model);

                // Bước 4: Cập nhật ImageUrl nếu có
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    slider.ImageUrl = imageUrl;
                }

                // Bước 5: Điều chỉnh DisplayOrder bằng SliderHelper nếu ProductId thay đổi hoặc DisplayOrder thay đổi
                if (oldProductId != model.ProductId)
                {
                    await SliderHelper.AdjustDisplayOrder(_context, oldProductId, 0); // Điều chỉnh lại ProductId cũ
                    await SliderHelper.AdjustDisplayOrder(_context, model.ProductId, model.DisplayOrder, model.Id); // Điều chỉnh ProductId mới
                }
                else
                {
                    await SliderHelper.AdjustDisplayOrder(_context, model.ProductId, model.DisplayOrder, model.Id);
                }

                // Bước 6: Lưu thay đổi vào DB
                await _context.SaveChangesAsync();

                // Bước 7: Chuyển đổi sang VModel để trả về
                var sliderVM = slider.ToGetVModel();
                return new SuccessResponseResult(sliderVM, "Slider updated successfully");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult("Failed to update slider: " + ex.Message);
            }
        }
    }
}