using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.Gatherers.Queries;

public sealed record GetGatherersQuery : IRequest<IReadOnlyList<GathererDto>>;

internal sealed class GetGatherersQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetGatherersQuery, IReadOnlyList<GathererDto>>
{
    public async Task<IReadOnlyList<GathererDto>> Handle(GetGatherersQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.Gatherers.GetAllProfilesAsync(cancellationToken);
        return data.Select(x => x.ToDto()).ToList();
    }
}
