using TrackTrash.Domain.Entities;

namespace TrackTrash.Application.Dtos;

public sealed record GradeDto
{
    public long GradeId { get; init; }

    public long PickupServiceId { get; init; }

    public long UserId { get; init; }

    public short Score { get; init; }

    public string? Comment { get; init; }

    public DateTime CreatedAt { get; init; }
}

internal static class GradeDtoMapping
{
    public static GradeDto ToDto(this Grade entity)
    {
        return new GradeDto
        {
            GradeId = entity.GradeId,
            PickupServiceId = entity.PickupServiceId,
            UserId = entity.UserId,
            Score = entity.Score,
            Comment = entity.Comment,
            CreatedAt = entity.CreatedAt
        };
    }
}
