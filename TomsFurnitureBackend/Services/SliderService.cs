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
        public async Task<ResponseResult> GetByIdAsync(int id)
        {
            try
            {
                var slider = await _context.Sliders
                    .Include(s => s.Product) // nếu bạn cần include Product
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (slider == null)
                {
                    return new ErrorResponseResult("Không tìm thấy slider");
                }

                var viewModel = slider.ToGetVModel();
                return new SuccessResponseResult(viewModel, "Lấy slider thành công");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult(ex.Message);
            }
        }

    }
}
