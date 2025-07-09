using System;
using System.Collections.Generic;
using TomsFurnitureBackend.VModels;
using static TomsFurnitureBackend.VModels.ProductVModel;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OA.Domain.Common.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TomsFurnitureBackend.Services.Interfaces;

namespace TomsFurnitureBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductVariantImageController : ControllerBase
    {
        private readonly IProductVariantImageService _service;
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<ProductVariantImageController> _logger;

        public ProductVariantImageController(IProductVariantImageService service, Cloudinary cloudinary, ILogger<ProductVariantImageController> logger)
        {
            _service = service;
            _cloudinary = cloudinary;
            _logger = logger;
        }

        [HttpGet]
        public async Task<List<ProductVariantImageGetVModel>> GetAll()
        {
            return await _service.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductVariantImageGetVModel>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductVariantImageCreateVModel model, IFormFile imageFile)
        {
            try
            {
                string imageUrl = null;
                if (imageFile != null && imageFile.Length > 0)
                {
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                        return BadRequest("Unsupported file type");
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(imageFile.FileName, imageFile.OpenReadStream())
                    };
                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    if (uploadResult.Error != null)
                    {
                        _logger.LogError("Cloudinary upload error: {ErrorMessage}", uploadResult.Error.Message);
                        return BadRequest($"Cloudinary upload failed: {uploadResult.Error.Message}");
                    }
                    imageUrl = uploadResult.SecureUrl.AbsoluteUri;
                }
                var result = await _service.CreateAsync(model, imageUrl);
                if (!result.IsSuccess) return BadRequest(result.Message);
                var successResult = result as SuccessResponseResult;
                return CreatedAtAction(nameof(GetById), new { id = successResult.Data?.Id }, successResult.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating product variant image: {Error}", ex.Message);
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromForm] ProductVariantImageUpdateVModel model, IFormFile imageFile = null)
        {
            try
            {
                string imageUrl = null;
                if (imageFile != null && imageFile.Length > 0)
                {
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                        return BadRequest("Unsupported file type");
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(imageFile.FileName, imageFile.OpenReadStream())
                    };
                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    if (uploadResult.Error != null)
                    {
                        _logger.LogError("Cloudinary upload error: {ErrorMessage}", uploadResult.Error.Message);
                        return BadRequest($"Cloudinary upload failed: {uploadResult.Error.Message}");
                    }
                    imageUrl = uploadResult.SecureUrl.AbsoluteUri;
                }
                var result = await _service.UpdateAsync(model, imageUrl);
                if (!result.IsSuccess) return BadRequest(result.Message);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating product variant image: {Error}", ex.Message);
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result.IsSuccess) return BadRequest(result.Message);
            return Ok(result);
        }
    }
}
