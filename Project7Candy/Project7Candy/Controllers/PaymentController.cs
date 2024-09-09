using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Project7Candy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly PayPalService _payPalService;

        public PaymentController(PayPalService payPalService)
        {
            _payPalService = payPalService;
        }

        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] decimal amount)
        {
            var order = await _payPalService.CreateOrder(amount, "USD");
            return Ok(order);
        }

        [HttpPost("capture-order/{orderId}")]
        public async Task<IActionResult> CaptureOrder(string orderId)
        {
            var order = await _payPalService.CaptureOrder(orderId);
            return Ok(order);
        }

    }
}
