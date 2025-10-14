namespace Gateway.Domain.DTOs
{
    public record PaymentVerifyResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Token { get; set; } = null!;
        public decimal Amount { get; set; }
        public string ReservationNumber { get; set; } = null!;
        public string RedirectUrl { get; set; } = null!;
        public string Message { get; set; } = null!;
    }
}