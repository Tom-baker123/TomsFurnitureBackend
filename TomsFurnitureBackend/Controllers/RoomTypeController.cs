using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OA.Domain.Common.Models;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;
using TomsFurnitureBackend.Helpers;

namespace TomsFurnitureBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomTypeController : ControllerBase
    {
        private readonly TomfurnitureContext _context;
        private readonly IRoomTypeService _roomTypeService;
        private readonly ILogger<RoomTypeController> _logger;
        private readonly Cloudinary _cloudinary;

        public RoomTypeController(IRoomTypeService roomTypeService, ILogger<RoomTypeController> logger, Cloudinary cloudinary, TomfurnitureContext context)
        {
            _context = context;
            _roomTypeService = roomTypeService;
            _logger = logger;
            _cloudinary = cloudinary;
        }

        // Lấy danh sách tất cả loại phòng
        [HttpGet]
        public async Task<List<RoomTypeGetVModel>> GetAllRoomTypes()
        {
            return await _roomTypeService.GetAllAsync();
        }

        // Lấy loại phòng theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoomTypeById(int id)
        {
            var roomType = await _roomTypeService.GetByIdAsync(id);
            if (roomType == null)
            {
                return NotFound(new { Message = "Room type not found." });
            }
            return Ok(roomType);
        }

        // Tạo loại phòng mới
        [HttpPost]
        public async Task<IActionResult> CreateRoomType([FromForm] RoomTypeCreateVModel roomTypeVModel, IFormFile? imageFile)
        {
            try
            {
                string? imageUrl = null;
                if (imageFile != null && imageFile.Length > 0)
                {
                    try
                    {
                        imageUrl = await CloudinaryHelper.HandleSliderImageUpload(_cloudinary, imageFile, _logger);
                    }
                    catch (ArgumentException ex)
                    {
                        return BadRequest(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex.Message);
                    }
                }

                var result = await _roomTypeService.CreateAsync(roomTypeVModel, imageUrl);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.Message);
                }

                var successResult = result as SuccessResponseResult;
                return CreatedAtAction(nameof(GetRoomTypeById),
                    new { id = successResult.Data.Id },
                    new { Message = successResult.Message, Data = successResult.Data.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while creating the room type.", Error = ex.Message });
            }
        }

        // Xóa loại phòng theo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoomType(int id)
        {
            try
            {
                var roomType = await _context.RoomTypes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(rt => rt.Id == id);
                if (roomType == null)
                {
                    return NotFound($"Not found ID {id} in room type");
                }

                if (!string.IsNullOrEmpty(roomType.ImageUrl))
                {
                    try
                    {
                        var uri = new Uri(roomType.ImageUrl);
                        var publicId = Path.GetFileNameWithoutExtension(uri.AbsolutePath);

                        var deletionParams = new DeletionParams(publicId)
                        {
                            ResourceType = ResourceType.Image
                        };

                        var deletionResult = await _cloudinary.DestroyAsync(deletionParams);
                        if (deletionResult.Error != null)
                        {
                            _logger.LogWarning("Không thể xóa ảnh trên Cloudinary: {ErrorMessage}", deletionResult.Error.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, new { Message = "An error occurred while deleting the image.", Details = ex.Message });
                    }
                }

                var result = await _roomTypeService.DeleteAsync(id);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.Message);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while deleting the room type.", Details = ex.Message });
            }
        }

        // Cập nhật loại phòng
        [HttpPut]
        public async Task<IActionResult> UpdateRoomType([FromForm] RoomTypeUpdateVModel roomTypeVModel, IFormFile? imageFile = null)
        {
            try
            {
                string? imageUrl = null;
                if (imageFile != null && imageFile.Length > 0)
                {
                    try
                    {
                        imageUrl = await CloudinaryHelper.HandleSliderImageUpload(_cloudinary, imageFile, _logger);
                    }
                    catch (ArgumentException ex)
                    {
                        return BadRequest(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex.Message);
                    }

                    var existingRoomType = await _context.RoomTypes
                        .AsNoTracking()
                        .FirstOrDefaultAsync(rt => rt.Id == roomTypeVModel.Id);
                    if (existingRoomType == null)
                    {
                        return NotFound($"Not found ID {roomTypeVModel.Id} in room type");
                    }

                    if (!string.IsNullOrEmpty(existingRoomType.ImageUrl))
                    {
                        try
                        {
                            var uri = new Uri(existingRoomType.ImageUrl);
                            var publicId = Path.GetFileNameWithoutExtension(uri.AbsolutePath);

                            var deletionParams = new DeletionParams(publicId)
                            {
                                ResourceType = ResourceType.Image
                            };

                            var deletionResult = await _cloudinary.DestroyAsync(deletionParams);
                            if (deletionResult.Error != null)
                            {
                                _logger.LogWarning("Không thể xóa ảnh cũ trên Cloudinary: {ErrorMessage}", deletionResult.Error.Message);
                            }
                        }
                        catch (Exception ex)
                        {
                            return StatusCode(500, new { Message = $"Error when deleting old image in Cloudinary: {ex.Message}" });
                        }
                    }
                }

                var result = await _roomTypeService.UpdateAsync(roomTypeVModel, imageUrl);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.Message);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error when updating room type: {ex.Message}" });
            }
        }
    }
}
