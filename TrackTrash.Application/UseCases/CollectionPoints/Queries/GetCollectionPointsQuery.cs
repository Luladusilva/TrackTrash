using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.CollectionPoints.Queries;

public sealed record GetCollectionPointsQuery : IRequest<IReadOnlyList<CollectionPointDto>>;

internal sealed class GetCollectionPointsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetCollectionPointsQuery, IReadOnlyList<CollectionPointDto>>
{
    public async Task<IReadOnlyList<CollectionPointDto>> Handle(GetCollectionPointsQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.CollectionPoints.GetAllAsync(cancellationToken);
        return data.Select(x => x.ToDto()).ToList();
    }
}
