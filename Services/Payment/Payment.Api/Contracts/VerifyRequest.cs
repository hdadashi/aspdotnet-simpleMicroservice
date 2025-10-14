namespace Payment.Api.Contracts;

public class VerifyRequest
{
    public string Token { get; set; } = null!;
    public string AppCode { get; set; } = null!;
}