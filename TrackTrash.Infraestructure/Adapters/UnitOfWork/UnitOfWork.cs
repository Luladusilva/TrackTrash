using TrackTrash.Domain.Ports.Repositories;
using TrackTrash.Domain.Ports.UnitOfWork;
using TrackTrash.Infraestructure.Context;

namespace TrackTrash.Infraestructure.Adapters.UnitOfWork;

public class UnitOfWork(
    TrackTrashDBContext context,
    IUserRepository users,
    ILocationRepository locations,
    IOrganizationRepository organizations,
    IGathererRepository gatherers,
    IResidueTypeRepository residueTypes,
    ICollectionPointRepository collectionPoints,
    ICollectRequestRepository collectRequests,
    IRequestDetailRepository requestDetails,
    IRequestHistoryRepository requestHistories,
    IPickupServiceRepository pickupServices,
    IRecoveredMaterialRepository recoveredMaterials,
    IGradeRepository grades,
    IGathererGradeRepository gathererGrades,
    IIndicatorRepository indicators,
    ICollectRequestApplicationRepository collectRequestApplications) : IUnitOfWork
{
    public IUserRepository Users { get; } = users;

    public ILocationRepository Locations { get; } = locations;

    public IOrganizationRepository Organizations { get; } = organizations;

    public IGathererRepository Gatherers { get; } = gatherers;

    public IResidueTypeRepository ResidueTypes { get; } = residueTypes;

    public ICollectionPointRepository CollectionPoints { get; } = collectionPoints;

    public ICollectRequestRepository CollectRequests { get; } = collectRequests;

    public IRequestDetailRepository RequestDetails { get; } = requestDetails;

    public IRequestHistoryRepository RequestHistories { get; } = requestHistories;

    public IPickupServiceRepository PickupServices { get; } = pickupServices;

    public IRecoveredMaterialRepository RecoveredMaterials { get; } = recoveredMaterials;

    public IGradeRepository Grades { get; } = grades;

    public IGathererGradeRepository GathererGrades { get; } = gathererGrades;

    public IIndicatorRepository Indicators { get; } = indicators;

    public ICollectRequestApplicationRepository CollectRequestApplications { get; } = collectRequestApplications;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}
