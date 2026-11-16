using TrackTrash.Domain.Ports.Repositories;

namespace TrackTrash.Domain.Ports.UnitOfWork;

public interface IUnitOfWork
{
    IUserRepository Users { get; }

    ILocationRepository Locations { get; }

    IOrganizationRepository Organizations { get; }

    IGathererRepository Gatherers { get; }

    IResidueTypeRepository ResidueTypes { get; }

    ICollectionPointRepository CollectionPoints { get; }

    ICollectRequestRepository CollectRequests { get; }

    IRequestDetailRepository RequestDetails { get; }

    IRequestHistoryRepository RequestHistories { get; }

    IPickupServiceRepository PickupServices { get; }

    IRecoveredMaterialRepository RecoveredMaterials { get; }

    IGradeRepository Grades { get; }

    IGathererGradeRepository GathererGrades { get; }

    IIndicatorRepository Indicators { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
