using System.Net.Http.Json;
using Gateway.Application.Services.Interfaces;
using Gateway.Domain.DTOs;
using Microsoft.Extensions.Configuration;

namespace Gateway.Application.Services
{
    public class PaymentClient(HttpClient http, IConfiguration cfg) : IPaymentClient
    {
        public async Task<PaymentUpdateResponseDto> UpdatePaymentStatusAsync(PaymentResultDto result)
        {
            var baseUrl = cfg["PaymentService:BaseUrl"];
            var response = await http.PostAsJsonAsync($"{baseUrl}/api/payment/update-status", result);
            var content = await response.Content.ReadFromJsonAsync<PaymentUpdateResponseDto>();
            return content ?? new PaymentUpdateResponseDto { IsSuccess = false, Message = "No response" };
        }
    }
}