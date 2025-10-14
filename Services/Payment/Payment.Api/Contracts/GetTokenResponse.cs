namespace Payment.Api.Contracts;

public class GetTokenResponse
{
    public bool IsSuccess { get; set; }
    public string GatewayUrl { get; set; } = null!;
    public string Token { get; set; } = null!;
    public string Message { get; set; } = null!;
}
