using Microsoft.EntityFrameworkCore;
using TomsFurnitureBackend.Extensions;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.Interfaces;
using TomsFurnitureBackend.VModels;
using Microsoft.AspNetCore.Http;
using System;
using OA.Domain.Common.Models;
using Azure;
using TomsFurnitureBackend.Mappings;
using System.Linq;

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

                // Bước 3: Thêm vào DbContext
                _context.Sliders.Add(slider);
                await _context.SaveChangesAsync();

                // Bước 4: Convert sang ViewModel để trả về
                var sliderVm = slider.ToGetVModel();

                response = new SuccessResponseResult(sliderVm, "Thêm slider thành công");
                return response;
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult(ex.Message);
            }
        }

        public async Task<ResponseResult> DeleteAsync(int id)
        {
            try
            {
                // Tìm Slider theo ID
                var slider = await _context.Sliders.FirstOrDefaultAsync(s => s.Id == id);
                if (slider != null)
                {
                    _context.Sliders.Remove(slider);
                    await _context.SaveChangesAsync();
                }
                return new SuccessResponseResult(slider, "You have deleted your slider!");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"You have an error when deleted Slider: {ex.Message}");
            }
        }

        public async Task<List<SliderGetVModel>> GetAllAsync()
        {

            // Lấy tất cả Slider từ database
            var sliders = await _context.Sliders
                .OrderBy(x => x.DisplayOrder) // Sắp xếp sản phẩm theo Display Order
                .ToListAsync();

            // Chuyển đổi từ model sang VModel
            var result = sliders.Select(x => SliderExtensions.ToGetVModel(x)).ToList();
            return result;
        }

        public async Task<SliderGetVModel>? GetByIdAsync(int id)
        {
            var slider = await _context.Sliders
                //.Include(s => s.Product) // nếu bạn cần include Product
                .FirstOrDefaultAsync(s => s.Id == id);

            if (slider == null)
            {
                return null;
            }

            var viewModel = SliderExtensions.ToGetVModel(slider);
            return viewModel;
        }

        public async Task<ResponseResult> UpdateAsync(SliderUpdateVModel model, string? imageUrl = null)
        {
            try
            {
                // B1: Tìm Slider theo ID
                var slider = await _context.Sliders
                    .FirstOrDefaultAsync(t => t.Id == model.Id);
                // + Kiểm tra slider có rỗng không ?

                if (slider == null) return new ErrorResponseResult($"Not found ID: {slider.Id}");

                // B2: Cập nhật info từ model
                slider.UpdateEntity(model);

                // B3: Cập nhật ImageUrl nếu có tham số truyền vào
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    slider.ImageUrl = imageUrl;
                }

                // B4: Lưu thay đổi vào DB.
                _context.SaveChanges();
                await _context.SaveChangesAsync();

                // B5: Chuyển đổi sang VModel để trả về thông báo cho SuccessResponseResult. 
                var sliderVM = slider.ToGetVModel();
                return new SuccessResponseResult(sliderVM, "Cập nhật slider thành công");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"You have an error when updated Slider: {ex.Message}");
            }
        }
    }
}
