using Gateway.Application.Services.Interfaces;
using MediatR;

namespace Gateway.Application.Features.Commands.Callback;

public class CallbackHandler(IPaymentClient paymentClient) : IRequestHandler<CallbackCommand, CallbackResponseDto>
{
    public async Task<CallbackResponseDto> Handle(CallbackCommand command, CancellationToken ct)
    {
        var result = command.Result;

        var verify = await paymentClient.VerifyPaymentTokenAsync(result.Token);
        if (verify is not { IsSuccess: true })
        {
            return new CallbackResponseDto
            {
                IsSuccess = false,
                Token = result.Token,
                Message = "توکن نامعتبر یا منقضی شده است."
            };
        }

        var updateOk = await paymentClient.UpdatePaymentStatusAsync(result.Token, result.IsSuccess, result.Rrn);

        var message = result.IsSuccess ? "پرداخت با موفقیت انجام شد" : "پرداخت ناموفق بود";

        return new CallbackResponseDto
        {
            IsSuccess = result.IsSuccess && updateOk,
            Token = result.Token,
            Rrn = result.Rrn,
            Amount = verify.Amount,
            Message = message,
            RedirectUrl = verify.RedirectUrl
        };
    }
}