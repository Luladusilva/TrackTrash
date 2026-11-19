using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.Locations.Queries;

public sealed record GetLocationsQuery : IRequest<IReadOnlyList<LocationDto>>;

internal sealed class GetLocationsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetLocationsQuery, IReadOnlyList<LocationDto>>
{
    public async Task<IReadOnlyList<LocationDto>> Handle(GetLocationsQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.Locations.GetAllAsync(cancellationToken);
        return data.Select(x => x.ToDto()).ToList();
    }
}
