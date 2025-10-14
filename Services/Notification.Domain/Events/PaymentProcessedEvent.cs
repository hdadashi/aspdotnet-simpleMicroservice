namespace Notification.Domain.Events
{
    public class PaymentProcessedEvent
    {
        public Guid TransactionId { get; set; }
        public string Token { get; set; } = null!;
        public decimal Amount { get; set; }
        public string? Rrn { get; set; }
        public string? AppCode { get; set; }
        public string Status { get; set; } = null!;
        public DateTime ProcessedAt { get; set; }
    }
}