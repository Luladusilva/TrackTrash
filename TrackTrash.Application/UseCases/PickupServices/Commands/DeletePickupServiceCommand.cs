using MediatR;
using TrackTrash.Application.UseCases.CollectRequests;
using TrackTrash.Domain.Enums;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.PickupServices.Commands;

/// <summary>
/// Removes a pickup service that has not started yet and returns its request to Pending.
/// </summary>
public sealed record DeletePickupServiceCommand : IRequest<bool>
{
    public long PickupServiceId { get; set; }
}

internal sealed class DeletePickupServiceCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeletePickupServiceCommand, bool>
{
    public async Task<bool> Handle(DeletePickupServiceCommand request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.PickupServices.GetByIdAsync(request.PickupServiceId, cancellationToken)
                   ?? throw new KeyNotFoundException("Pickup service not found.");

        if (data.StatusId is not (PickupServiceStatus.Pending or PickupServiceStatus.Assigned))
        {
            throw new InvalidOperationException("Only pickup services that have not started can be deleted.");
        }

        var collectRequest = await unitOfWork.CollectRequests.GetByIdAsync(data.CollectRequestId, cancellationToken);
        if (collectRequest is not null && CollectRequestStatusRules.CanChange(collectRequest.StatusId, CollectRequestStatus.Pending))
        {
            await CollectRequestStatusRules.ApplyAsync(
                unitOfWork, collectRequest, CollectRequestStatus.Pending, null, "Pickup service removed.", cancellationToken);
        }

        unitOfWork.PickupServices.Remove(data);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
