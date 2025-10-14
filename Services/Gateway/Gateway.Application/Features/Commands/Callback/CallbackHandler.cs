using Gateway.Application.Services.Interfaces;
using MediatR;

namespace Gateway.Application.Features.Commands.Callback;

public class CallbackHandler(IPaymentClient paymentClient) : IRequestHandler<CallbackCommand, object>
{
    public async Task<object> Handle(CallbackCommand command, CancellationToken ct)
    {
        var result = command.Result;

        var verify = await paymentClient.VerifyPaymentTokenAsync(result.Token);
        if (verify is not { IsSuccess: true })
        {
            return new
            {
                isSuccess = false,
                token = result.Token,
                message = "توکن نامعتبر یا منقضی شده است."
            };
        }

        var updateOk = await paymentClient.UpdatePaymentStatusAsync(result.Token, result.IsSuccess, result.Rrn);

        var message = result.IsSuccess ? "پرداخت با موفقیت انجام شد" : "پرداخت ناموفق بود";

        return new
        {
            isSuccess = result.IsSuccess && updateOk,
            token = result.Token,
            rrn = result.Rrn,
            amount = verify.Amount,
            message,
            redirectUrl = verify.RedirectUrl
        };
    }
}