namespace Payment.Application.Features.Commands.UpdateStatus;

public record UpdateStatusResponseDto
{
    public required bool IsSuccess { get; set; }
    public required string Message { get; set; }
    public string? Status { get; set; }
    public decimal? Amount { get; set; }
    public string? ReservationNumber { get; set; }
    public string? Rrn { get; set; }
}