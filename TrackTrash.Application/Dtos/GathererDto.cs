using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Enums;

namespace TrackTrash.Application.Dtos;

public sealed record GathererDto
{
    public long GathererId { get; init; }

    public long UserId { get; init; }

    public GathererType GathererTypeId { get; init; }

    public string? Description { get; init; }

    public string? VerificationDocument { get; init; }

    public VerificationStatus VerificationStatusId { get; init; }

    public bool IsActive { get; init; }

    public DateTime CreatedAt { get; init; }
}

internal static class GathererDtoMapping
{
    public static GathererDto ToDto(this Gatherer entity)
    {
        return new GathererDto
        {
            GathererId = entity.GathererId,
            UserId = entity.UserId,
            GathererTypeId = entity.GathererTypeId,
            Description = entity.Description,
            VerificationDocument = entity.VerificationDocument,
            VerificationStatusId = entity.VerificationStatusId,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt
        };
    }
}
