using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.CollectionPoints.Queries;

public sealed record GetActiveCollectionPointsByResidueTypeQuery : IRequest<IReadOnlyList<CollectionPointDto>>
{
    public long ResidueTypeId { get; set; }
}

internal sealed class GetActiveCollectionPointsByResidueTypeQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetActiveCollectionPointsByResidueTypeQuery, IReadOnlyList<CollectionPointDto>>
{
    public async Task<IReadOnlyList<CollectionPointDto>> Handle(GetActiveCollectionPointsByResidueTypeQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.CollectionPoints.GetActiveByResidueTypeAsync(request.ResidueTypeId, cancellationToken);
        return data.Select(x => x.ToDto()).ToList();
    }
}
