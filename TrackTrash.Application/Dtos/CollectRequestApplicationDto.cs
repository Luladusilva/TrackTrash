using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Enums;

namespace TrackTrash.Application.Dtos;

public sealed record CollectRequestApplicationDto
{
    public long ApplicationId { get; init; }
    public long CollectRequestId { get; init; }
    public long GathererId { get; init; }
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string? Description { get; init; }
    public VerificationStatus VerificationStatusId { get; init; }
    public PickupApplicationStatus StatusId { get; init; }
    public DateTime AppliedAt { get; init; }
    public DateTime? RespondedAt { get; init; }
}

internal static class CollectRequestApplicationDtoMapping
{
    public static CollectRequestApplicationDto ToDto(this CollectRequestApplication entity) => new()
    {
        ApplicationId = entity.CollectRequestApplicationId,
        CollectRequestId = entity.CollectRequestId,
        GathererId = entity.GathererId,
        FirstName = entity.Gatherer.User.FirstName,
        LastName = entity.Gatherer.User.LastName,
        Description = entity.Gatherer.Description,
        VerificationStatusId = entity.Gatherer.VerificationStatusId,
        StatusId = entity.StatusId,
        AppliedAt = entity.AppliedAt,
        RespondedAt = entity.RespondedAt
    };
}
