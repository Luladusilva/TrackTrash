using System.ComponentModel.DataAnnotations;
using TrackTrash.Application.UseCases.Locations.Commands;
using TrackTrash.Domain.Enums;

namespace TrackTrash.Application.Dtos;

public sealed record RegisterDto
{
    [Required, StringLength(100)]
    public required string FirstName { get; init; }

    [Required, StringLength(100)]
    public required string LastName { get; init; }

    [Required, EmailAddress, StringLength(150)]
    public required string Email { get; init; }

    [Required, StringLength(100, MinimumLength = 8)]
    public required string Password { get; init; }

    [StringLength(20)]
    public string? Phone { get; init; }

    public UserRole RoleId { get; init; } = UserRole.Generator;

    [Required]
    public required CreateLocationCommand Location { get; init; }

    public RegisterOrganizationDto? Organization { get; init; }

    public RegisterGathererDto? Gatherer { get; init; }
}

public sealed record RegisterOrganizationDto
{
    [Required, StringLength(180)]
    public required string BusinessName { get; init; }

    [StringLength(11, MinimumLength = 11)]
    public string? TaxId { get; init; }

    [EnumDataType(typeof(OrganizationType))]
    public required OrganizationType OrganizationTypeId { get; init; }
}

public sealed record RegisterGathererDto
{
    [EnumDataType(typeof(GathererType))]
    public required GathererType GathererTypeId { get; init; }

    public string? Description { get; init; }

    [StringLength(50)]
    public string? VerificationDocument { get; init; }
}
