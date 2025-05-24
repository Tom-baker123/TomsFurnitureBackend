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

namespace TomsFurnitureBackend.Services
{
    public class SliderService : ISliderService
    {
        private readonly TomfurnitureContext _context;

        public SliderService(TomfurnitureContext context)
        {
            _context = context;
        }
        public async Task<ResponseResult> Create(SliderCreateVModel model, string imageUrl)
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
            catch (Exception ex) {
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

        public async Task<SliderGetVModel?> GetByIdAsync(int id)
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

        public Task<ResponseResult> Update(int id, SliderUpdateVModel model)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseResult> Update(SliderUpdateVModel model)
        {
            throw new NotImplementedException();
        }
    }
}
