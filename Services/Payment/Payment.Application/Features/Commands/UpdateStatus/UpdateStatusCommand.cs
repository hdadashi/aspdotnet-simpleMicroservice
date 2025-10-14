using MediatR;
using Payment.Application.Contracts;

namespace Payment.Application.Features.Commands.UpdateStatus;

public record UpdateStatusCommand(UpdateStatusRequest Request) : IRequest<object>;