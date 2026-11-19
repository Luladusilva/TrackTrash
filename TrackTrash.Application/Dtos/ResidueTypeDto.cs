using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Enums;

namespace TrackTrash.Application.Dtos;

public sealed record ResidueTypeDto
{
    public long ResidueTypeId { get; init; }

    public string Name { get; init; } = null!;

    public ResidueCategory ResidueCategoryId { get; init; }

    public string? Description { get; init; }

    public bool IsRecyclable { get; init; }

    public string? ClassificationInstructions { get; init; }

    public bool IsActive { get; init; }

    public DateTime CreatedAt { get; init; }
}

internal static class ResidueTypeDtoMapping
{
    public static ResidueTypeDto ToDto(this ResidueType entity)
    {
        return new ResidueTypeDto
        {
            ResidueTypeId = entity.ResidueTypeId,
            Name = entity.Name,
            ResidueCategoryId = entity.ResidueCategoryId,
            Description = entity.Description,
            IsRecyclable = entity.IsRecyclable,
            ClassificationInstructions = entity.ClassificationInstructions,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt
        };
    }
}
