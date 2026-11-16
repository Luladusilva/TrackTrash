using MediatR;
using TrackTrash.Domain.Enums;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.CollectRequests.Commands;

public sealed record UpdateCollectRequestCommand : IRequest<long>
{
    public long CollectRequestId { get; set; }

    public long LocationId { get; set; }

    public DateOnly ScheduledDate { get; set; }

    public TimeOnly ScheduledTime { get; set; }

    public string? Observations { get; set; }
}

internal sealed class UpdateCollectRequestCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateCollectRequestCommand, long>
{
    public async Task<long> Handle(UpdateCollectRequestCommand request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.CollectRequests.GetByIdAsync(request.CollectRequestId, cancellationToken)
                   ?? throw new KeyNotFoundException("Collect request not found.");

        if (data.StatusId != CollectRequestStatus.Pending)
        {
            throw new InvalidOperationException("Only pending collect requests can be updated.");
        }

        if (!await unitOfWork.Locations.ExistsAsync(x => x.LocationId == request.LocationId, cancellationToken))
        {
            throw new KeyNotFoundException("Location not found.");
        }

        if (request.ScheduledDate < DateOnly.FromDateTime(DateTime.UtcNow))
        {
            throw new InvalidOperationException("The scheduled date cannot be in the past.");
        }

        data.LocationId = request.LocationId;
        data.ScheduledDate = request.ScheduledDate;
        data.ScheduledTime = request.ScheduledTime;
        data.Observations = request.Observations;
        data.UpdatedAt = DateTime.UtcNow;

        unitOfWork.CollectRequests.Update(data);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return data.CollectRequestId;
    }
}
