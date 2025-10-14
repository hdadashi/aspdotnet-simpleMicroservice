using MediatR;
using Microsoft.Extensions.Configuration;
using Payment.Application.Contracts;
using Payment.Domain.Entities;
using Payment.Domain.Enums;
using Payment.Domain.Interfaces;

namespace Payment.Application.Features.Commands.GetToken;

public class GetTokenHandler(
    ITransactionRepository txRepository,
    IConfiguration cfg)
    : IRequestHandler<GetTokenCommand, GetTokenResponse>
{
    public async Task<GetTokenResponse> Handle(GetTokenCommand request, CancellationToken ct)
    {
        var req = request.Request;
        var token = Guid.NewGuid().ToString();

        var tx = new Transaction
        {
            TerminalNo = req.TerminalNo,
            Amount = req.Amount,
            RedirectUrl = req.RedirectUrl,
            ReservationNumber = req.ReservationNumber,
            PhoneNumber = req.PhoneNumber,
            Token = token,
            Status = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await txRepository.CreateAsync(tx);

        var gatewayUrl = $"{cfg["GatewayBaseUrl"] ?? "https://localhost:5002"}/api/gateway/pay/{token}";

        return new GetTokenResponse
        {
            IsSuccess = true,
            GatewayUrl = gatewayUrl,
            Token = token,
            Message = "توکن با موفقیت ایجاد شد"
        };
    }
}