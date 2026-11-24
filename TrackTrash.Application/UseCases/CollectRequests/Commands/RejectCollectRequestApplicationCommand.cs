using MediatR;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.CollectRequests.Commands;

public sealed record RejectCollectRequestApplicationCommand(long CollectRequestId, long ApplicationId, long ActorUserId, bool IsAdmin) : IRequest;

internal sealed class RejectCollectRequestApplicationCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<RejectCollectRequestApplicationCommand>
{
    public async Task Handle(RejectCollectRequestApplicationCommand request, CancellationToken cancellationToken)
    {
        await unitOfWork.CollectRequestApplications.RejectAsync(
            request.CollectRequestId, request.ApplicationId, request.ActorUserId, request.IsAdmin, cancellationToken);
    }
}
