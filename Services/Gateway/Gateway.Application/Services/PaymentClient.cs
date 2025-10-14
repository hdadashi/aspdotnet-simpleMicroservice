using System.Net.Http.Json;
using Gateway.Application.Services.Interfaces;
using Gateway.Domain.DTOs;
using Microsoft.Extensions.Configuration;

namespace Gateway.Application.Services
{
    public class PaymentClient(HttpClient http, IConfiguration cfg) : IPaymentClient
    {
        public async Task<PaymentVerifyResponseDto?> VerifyPaymentTokenAsync(string token)
        {
            var baseUrl = cfg["PaymentService:BaseUrl"];
            var verifyUrl = $"{baseUrl}/api/payment/verify";
            var response = await http.PostAsJsonAsync(verifyUrl, new { token, appCode = "GatewayService" });
            if (!response.IsSuccessStatusCode) return null;

            var result = await response.Content.ReadFromJsonAsync<PaymentVerifyResponseDto>();
            return result;
        }

        public async Task<bool> UpdatePaymentStatusAsync(string token, bool isSuccess, string? rrn)
        {
            var baseUrl = cfg["PaymentService:BaseUrl"];
            var updateUrl = $"{baseUrl}/api/payment/update-status";
            var body = new PaymentUpdateStatusRequest
            {
                Token = token,
                IsSuccess = isSuccess,
                Rrn = rrn
            };

            var response = await http.PostAsJsonAsync(updateUrl, body);
            return response.IsSuccessStatusCode;
        }
    }
}