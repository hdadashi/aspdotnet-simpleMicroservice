using Microsoft.AspNetCore.Mvc;
using Gateway.Application.Services.Interfaces;
using Gateway.Domain.DTOs;

namespace Gateway.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GatewayController(IPaymentClient paymentClient) : ControllerBase
    {
        private readonly Random _random = new();

        // simulate payment page
        [HttpGet("pay/{token}")]
        public async Task<IActionResult> Pay(string token)
        {
            var verifyResult = await paymentClient.VerifyPaymentTokenAsync(token);
            if (verifyResult is { IsSuccess: true })
            {
                return BadRequest(new
                {
                    isSuccess = false,
                    token,
                    message = "توکن نامعتبر است یا منقضی شده است."
                });
            }

            var isSuccess = _random.Next(1, 101) <= 80;

            string? rrn = null;
            if (isSuccess)
            {
                rrn = string.Concat(Enumerable.Range(0, 12).Select(_ => _random.Next(0, 10).ToString()));
            }

            await paymentClient.UpdatePaymentStatusAsync(token, isSuccess, rrn);

            var message = isSuccess ? "پرداخت با موفقیت انجام شد" : "پرداخت ناموفق بود";

            var response = new
            {
                isSuccess,
                token,
                rrn,
                amount = verifyResult.Amount,
                message,
                redirectUrl = verifyResult.RedirectUrl
            };

            return Ok(response);
        }

        [HttpPost("callback")]
        public async Task<IActionResult> Callback([FromBody] PaymentResultDto result)
        {
            var verify = await paymentClient.VerifyPaymentTokenAsync(result.Token);
            if (verify is not { IsSuccess: true })
            {
                return Ok(new
                {
                    isSuccess = false,
                    token = result.Token,
                    message = "توکن نامعتبر یا منقضی شده است."
                });
            }

            var updateOk = await paymentClient.UpdatePaymentStatusAsync(result.Token, result.IsSuccess, result.Rrn);

            var message = result.IsSuccess ? "پرداخت با موفقیت انجام شد" : "پرداخت ناموفق بود";

            var response = new
            {
                isSuccess = result.IsSuccess && updateOk,
                token = result.Token,
                rrn = result.Rrn,
                amount = verify.Amount,
                message,
                redirectUrl = verify.RedirectUrl
            };

            return Ok(response);
        }
    }
}