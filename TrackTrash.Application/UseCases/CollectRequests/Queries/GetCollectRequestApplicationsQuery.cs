using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.CollectRequests.Queries;

public sealed record GetCollectRequestApplicationsQuery(long CollectRequestId) : IRequest<IReadOnlyList<CollectRequestApplicationDto>>;

internal sealed class GetCollectRequestApplicationsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetCollectRequestApplicationsQuery, IReadOnlyList<CollectRequestApplicationDto>>
{
    public async Task<IReadOnlyList<CollectRequestApplicationDto>> Handle(
        GetCollectRequestApplicationsQuery request,
        CancellationToken cancellationToken)
    {
        var applications = await unitOfWork.CollectRequestApplications.GetByRequestAsync(request.CollectRequestId, cancellationToken);
        return applications.Select(x => x.ToDto()).ToList();
    }
}
