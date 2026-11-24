using MediatR;
using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Enums;
using TrackTrash.Domain.Ports.UnitOfWork;

namespace TrackTrash.Application.UseCases.CollectRequests.Commands;

public sealed record ApplyForCollectRequestCommand(long CollectRequestId, long UserId) : IRequest<long>;

internal sealed class ApplyForCollectRequestCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ApplyForCollectRequestCommand, long>
{
    public async Task<long> Handle(ApplyForCollectRequestCommand request, CancellationToken cancellationToken)
    {
        var gatherer = await unitOfWork.Gatherers.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new UnauthorizedAccessException("A gatherer profile is required to apply.");

        if (!gatherer.IsActive || !gatherer.User.IsActive || gatherer.VerificationStatusId != VerificationStatus.Verified)
            throw new UnauthorizedAccessException("Only active, verified gatherers can apply.");

        var collectRequest = await unitOfWork.CollectRequests.GetWithDetailsAsync(request.CollectRequestId, cancellationToken)
            ?? throw new KeyNotFoundException("Collect request not found.");

        if (collectRequest.StatusId != CollectRequestStatus.Pending || collectRequest.PickupService is not null)
            throw new InvalidOperationException("This collect request is no longer accepting applications.");

        if (collectRequest.RequestDetails.Count == 0)
            throw new InvalidOperationException("The collect request has no residue details yet.");

        var supportedResidueIds = gatherer.ResidueTypes.Select(x => x.ResidueTypeId).ToHashSet();
        if (collectRequest.RequestDetails.Any(x => !supportedResidueIds.Contains(x.ResidueTypeId)))
            throw new InvalidOperationException("You must support every residue type in the collect request to apply.");

        if (await unitOfWork.CollectRequestApplications.GetByRequestAndGathererAsync(
                request.CollectRequestId, gatherer.GathererId, cancellationToken) is not null)
            throw new InvalidOperationException("You have already applied to this collect request.");

        var application = new CollectRequestApplication
        {
            CollectRequestId = collectRequest.CollectRequestId,
            GathererId = gatherer.GathererId,
            StatusId = PickupApplicationStatus.Pending,
            AppliedAt = DateTime.UtcNow
        };
        await unitOfWork.CollectRequestApplications.AddAsync(application, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return application.CollectRequestApplicationId;
    }
}
