using Gateway.Domain.DTOs;
using MediatR;

namespace Gateway.Application.Features.Commands.Callback;

public record CallbackCommand(PaymentResultDto Result) : IRequest<CallbackResponseDto>;