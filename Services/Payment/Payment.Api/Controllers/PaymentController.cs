using MediatR;
using Microsoft.AspNetCore.Mvc;
using Payment.Application.Contracts;
using Payment.Application.Features.Commands.GetToken;
using Payment.Application.Features.Commands.UpdateStatus;
using Payment.Application.Features.Commands.VerifyPayment;

namespace Payment.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController(IMediator mediator) : ControllerBase
{
    [HttpPost("get-token")]
    public async Task<IActionResult> GetToken([FromBody] GetTokenRequest req)
    {
        var result = await mediator.Send(new GetTokenCommand(req));
        return Ok(result);
    }

    [HttpPost("verify")]
    public async Task<IActionResult> Verify([FromBody] VerifyRequest req)
    {
        var result = await mediator.Send(new VerifyPaymentCommand(req));
        return Ok(result);
    }

    [HttpPost("update-status")]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusRequest req)
    {
        var result = await mediator.Send(new UpdateStatusCommand(req));
        return Ok(result);
    }
}