using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Enums;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.CollectRequests.Queries;

public sealed record GetPendingCollectRequestsQuery(long? GathererUserId = null) : IRequest<IReadOnlyList<CollectRequestDto>>;

internal sealed class GetPendingCollectRequestsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetPendingCollectRequestsQuery, IReadOnlyList<CollectRequestDto>>
{
    public async Task<IReadOnlyList<CollectRequestDto>> Handle(GetPendingCollectRequestsQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<TrackTrash.Domain.Entities.CollectRequest> data;
        if (request.GathererUserId is null)
        {
            data = await unitOfWork.CollectRequests.GetPendingAsync(cancellationToken);
        }
        else
        {
            var gatherer = await unitOfWork.Gatherers.GetByUserIdAsync(request.GathererUserId.Value, cancellationToken)
                ?? throw new UnauthorizedAccessException("A gatherer profile is required.");
            if (!gatherer.IsActive || !gatherer.User.IsActive || gatherer.VerificationStatusId != VerificationStatus.Verified)
                throw new UnauthorizedAccessException("Only active, verified gatherers can view compatible requests.");
            data = await unitOfWork.CollectRequests.GetPendingCompatibleWithResiduesAsync(
                gatherer.ResidueTypes.Select(x => x.ResidueTypeId).ToArray(), cancellationToken);
        }
        return data.Select(x => x.ToDto()).ToList();
    }
}
