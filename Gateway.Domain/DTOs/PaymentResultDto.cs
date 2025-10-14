namespace Gateway.Domain.DTOs
{
    public class PaymentResultDto
    {
        public string Token { get; set; } = null!;
        public bool IsSuccess { get; set; }
        public string? Rrn { get; set; }
    }

    public class PaymentUpdateResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = null!;
    }
}