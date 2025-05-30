using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OA.Domain.Common.Models;
using TomsFurnitureBackend.Models;
using TomsFurnitureBackend.Services.IServices;
using TomsFurnitureBackend.VModels;

namespace TomsFurnitureBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentMethodController : ControllerBase
    {
        private readonly TomfurnitureContext _context;
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly ILogger<PaymentMethodController> _logger;

        // Constructor nhận các dependency qua DI
        public PaymentMethodController(IPaymentMethodService paymentMethodService, ILogger<PaymentMethodController> logger, TomfurnitureContext context)
        {
            _context = context;
            _paymentMethodService = paymentMethodService;
            _logger = logger;
        }

        // [1.] Lấy danh sách tất cả phương thức thanh toán
        [HttpGet]
        public async Task<List<PaymentMethodGetVModel>> GetAllPaymentMethod()
        {
            // Gọi service để lấy danh sách tất cả phương thức thanh toán
            return await _paymentMethodService.GetAllAsync();
        }

        // [2.] Lấy phương thức thanh toán theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdPaymentMethod(int id)
        {
            // Gọi service để lấy phương thức thanh toán theo ID
            var paymentMethod = await _paymentMethodService.GetByIdAsync(id);
            if (paymentMethod == null)
            {
                return NotFound(new { Message = "Payment method not found." });
            }
            return Ok(paymentMethod);
        }

        // [3.] Tạo phương thức thanh toán mới
        [HttpPost]
        public async Task<IActionResult> CreatePaymentMethod([FromBody] PaymentMethodCreateVModel paymentMethodVModel)
        {
            try
            {
                // B1: Gọi service để tạo phương thức thanh toán
                var result = await _paymentMethodService.CreateAsync(paymentMethodVModel);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.Message);
                }

                // B2: Trả về phản hồi thành công
                var successResult = result as SuccessResponseResult;
                return CreatedAtAction(
                    nameof(GetByIdPaymentMethod),
                    new { id = ((PaymentMethodGetVModel)successResult.Data).Id },
                    new
                    {
                        Message = successResult.Message,
                        PaymentMethodId = ((PaymentMethodGetVModel)successResult.Data).Id
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating payment method: {Error}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while creating the payment method.", Error = ex.Message });
            }
        }

        // [4.] Cập nhật phương thức thanh toán
        [HttpPut]
        public async Task<IActionResult> UpdatePaymentMethod([FromForm] PaymentMethodUpdateVModel paymentMethodVModel)
        {
            try
            {
                // B1: Gọi service để cập nhật phương thức thanh toán
                var result = await _paymentMethodService.UpdateAsync(paymentMethodVModel);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.Message);
                }

                // Trả về phản hồi thành công
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating payment method: {Error}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while updating the payment method.", Error = ex.Message });
            }
        }

        // [5.] Xóa phương thức thanh toán theo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaymentMethod(int id)
        {
            try
            {
                // B1: Gọi service để xóa phương thức thanh toán
                var result = await _paymentMethodService.DeleteAsync(id);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.Message);
                }

                // B2: Trả về phản hồi thành công
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error deleting payment method: {Error}", ex.Message);
                return StatusCode(500, new { Message = "An error occurred while deleting the payment method.", Error = ex.Message });
            }
        }
    }
}
