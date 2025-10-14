using Gateway.Domain.DTOs;

namespace Gateway.Application.Services.Interfaces
{
    public interface IPaymentClient
    {
        Task<PaymentUpdateResponseDto> UpdatePaymentStatusAsync(PaymentResultDto result);
    }
}