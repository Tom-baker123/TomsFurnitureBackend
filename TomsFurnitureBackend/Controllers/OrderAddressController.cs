using Microsoft.AspNetCore.Mvc;
using OA.Domain.Common.Models;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;
using TomsFurnitureBackend.Services.Interfaces;
using TomsFurnitureBackend.Services;

namespace TomsFurnitureBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderAddressController : ControllerBase
    {
        private readonly IOrderAddressService _service;
        private readonly ILogger<OrderAddressController> _logger;
        private readonly IAuthService _authService;

        public OrderAddressController(IOrderAddressService service, ILogger<OrderAddressController> logger, IAuthService authService)
        {
            _service = service;
            _logger = logger;
            _authService = authService;
        }

        [HttpGet]
        public async Task<List<OrderAddressGetVModel>> GetAll(
            [FromQuery] int? userId,
            [FromQuery(Name = "isDeafaultAddress")] bool? isDeafaultAddress)
        {
            return await _service.GetAllAsync(userId, isDeafaultAddress);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { Message = "Order address not found." });
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderAddressCreateVModel model)
        {
            var authStatus = await _authService.GetAuthStatusAsync(User, HttpContext);
            if (!authStatus.IsAuthenticated)
                return Unauthorized(new { Message = "User is not authenticated." });
            model.UserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _service.CreateAsync(model);
            if (!result.IsSuccess)
                return BadRequest(result.Message);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] OrderAddressUpdateVModel model)
        {
            var authStatus = await _authService.GetAuthStatusAsync(User, HttpContext);
            if (!authStatus.IsAuthenticated)
                return Unauthorized(new { Message = "User is not authenticated." });
            model.UserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _service.UpdateAsync(model);
            if (!result.IsSuccess)
                return BadRequest(result.Message);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result.Message);
            return Ok(result);
        }
    }
}
