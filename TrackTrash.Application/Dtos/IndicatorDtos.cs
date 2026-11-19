using TrackTrash.Domain.Entities;

namespace TrackTrash.Application.Dtos;

public sealed record GeneralIndicatorDto
{
    public long TotalUsers { get; init; }

    public long TotalOrganizations { get; init; }

    public long VerifiedGatherers { get; init; }

    public long TotalRequests { get; init; }

    public long CompletedRequests { get; init; }

    public long CompletedServices { get; init; }

    public decimal RecoveredKilograms { get; init; }
}

public sealed record ResiduesByTypeDto
{
    public long? ResidueTypeId { get; init; }

    public string? ResidueType { get; init; }

    public string? Category { get; init; }

    public string? Unit { get; init; }

    public decimal RecoveredQuantity { get; init; }
}

public sealed record GathererRatingDto
{
    public long? GathererId { get; init; }

    public string? FirstName { get; init; }

    public string? LastName { get; init; }

    public decimal AverageScore { get; init; }

    public long TotalGrades { get; init; }
}

internal static class IndicatorDtoMapping
{
    public static GeneralIndicatorDto ToDto(this VwGeneralIndicator entity)
    {
        return new GeneralIndicatorDto
        {
            TotalUsers = entity.TotalUsers ?? 0,
            TotalOrganizations = entity.TotalOrganizations ?? 0,
            VerifiedGatherers = entity.VerifiedGatherers ?? 0,
            TotalRequests = entity.TotalRequests ?? 0,
            CompletedRequests = entity.CompletedRequests ?? 0,
            CompletedServices = entity.CompletedServices ?? 0,
            RecoveredKilograms = entity.RecoveredKilograms ?? 0
        };
    }

    public static ResiduesByTypeDto ToDto(this VwResiduesByType entity)
    {
        return new ResiduesByTypeDto
        {
            ResidueTypeId = entity.ResidueTypeId,
            ResidueType = entity.ResidueType,
            Category = entity.Category,
            Unit = entity.Unit,
            RecoveredQuantity = entity.RecoveredQuantity ?? 0
        };
    }

    public static GathererRatingDto ToDto(this VwGathererGrade entity)
    {
        return new GathererRatingDto
        {
            GathererId = entity.GathererId,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            AverageScore = entity.AverageScore ?? 0,
            TotalGrades = entity.TotalGrades ?? 0
        };
    }
}
