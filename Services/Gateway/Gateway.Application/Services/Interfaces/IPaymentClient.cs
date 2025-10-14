using Gateway.Domain.DTOs;

namespace Gateway.Application.Services.Interfaces
{
    public interface IPaymentClient
    {
        Task<PaymentVerifyResponseDto?> VerifyPaymentTokenAsync(string token);
        Task<bool> UpdatePaymentStatusAsync(string token, bool isSuccess, string? rrn);

    }
}