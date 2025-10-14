using Payment.Domain.Enums;

namespace Payment.Domain.Entities;

public class Transaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TerminalNo { get; set; } = null!;
    public decimal Amount { get; set; }
    public string RedirectUrl { get; set; } = null!;
    public string ReservationNumber { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Token { get; set; } = null!; // store Guid as string
    public string? RRN { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string? AppCode { get; set; }
}