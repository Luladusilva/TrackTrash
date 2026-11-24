using System.ComponentModel.DataAnnotations;
using MediatR;
using TrackTrash.Domain.Enums;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.PickupServices.Commands;

public sealed record UpdatePickupServiceCommand : IRequest<long>
{
    public long PickupServiceId { get; set; }

    [Range(typeof(decimal), "0", "100")]
    public decimal CommissionPercentage { get; set; }

    public string? Observations { get; set; }
}

internal sealed class UpdatePickupServiceCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdatePickupServiceCommand, long>
{
    public async Task<long> Handle(UpdatePickupServiceCommand request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.PickupServices.GetByIdAsync(request.PickupServiceId, cancellationToken)
                   ?? throw new KeyNotFoundException("Pickup service not found.");

        if (data.StatusId is PickupServiceStatus.Completed or PickupServiceStatus.Cancelled)
        {
            throw new InvalidOperationException("Finished pickup services cannot be edited.");
        }

        data.CommissionPercentage = request.CommissionPercentage;
        data.Observations = request.Observations;

        unitOfWork.PickupServices.Update(data);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return data.PickupServiceId;
    }
}
