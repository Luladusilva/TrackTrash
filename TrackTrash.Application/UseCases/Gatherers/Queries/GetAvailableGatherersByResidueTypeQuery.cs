using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.Gatherers.Queries;

public sealed record GetAvailableGatherersByResidueTypeQuery : IRequest<IReadOnlyList<GathererDto>>
{
    public long ResidueTypeId { get; set; }
}

internal sealed class GetAvailableGatherersByResidueTypeQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAvailableGatherersByResidueTypeQuery, IReadOnlyList<GathererDto>>
{
    public async Task<IReadOnlyList<GathererDto>> Handle(GetAvailableGatherersByResidueTypeQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.Gatherers.GetAvailableByResidueTypeAsync(request.ResidueTypeId, cancellationToken);
        return data.Select(x => x.ToDto()).ToList();
    }
}
