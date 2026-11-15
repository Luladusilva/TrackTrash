using TrackTrash.Domain.Entities;
using TrackTrash.Domain.Ports.Repositories;
using TrackTrash.Infraestructure.Context;

namespace TrackTrash.Infraestructure.Adapters.Repositories;

public class PickupServiceRepository(TrackTrashDBContext context) : GenericRepository<PickupService>(context), IPickupServiceRepository
{
}
