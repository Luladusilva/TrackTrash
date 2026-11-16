using MediatR;
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
}

internal sealed class CreateCollectRequestCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateCollectRequestCommand, long>
{
    public async Task<long> Handle(CreateCollectRequestCommand request, CancellationToken cancellationToken)
    {
        if (!await unitOfWork.Users.ExistsAsync(x => x.UserId == request.UserId, cancellationToken))
        {
            throw new KeyNotFoundException("User not found.");
        }

        if (!await unitOfWork.Locations.ExistsAsync(x => x.LocationId == request.LocationId, cancellationToken))
        {
            throw new KeyNotFoundException("Location not found.");
        }

        if (request.OrganizationId is not null
            && !await unitOfWork.Organizations.ExistsAsync(x => x.OrganizationId == request.OrganizationId, cancellationToken))
        {
            throw new KeyNotFoundException("Organization not found.");
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
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return data.CollectRequestId;
    }
}
