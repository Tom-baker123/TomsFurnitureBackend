using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using OA.Domain.Common.Models;
using TomsFurnitureBackend.Mappings;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;
using TomsFurnitureBackend.Helpers;
using System.Linq;

namespace TomsFurnitureBackend.Services
{
    public class RoomTypeService : IRoomTypeService
    {
        private readonly TomfurnitureContext _context;

        public RoomTypeService(TomfurnitureContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Validation chung cho tạo mới và cập nhật loại phòng
        public static string Validate(RoomTypeCreateVModel model, bool isUpdate = false)
        {
            if (string.IsNullOrWhiteSpace(model.RoomTypeName))
            {
                return "Room type name is required.";
            }

            if (model.RoomTypeName.Length > 100)
            {
                return "Room type name must be less than 100 characters.";
            }

            if (isUpdate && model is RoomTypeUpdateVModel updateModel && updateModel.Id <= 0)
            {
                return "Invalid Room Type ID.";
            }

            return string.Empty;
        }

        // Tạo mới loại phòng
        public async Task<ResponseResult> CreateAsync(RoomTypeCreateVModel model, string imageUrl)
        {
            try
            {
                var validationResult = Validate(model);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    return new ErrorResponseResult(validationResult);
                }

                var existingRoomType = await _context.RoomTypes
                    .AnyAsync(rt => rt.RoomTypeName.ToLower() == model.RoomTypeName.ToLower());
                if (existingRoomType)
                {
                    return new ErrorResponseResult("Room type name already exists.");
                }

                var slug = await SlugHelper.GenerateUniqueSlugAsync(
                    model.RoomTypeName,
                    async (slug) => await _context.RoomTypes.AnyAsync(rt => rt.Slug == slug)
                );
                var roomType = model.ToEntity(imageUrl, slug);
                _context.RoomTypes.Add(roomType);
                await _context.SaveChangesAsync();

                // Liên kết các Category nếu có
                if (model.CategoryIds != null && model.CategoryIds.Any())
                {
                    var categories = await _context.Categories.Where(c => model.CategoryIds.Contains(c.Id)).ToListAsync();
                    foreach (var cat in categories)
                    {
                        cat.RoomTypeId = roomType.Id;
                    }
                    await _context.SaveChangesAsync();
                }

                var roomTypeVM = await _context.RoomTypes.Include(rt => rt.Categories).FirstOrDefaultAsync(rt => rt.Id == roomType.Id);
                return new SuccessResponseResult(roomTypeVM.ToGetVModel(), "Room type created successfully.");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult("An error occurred while creating the room type: " + ex.Message);
            }
        }

        // Xóa loại phòng
        public async Task<ResponseResult> DeleteAsync(int id)
        {
            try
            {
                var roomType = await _context.RoomTypes
                    .FirstOrDefaultAsync(rt => rt.Id == id);
                if (roomType == null)
                {
                    return new ErrorResponseResult("Room type not found.");
                }

                var isUsedInCategories = await _context.Categories
                    .AnyAsync(c => c.RoomTypeId == id);
                if (isUsedInCategories)
                {
                    return new ErrorResponseResult("Room type cannot be deleted because it is associated with one or more categories.");
                }

                _context.Remove(roomType);
                await _context.SaveChangesAsync();

                return new SuccessResponseResult("Room type deleted successfully.");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult("An error occurred while deleting the room type: " + ex.Message);
            }
        }

        // Lấy tất cả loại phòng
        public async Task<List<RoomTypeGetVModel>> GetAllAsync()
        {
            var roomTypes = await _context.RoomTypes.Include(rt => rt.Categories).OrderBy(rt => rt.Id).ToListAsync();
            return roomTypes.Select(rt => rt.ToGetVModel()).ToList();
        }

        // Lấy loại phòng theo ID
        public async Task<RoomTypeGetVModel?> GetByIdAsync(int id)
        {
            var roomType = await _context.RoomTypes.Include(rt => rt.Categories).FirstOrDefaultAsync(rt => rt.Id == id);
            return roomType?.ToGetVModel();
        }

        // Cập nhật loại phòng
        public async Task<ResponseResult> UpdateAsync(RoomTypeUpdateVModel model, string? imageUrl = null)
        {
            try
            {
                var validationResult = Validate(model, isUpdate: true);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    return new ErrorResponseResult(validationResult);
                }

                var roomType = await _context.RoomTypes.Include(rt => rt.Categories).FirstOrDefaultAsync(rt => rt.Id == model.Id);
                if (roomType == null)
                {
                    return new ErrorResponseResult($"Room type not found with ID: {model.Id}.");
                }

                var existingRoomType = await _context.RoomTypes
                    .AnyAsync(rt => rt.RoomTypeName.ToLower() == model.RoomTypeName.ToLower() && rt.Id != model.Id);
                if (existingRoomType)
                {
                    return new ErrorResponseResult("Room type name already exists.");
                }

                var slug = await SlugHelper.GenerateUniqueSlugAsync(
                    model.RoomTypeName,
                    async (slug) => await _context.RoomTypes.AnyAsync(rt => rt.Slug == slug && rt.Id != model.Id)
                );
                roomType.UpdateEntity(model, imageUrl, slug);
                await _context.SaveChangesAsync();

                // Cập nhật liên kết Category
                if (model.CategoryIds != null)
                {
                    // Xóa liên kết cũ
                    var allCategories = await _context.Categories.Where(c => c.RoomTypeId == roomType.Id).ToListAsync();
                    foreach (var cat in allCategories)
                    {
                        cat.RoomTypeId = null;
                    }
                    // Thêm liên kết mới
                    var newCategories = await _context.Categories.Where(c => model.CategoryIds.Contains(c.Id)).ToListAsync();
                    foreach (var cat in newCategories)
                    {
                        cat.RoomTypeId = roomType.Id;
                    }
                    await _context.SaveChangesAsync();
                }

                var roomTypeVM = await _context.RoomTypes.Include(rt => rt.Categories).FirstOrDefaultAsync(rt => rt.Id == roomType.Id);
                return new SuccessResponseResult(roomTypeVM.ToGetVModel(), "Room type updated successfully.");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult($"An error occurred while updating the room type: {ex.Message}");
            }
        }
    }
}