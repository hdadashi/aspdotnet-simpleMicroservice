using Gateway.Application.Services.Interfaces;
using MediatR;

namespace Gateway.Application.Features.Commands.Pay;

public class PayHandler(IPaymentClient paymentClient) : IRequestHandler<PayCommand, PayResponseDto>
{
    private readonly Random _random = new();

    public async Task<PayResponseDto> Handle(PayCommand command, CancellationToken ct)
    {
        var token = command.Token;

        var verifyResult = await paymentClient.VerifyPaymentTokenAsync(token);
        if (verifyResult is { IsSuccess: true })
        {
            return new PayResponseDto
            {
                IsSuccess = false,
                Token = token,
                Message = "توکن نامعتبر است یا منقضی شده است."
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

        return new PayResponseDto
        {
            IsSuccess =  isSuccess,
            Token = token,
            Rrn = rrn,
            Amount = verifyResult.Amount,
            Message = message,
            RedirectUrl = verifyResult.RedirectUrl
        };
    }
}