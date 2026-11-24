using System.ComponentModel.DataAnnotations;

namespace TrackTrash.Application.Dtos;

public sealed record SetGathererResidueTypesDto
{
    [Required, MinLength(1)]
    public IReadOnlyList<long> ResidueTypeIds { get; init; } = [];
}
