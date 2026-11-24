using Microsoft.EntityFrameworkCore;
using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Enums;
using TrackTrash.Domain.Ports.Repositories;
using TrackTrash.Infraestructure.Context;

namespace TrackTrash.Infraestructure.Adapters.Repositories;

public sealed class CollectRequestApplicationRepository(TrackTrashDBContext context)
    : GenericRepository<CollectRequestApplication>(context), ICollectRequestApplicationRepository
{
    public Task<CollectRequestApplication?> GetByRequestAndGathererAsync(
        long requestId,
        long gathererId,
        CancellationToken cancellationToken = default) =>
        Set.SingleOrDefaultAsync(x => x.CollectRequestId == requestId && x.GathererId == gathererId, cancellationToken);

    public async Task<IReadOnlyList<CollectRequestApplication>> GetByRequestAsync(
        long requestId,
        CancellationToken cancellationToken = default) =>
        await Set.AsNoTracking()
            .Include(x => x.Gatherer).ThenInclude(x => x.User)
            .Where(x => x.CollectRequestId == requestId)
            .OrderBy(x => x.StatusId).ThenBy(x => x.AppliedAt)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<CollectRequestApplication>> GetByGathererUserIdAsync(
        long userId,
        CancellationToken cancellationToken = default) =>
        await Set.AsNoTracking()
            .Include(x => x.Gatherer).ThenInclude(x => x.User)
            .Where(x => x.Gatherer.UserId == userId)
            .OrderByDescending(x => x.AppliedAt)
            .ToListAsync(cancellationToken);

    public async Task<long> AcceptAsync(
        long requestId,
        long applicationId,
        long actorUserId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await Context.Database.BeginTransactionAsync(cancellationToken);
        var request = await Context.CollectRequests
            .Include(x => x.Organization)
            .Include(x => x.RequestDetails).ThenInclude(x => x.ResidueType)
            .Include(x => x.PickupService)
            .SingleOrDefaultAsync(x => x.CollectRequestId == requestId, cancellationToken)
            ?? throw new KeyNotFoundException("Collect request not found.");

        if (!isAdmin && request.UserId != actorUserId && request.Organization?.UserId != actorUserId)
            throw new UnauthorizedAccessException("You cannot accept applications for this collect request.");

        if (request.StatusId != CollectRequestStatus.Pending || request.PickupService is not null)
            throw new InvalidOperationException("This collect request is no longer accepting applications.");

        var application = await Set
            .Include(x => x.Gatherer).ThenInclude(x => x.User)
            .Include(x => x.Gatherer).ThenInclude(x => x.ResidueTypes)
            .SingleOrDefaultAsync(x => x.CollectRequestApplicationId == applicationId && x.CollectRequestId == requestId, cancellationToken)
            ?? throw new KeyNotFoundException("Application not found.");

        if (application.StatusId != PickupApplicationStatus.Pending)
            throw new InvalidOperationException("This application has already been answered.");

        if (!application.Gatherer.IsActive || !application.Gatherer.User.IsActive ||
            application.Gatherer.VerificationStatusId != VerificationStatus.Verified)
            throw new InvalidOperationException("Only active, verified gatherers can be assigned.");

        var supportedResidueIds = application.Gatherer.ResidueTypes.Select(x => x.ResidueTypeId).ToHashSet();
        if (request.RequestDetails.Count == 0 || request.RequestDetails.Any(x => !supportedResidueIds.Contains(x.ResidueTypeId)))
            throw new InvalidOperationException("The gatherer does not support every residue type in this request.");

        var now = DateTime.UtcNow;
        application.StatusId = PickupApplicationStatus.Accepted;
        application.RespondedAt = now;

        var otherApplications = await Set
            .Where(x => x.CollectRequestId == requestId && x.CollectRequestApplicationId != applicationId && x.StatusId == PickupApplicationStatus.Pending)
            .ToListAsync(cancellationToken);
        foreach (var other in otherApplications)
        {
            other.StatusId = PickupApplicationStatus.Rejected;
            other.RespondedAt = now;
        }

        request.StatusId = CollectRequestStatus.Assigned;
        request.UpdatedAt = now;
        var pickupService = new PickupService
        {
            CollectRequestId = requestId,
            GathererId = application.GathererId,
            AssignedAt = now,
            StatusId = PickupServiceStatus.Assigned,
            CommissionPercentage = 10m
        };
        var history = new RequestHistory
        {
            CollectRequestId = requestId,
            UserId = actorUserId,
            PreviousStatusId = CollectRequestStatus.Pending,
            NewStatusId = CollectRequestStatus.Assigned,
            Observations = "Gatherer application accepted.",
            CreatedAt = now
        };

        await Context.PickupServices.AddAsync(pickupService, cancellationToken);
        await Context.RequestHistories.AddAsync(history, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return pickupService.PickupServiceId;
    }

    public async Task RejectAsync(
        long requestId,
        long applicationId,
        long actorUserId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var application = await Set
            .Include(x => x.CollectRequest).ThenInclude(x => x.Organization)
            .SingleOrDefaultAsync(x => x.CollectRequestApplicationId == applicationId && x.CollectRequestId == requestId, cancellationToken)
            ?? throw new KeyNotFoundException("Application not found.");

        if (!isAdmin && application.CollectRequest.UserId != actorUserId && application.CollectRequest.Organization?.UserId != actorUserId)
            throw new UnauthorizedAccessException("You cannot reject applications for this collect request.");

        if (application.StatusId != PickupApplicationStatus.Pending)
            throw new InvalidOperationException("This application has already been answered.");

        application.StatusId = PickupApplicationStatus.Rejected;
        application.RespondedAt = DateTime.UtcNow;
        await Context.SaveChangesAsync(cancellationToken);
    }
}
