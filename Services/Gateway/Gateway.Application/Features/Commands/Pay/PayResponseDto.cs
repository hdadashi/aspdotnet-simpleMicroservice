namespace Gateway.Application.Features.Commands.Pay;

public record PayResponseDto
{
    public required bool IsSuccess { get; set; }
    public required string Message { get; set; }
    public required string Token { get; set; }
    public decimal? Amount { get; set; }
    public string? RedirectUrl { get; set; }
    public string? Rrn { get; set; }
}