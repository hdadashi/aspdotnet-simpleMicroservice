using MediatR;
using Payment.Application.Contracts;

namespace Payment.Application.Features.Commands.VerifyPayment;

public record VerifyPaymentCommand(VerifyRequest Request) : IRequest<object>;