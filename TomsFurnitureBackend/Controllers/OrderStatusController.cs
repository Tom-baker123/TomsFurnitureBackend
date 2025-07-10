using Microsoft.AspNetCore.Mvc;
using OA.Domain.Common.Models;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderStatusController : ControllerBase
    {
        private readonly IOrderStatusService _service;
        private readonly ILogger<OrderStatusController> _logger;

        public OrderStatusController(IOrderStatusService service, ILogger<OrderStatusController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<List<OrderStatusGetVModel>> GetAll()
        {
            return await _service.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { Message = "Order status not found." });
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderStatusCreateVModel model)
        {
            var result = await _service.CreateAsync(model);
            if (!result.IsSuccess)
                return BadRequest(result.Message);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] OrderStatusUpdateVModel model)
        {
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
