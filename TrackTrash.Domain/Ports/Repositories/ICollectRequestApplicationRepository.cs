using TrackTrash.Domain.Entities;

namespace TrackTrash.Domain.Ports.Repositories;

public interface ICollectRequestApplicationRepository : IGenericRepository<CollectRequestApplication>
{
    Task<CollectRequestApplication?> GetByRequestAndGathererAsync(long requestId, long gathererId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CollectRequestApplication>> GetByRequestAsync(long requestId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CollectRequestApplication>> GetByGathererUserIdAsync(long userId, CancellationToken cancellationToken = default);

    Task<long> AcceptAsync(long requestId, long applicationId, long actorUserId, bool isAdmin, CancellationToken cancellationToken = default);

    Task RejectAsync(long requestId, long applicationId, long actorUserId, bool isAdmin, CancellationToken cancellationToken = default);
}
