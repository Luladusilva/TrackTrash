using MediatR;
using TrackTrash.Application.Dtos;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.Locations.Queries;

public sealed record GetLocationByIdQuery : IRequest<LocationDto>
{
    public long LocationId { get; set; }
}

internal sealed class GetLocationByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetLocationByIdQuery, LocationDto>
{
    public async Task<LocationDto> Handle(GetLocationByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.Locations.GetByIdAsync(request.LocationId, cancellationToken)
                   ?? throw new KeyNotFoundException("Location not found.");

        return data.ToDto();
    }
}
