using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;
using Microsoft.AspNetCore.Authorization;

namespace TomsFurnitureBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserGuestController : ControllerBase
    {
        private readonly IUserGuestService _service;
        public UserGuestController(IUserGuestService service)
        {
            _service = service;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] UserGuestCreateVModel model)
        {
            var result = await _service.CreateAsync(model);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        //[HttpDelete("{id}")]
        //[Authorize]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var result = await _service.DeleteAsync(id);
        //    if (!result.IsSuccess) return BadRequest(result);
        //    return Ok(result);
        //}

        //// API tìm ki?m khách vãng lai theo s? ?i?n tho?i ho?c email
        //[HttpGet("find")]
        //[AllowAnonymous]
        //public async Task<IActionResult> Find([FromQuery] string? phone, [FromQuery] string? email)
        //{
        //    var result = await _service.FindByPhoneOrEmailAsync(phone, email);
        //    if (result == null) return NotFound();
        //    return Ok(result);
        //}
    }
}
