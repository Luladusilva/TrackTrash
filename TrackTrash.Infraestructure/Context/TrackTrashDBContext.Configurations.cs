using Microsoft.EntityFrameworkCore;

namespace TrackTrash.Infraestructure.Context;

public partial class TrackTrashDBContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TrackTrashDBContext).Assembly);
    }
}
