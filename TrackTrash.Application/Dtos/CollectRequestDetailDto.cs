using TrackTrash.Domain.Entities;

namespace TrackTrash.Application.Dtos;

public sealed record CollectRequestDetailDto
{
    public CollectRequestDto Request { get; init; } = null!;

    public LocationDto Location { get; init; } = null!;

    public IReadOnlyList<RequestDetailDto> Details { get; init; } = [];

    public PickupServiceDto? PickupService { get; init; }
}

internal static class CollectRequestDetailDtoMapping
{
    public static CollectRequestDetailDto ToDetailDto(this CollectRequest entity)
    {
        return new CollectRequestDetailDto
        {
            Request = entity.ToDto(),
            Location = entity.Location.ToDto(),
            Details = entity.RequestDetails.Select(x => x.ToDto()).ToList(),
            PickupService = entity.PickupService?.ToDto()
        };
    }
}
