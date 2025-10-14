namespace Payment.Application.EventBus
{
    public class PaymentProcessedEvent
    {
        public string Token { get; set; } = null!;
        public string ReservationNumber { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Status { get; set; } = null!; // Success, Failed, Expired
        public string? Rrn { get; set; }
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    }
}