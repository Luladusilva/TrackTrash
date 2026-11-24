using MediatR;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.Gatherers.Commands;

public sealed record SetGathererResidueTypesCommand(
    long GathererId,
    long ActorUserId,
    bool IsAdmin,
    IReadOnlyCollection<long> ResidueTypeIds) : IRequest;

internal sealed class SetGathererResidueTypesCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<SetGathererResidueTypesCommand>
{
    public async Task Handle(SetGathererResidueTypesCommand request, CancellationToken cancellationToken)
    {
        await unitOfWork.Gatherers.UpdateSupportedResiduesAsync(
            request.GathererId, request.ActorUserId, request.IsAdmin, request.ResidueTypeIds, cancellationToken);
    }
}
