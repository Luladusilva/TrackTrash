using MediatR;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.CollectRequests.Commands;

public sealed record AcceptCollectRequestApplicationCommand(long CollectRequestId, long ApplicationId, long ActorUserId, bool IsAdmin) : IRequest<long>;

internal sealed class AcceptCollectRequestApplicationCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<AcceptCollectRequestApplicationCommand, long>
{
    public Task<long> Handle(AcceptCollectRequestApplicationCommand request, CancellationToken cancellationToken) =>
        unitOfWork.CollectRequestApplications.AcceptAsync(
            request.CollectRequestId, request.ApplicationId, request.ActorUserId, request.IsAdmin, cancellationToken);
}
