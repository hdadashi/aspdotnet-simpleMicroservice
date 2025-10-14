using MediatR;
using Payment.Application.Contracts;

namespace Payment.Application.Features.Commands.GetToken;

public record GetTokenCommand(GetTokenRequest Request) : IRequest<GetTokenResponse>;