using TrackTrash.Domain.Entities;

namespace TrackTrash.Domain.Ports.Repositories;

public interface IOrganizationRepository : IGenericRepository<Organization>
{
    Task<Organization?> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default);
}
