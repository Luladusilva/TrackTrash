using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.CollectionPoints.Queries;

public sealed record GetCollectionPointByIdQuery : IRequest<CollectionPointDto>
{
    public long CollectionPointId { get; set; }
}

internal sealed class GetCollectionPointByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetCollectionPointByIdQuery, CollectionPointDto>
{
    public async Task<CollectionPointDto> Handle(GetCollectionPointByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.CollectionPoints.GetByIdAsync(request.CollectionPointId, cancellationToken)
                   ?? throw new KeyNotFoundException("Collection point not found.");

        return data.ToDto();
    }
}
