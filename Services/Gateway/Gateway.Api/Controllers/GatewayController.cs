using Gateway.Application.Features.Commands.Callback;
using Gateway.Application.Features.Commands.Pay;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Gateway.Domain.DTOs;

namespace Gateway.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GatewayController(IMediator mediator) : ControllerBase
    {
        [HttpGet("pay/{token}")]
        public async Task<IActionResult> Pay(string token)
        {
            var result = await mediator.Send(new PayCommand(token));
            return Ok(result);
        }

        [HttpPost("callback")]
        public async Task<IActionResult> Callback([FromBody] PaymentResultDto result)
        {
            var response = await mediator.Send(new CallbackCommand(result));
            return Ok(response);
        }
    }
}