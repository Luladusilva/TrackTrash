using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Enums;

namespace TrackTrash.Application.Dtos;

public sealed record RequestHistoryDto
{
    public long RequestHistoryId { get; init; }

    public long CollectRequestId { get; init; }

    public long? UserId { get; init; }

    public CollectRequestStatus? PreviousStatusId { get; init; }

    public CollectRequestStatus NewStatusId { get; init; }

    public string? Observations { get; init; }

    public DateTime CreatedAt { get; init; }
}

internal static class RequestHistoryDtoMapping
{
    public static RequestHistoryDto ToDto(this RequestHistory entity)
    {
        return new RequestHistoryDto
        {
            RequestHistoryId = entity.RequestHistoryId,
            CollectRequestId = entity.CollectRequestId,
            UserId = entity.UserId,
            PreviousStatusId = entity.PreviousStatusId,
            NewStatusId = entity.NewStatusId,
            Observations = entity.Observations,
            CreatedAt = entity.CreatedAt
        };
    }
}
