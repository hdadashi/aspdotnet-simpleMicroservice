using MediatR;

namespace Gateway.Application.Features.Commands.Pay;

public record PayCommand(string Token) : IRequest<object>;