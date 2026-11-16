using System.ComponentModel.DataAnnotations;
using MediatR;
using TrackTrash.Domain.Enums;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.Organizations.Commands;

public sealed record UpdateOrganizationCommand : IRequest<long>
{
    public long OrganizationId { get; set; }

    [Required, StringLength(180)]
    public string BusinessName { get; set; } = null!;

    [StringLength(11, MinimumLength = 11)]
    public string? TaxId { get; set; }

    [EnumDataType(typeof(OrganizationType))]
    public OrganizationType OrganizationTypeId { get; set; }

    public long? LocationId { get; set; }

    [EnumDataType(typeof(SubscriptionPlan))]
    public SubscriptionPlan SubscriptionPlanId { get; set; }

    public bool IsActive { get; set; }
}

internal sealed class UpdateOrganizationCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateOrganizationCommand, long>
{
    public async Task<long> Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.Organizations.GetByIdAsync(request.OrganizationId, cancellationToken)
                   ?? throw new KeyNotFoundException("Organization not found.");

        if (request.LocationId is not null
            && !await unitOfWork.Locations.ExistsAsync(x => x.LocationId == request.LocationId, cancellationToken))
        {
            throw new KeyNotFoundException("Location not found.");
        }

        if (request.TaxId is not null
            && await unitOfWork.Organizations.ExistsAsync(x => x.TaxId == request.TaxId && x.OrganizationId != request.OrganizationId, cancellationToken))
        {
            throw new InvalidOperationException("An organization with the same tax id already exists.");
        }

        data.BusinessName = request.BusinessName;
        data.TaxId = request.TaxId;
        data.OrganizationTypeId = request.OrganizationTypeId;
        data.LocationId = request.LocationId;
        data.SubscriptionPlanId = request.SubscriptionPlanId;
        data.IsActive = request.IsActive;

        unitOfWork.Organizations.Update(data);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return data.OrganizationId;
    }
}
