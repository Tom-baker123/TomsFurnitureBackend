using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using OA.Domain.Common.Models;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TomsFurnitureBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannerController : ControllerBase
    {
        private readonly IBannerService _bannerService;
        private readonly Cloudinary _cloudinary;

        public BannerController(IBannerService bannerService, Cloudinary cloudinary)
        {
            _bannerService = bannerService ?? throw new ArgumentNullException(nameof(bannerService));
            _cloudinary = cloudinary ?? throw new ArgumentNullException(nameof(cloudinary));
        }

        // Upload ảnh lên Cloudinary
        private async Task<string> UploadImageToCloudinary(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is required.");

            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "banners"
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.ToString();
        }

        // GET: api/Banner
        [HttpGet]
        public async Task<ActionResult<List<BannerGetVModel>>> GetAll()
        {
            var banners = await _bannerService.GetAllAsync();
            return Ok(banners);
        }

        // GET: api/Banner/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BannerGetVModel>> GetById(int id)
        {
            var banner = await _bannerService.GetByIdAsync(id);
            if (banner == null)
                return NotFound(new ErrorResponseResult("Banner not found."));
            return Ok(banner);
        }

        // POST: api/Banner
        [HttpPost]
        public async Task<ActionResult<ResponseResult>> Create([FromForm] BannerCreateVModel model, IFormFile imageFile, IFormFile imageFileMobile)
        {
            try
            {
                // Upload ảnh lên Cloudinary
                var imageUrl = await UploadImageToCloudinary(imageFile);
                var imageUrlMobile = await UploadImageToCloudinary(imageFileMobile);

                // Tạo banner
                var result = await _bannerService.CreateAsync(model, imageUrl, imageUrlMobile);
                if (result is ErrorResponseResult error)
                    return BadRequest(error);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseResult("Error uploading images: " + ex.Message));
            }
        }

        // PUT: api/Banner
        [HttpPut]
        public async Task<ActionResult<ResponseResult>> Update([FromForm] BannerUpdateVModel model, IFormFile? imageFile = null, IFormFile? imageFileMobile = null)
        {
            try
            {
                string? imageUrl = null;
                string? imageUrlMobile = null;

                // Upload ảnh mới nếu có
                if (imageFile != null)
                    imageUrl = await UploadImageToCloudinary(imageFile);
                if (imageFileMobile != null)
                    imageUrlMobile = await UploadImageToCloudinary(imageFileMobile);

                // Cập nhật banner
                var result = await _bannerService.UpdateAsync(model, imageUrl, imageUrlMobile);
                if (result is ErrorResponseResult error)
                    return BadRequest(error);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseResult("Error uploading images: " + ex.Message));
            }
        }

        // DELETE: api/Banner/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseResult>> Delete(int id)
        {
            var result = await _bannerService.DeleteAsync(id);
            if (result is ErrorResponseResult error)
                return BadRequest(error);

            return Ok(result);
        }
    }
}