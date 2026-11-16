using TrackTrash.Domain.Ports.Repositories;
using TrackTrash.Infraestructure.Context;

namespace TrackTrash.Infraestructure.Adapters.Repositories;

public class IndicatorRepository(TrackTrashDBContext context) : IIndicatorRepository
{
    protected TrackTrashDBContext Context { get; } = context;
}
