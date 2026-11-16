using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Ports.Repositories;
using TrackTrash.Infraestructure.Context;

namespace TrackTrash.Infraestructure.Adapters.Repositories;

public class CollectionPointRepository(TrackTrashDBContext context) : GenericRepository<CollectionPoint>(context), ICollectionPointRepository
{
}
