using FluentValidation;

namespace Payment.Application.Contracts;

public class GetTokenRequest
{
    public string TerminalNo { get; set; } = null!;
    public decimal Amount { get; set; }
    public string RedirectUrl { get; set; } = null!;
    public string ReservationNumber { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
}

public class GetTokenRequestValidator : AbstractValidator<GetTokenRequest>
{
    public  GetTokenRequestValidator()
    {
        RuleFor(x => x.TerminalNo).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.RedirectUrl).NotEmpty().Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute));
        RuleFor(x => x.ReservationNumber)
            .NotEmpty().WithMessage("Reservation number is required.")
            .Matches(@"^\d+$").WithMessage("Reservation number must contain only digits.");
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^09\d{9}$").WithMessage("Phone number must be in the format 09129991111.");

    }
}