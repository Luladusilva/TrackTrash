using MediatR;
using System.ComponentModel.DataAnnotations;
using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Enums;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.CollectRequests.Commands;

public sealed record CreateCollectRequestCommand : IRequest<long>
{
    public long UserId { get; set; }

    public long? OrganizationId { get; set; }

    public long LocationId { get; set; }

    public DateOnly ScheduledDate { get; set; }

    public TimeOnly ScheduledTime { get; set; }

    public string? Observations { get; set; }

    [Required, MinLength(1)]
    public List<CreateCollectRequestDetailDto> Details { get; set; } = [];
}

public sealed record CreateCollectRequestDetailDto
{
    [Range(1, long.MaxValue)]
    public long ResidueTypeId { get; init; }

    [Range(typeof(decimal), "0.01", "100000")]
    public decimal EstimatedQuantity { get; init; }

    public MeasurementUnit UnitId { get; init; } = MeasurementUnit.Kg;

    public string? Observations { get; init; }
}

internal sealed class CreateCollectRequestCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateCollectRequestCommand, long>
{
    public async Task<long> Handle(CreateCollectRequestCommand request, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null || !user.IsActive)
        {
            throw new KeyNotFoundException("User not found.");
        }

        if (!await unitOfWork.Locations.ExistsAsync(x => x.LocationId == request.LocationId, cancellationToken))
        {
            throw new KeyNotFoundException("Location not found.");
        }

        var organization = await unitOfWork.Organizations.GetByUserIdAsync(request.UserId, cancellationToken);
        if (user.RoleId == UserRole.Organization)
        {
            if (organization is null || !organization.IsActive)
                throw new UnauthorizedAccessException("An active organization profile is required to create requests.");
            if (request.OrganizationId is not null && request.OrganizationId != organization.OrganizationId)
                throw new UnauthorizedAccessException("The organization does not belong to the authenticated user.");
            request.OrganizationId = organization.OrganizationId;
        }
        else if (request.OrganizationId is not null)
        {
            throw new UnauthorizedAccessException("Only an organization account can create a request for an organization.");
        }

        if (request.Details.Count == 0 || request.Details.Select(x => x.ResidueTypeId).Distinct().Count() != request.Details.Count)
            throw new InvalidOperationException("A request must contain unique residue types.");

        foreach (var detail in request.Details)
        {
            var residueType = await unitOfWork.ResidueTypes.GetByIdAsync(detail.ResidueTypeId, cancellationToken);
            if (residueType is null || !residueType.IsActive || !residueType.IsRecyclable)
                throw new InvalidOperationException($"Residue type {detail.ResidueTypeId} is not available for collection.");
        }

        if (request.ScheduledDate < DateOnly.FromDateTime(DateTime.UtcNow))
        {
            throw new InvalidOperationException("The scheduled date cannot be in the past.");
        }

        var now = DateTime.UtcNow;

        var data = new CollectRequest
        {
            UserId = request.UserId,
            OrganizationId = request.OrganizationId,
            LocationId = request.LocationId,
            RequestedAt = now,
            ScheduledDate = request.ScheduledDate,
            ScheduledTime = request.ScheduledTime,
            StatusId = CollectRequestStatus.Pending,
            Observations = request.Observations,
            UpdatedAt = now
        };

        await unitOfWork.CollectRequests.AddAsync(data, cancellationToken);
        foreach (var detail in request.Details)
        {
            await unitOfWork.RequestDetails.AddAsync(new RequestDetail
            {
                CollectRequest = data,
                ResidueTypeId = detail.ResidueTypeId,
                EstimatedQuantity = detail.EstimatedQuantity,
                UnitId = detail.UnitId,
                Observations = detail.Observations
            }, cancellationToken);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return data.CollectRequestId;
    }
}
