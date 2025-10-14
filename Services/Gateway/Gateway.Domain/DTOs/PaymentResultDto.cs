namespace Gateway.Domain.DTOs
{
    public record PaymentResultDto
    {
        public string Token { get; set; } = null!;
        public bool IsSuccess { get; set; }
        public string? Rrn { get; set; }
    }
}