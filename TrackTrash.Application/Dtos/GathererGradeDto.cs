using TrackTrash.Domain.Entities;

namespace TrackTrash.Application.Dtos;

public sealed record GathererGradeDto
{
    public long GathererGradeId { get; init; }

    public long GathererId { get; init; }

    public long GradeId { get; init; }
}

internal static class GathererGradeDtoMapping
{
    public static GathererGradeDto ToDto(this GathererGrade entity)
    {
        return new GathererGradeDto
        {
            GathererGradeId = entity.GathererGradeId,
            GathererId = entity.GathererId,
            GradeId = entity.GradeId
        };
    }
}
