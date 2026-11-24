using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.CollectRequests.Queries;

public sealed record GetMyCollectRequestApplicationsQuery(long UserId) : IRequest<IReadOnlyList<CollectRequestApplicationDto>>;

internal sealed class GetMyCollectRequestApplicationsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetMyCollectRequestApplicationsQuery, IReadOnlyList<CollectRequestApplicationDto>>
{
    public async Task<IReadOnlyList<CollectRequestApplicationDto>> Handle(
        GetMyCollectRequestApplicationsQuery request,
        CancellationToken cancellationToken)
    {
        var applications = await unitOfWork.CollectRequestApplications.GetByGathererUserIdAsync(request.UserId, cancellationToken);
        return applications.Select(x => x.ToDto()).ToList();
    }
}
