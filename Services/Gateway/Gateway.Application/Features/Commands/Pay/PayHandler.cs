using Gateway.Application.Services.Interfaces;
using MediatR;

namespace Gateway.Application.Features.Commands.Pay;

public class PayHandler(IPaymentClient paymentClient) : IRequestHandler<PayCommand, object>
{
    private readonly Random _random = new();

    public async Task<object> Handle(PayCommand command, CancellationToken ct)
    {
        var token = command.Token;

        var verifyResult = await paymentClient.VerifyPaymentTokenAsync(token);
        if (verifyResult is { IsSuccess: true })
        {
            return new
            {
                isSuccess = false,
                token,
                message = "توکن نامعتبر است یا منقضی شده است."
            };
        }

        var isSuccess = _random.Next(1, 101) <= 80;
        string? rrn = null;

        if (isSuccess)
        {
            rrn = string.Concat(Enumerable.Range(0, 12).Select(_ => _random.Next(0, 10).ToString()));
        }

        await paymentClient.UpdatePaymentStatusAsync(token, isSuccess, rrn);

        var message = isSuccess ? "پرداخت با موفقیت انجام شد" : "پرداخت ناموفق بود";

        return new
        {
            isSuccess,
            token,
            rrn,
            amount = verifyResult.Amount,
            message,
            redirectUrl = verifyResult.RedirectUrl
        };
    }
}