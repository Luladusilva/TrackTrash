using System.ComponentModel.DataAnnotations;
using MediatR;
using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Enums;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.Organizations.Commands;

public sealed record CreateOrganizationCommand : IRequest<long>
{
    public long UserId { get; set; }

    [Required, StringLength(180)]
    public string BusinessName { get; set; } = null!;

    [StringLength(11, MinimumLength = 11)]
    public string? TaxId { get; set; }

    [EnumDataType(typeof(OrganizationType))]
    public OrganizationType OrganizationTypeId { get; set; }

    public long? LocationId { get; set; }

    [EnumDataType(typeof(SubscriptionPlan))]
    public SubscriptionPlan SubscriptionPlanId { get; set; } = SubscriptionPlan.Basic;
}

internal sealed class CreateOrganizationCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateOrganizationCommand, long>
{
    public async Task<long> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
    {
        if (!await unitOfWork.Users.ExistsAsync(x => x.UserId == request.UserId, cancellationToken))
        {
            throw new KeyNotFoundException("User not found.");
        }

        if (request.LocationId is not null
            && !await unitOfWork.Locations.ExistsAsync(x => x.LocationId == request.LocationId, cancellationToken))
        {
            throw new KeyNotFoundException("Location not found.");
        }

        if (await unitOfWork.Organizations.ExistsAsync(x => x.UserId == request.UserId, cancellationToken))
        {
            throw new InvalidOperationException("The user already has an organization.");
        }

        if (request.TaxId is not null
            && await unitOfWork.Organizations.ExistsAsync(x => x.TaxId == request.TaxId, cancellationToken))
        {
            throw new InvalidOperationException("An organization with the same tax id already exists.");
        }

        var data = new Organization
        {
            UserId = request.UserId,
            BusinessName = request.BusinessName,
            TaxId = request.TaxId,
            OrganizationTypeId = request.OrganizationTypeId,
            LocationId = request.LocationId,
            SubscriptionPlanId = request.SubscriptionPlanId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await unitOfWork.Organizations.AddAsync(data, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return data.OrganizationId;
    }
}
