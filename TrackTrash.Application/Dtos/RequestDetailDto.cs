using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Enums;

namespace TrackTrash.Application.Dtos;

public sealed record RequestDetailDto
{
    public long RequestDetailId { get; init; }

    public long CollectRequestId { get; init; }

    public long ResidueTypeId { get; init; }

    public decimal EstimatedQuantity { get; init; }

    public MeasurementUnit UnitId { get; init; }

    public string? Observations { get; init; }
}

internal static class RequestDetailDtoMapping
{
    public static RequestDetailDto ToDto(this RequestDetail entity)
    {
        return new RequestDetailDto
        {
            RequestDetailId = entity.RequestDetailId,
            CollectRequestId = entity.CollectRequestId,
            ResidueTypeId = entity.ResidueTypeId,
            EstimatedQuantity = entity.EstimatedQuantity,
            UnitId = entity.UnitId,
            Observations = entity.Observations
        };
    }
}
