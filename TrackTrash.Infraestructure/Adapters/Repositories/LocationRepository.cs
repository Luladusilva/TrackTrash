using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Ports.Repositories;
using TrackTrash.Infraestructure.Context;

namespace TrackTrash.Infraestructure.Adapters.Repositories;

public class LocationRepository(TrackTrashDBContext context) : GenericRepository<Location>(context), ILocationRepository
{
}
