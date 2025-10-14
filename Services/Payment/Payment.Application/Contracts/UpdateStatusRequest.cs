namespace Payment.Application.Contracts;

public class UpdateStatusRequest
{
    public string Token { get; set; } = null!;
    public bool IsSuccess { get; set; }
    public string? Rrn { get; set; }
}