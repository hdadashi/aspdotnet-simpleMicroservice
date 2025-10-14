using Microsoft.AspNetCore.Mvc;
using Gateway.Application.Services.Interfaces;
using Gateway.Domain.DTOs;

namespace Gateway.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GatewayController(IPaymentClient paymentClient) : ControllerBase
    {
        // simulate payment page
        [HttpGet("pay/{token}")]
        public Task<IActionResult> Pay(string token)
        {
            // Normally user sees bank page — I tried to simulate it:
            return Task.FromResult<IActionResult>(Ok(new
            {
                message = "Simulated payment page",
                token,
                info = "Use POST /api/gateway/callback to simulate payment result"
            }));
        }

        [HttpPost("callback")]
        public async Task<IActionResult> Callback([FromBody] PaymentResultDto result)
        {
            var updateResult = await paymentClient.UpdatePaymentStatusAsync(result);

            return Ok(new
            {
                isSuccess = updateResult.IsSuccess,
                message = updateResult.Message
            });
        }
    }
}