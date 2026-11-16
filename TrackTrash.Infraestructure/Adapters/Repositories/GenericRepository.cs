using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TrackTrash.Domain.Ports.Repositories;
using TrackTrash.Infraestructure.Context;

namespace TrackTrash.Infraestructure.Adapters.Repositories;

public class GenericRepository<T>(TrackTrashDBContext context) : IGenericRepository<T> where T : class
{
    protected TrackTrashDBContext Context { get; } = context;

    protected DbSet<T> Set { get; } = context.Set<T>();

    public async Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await Set.FindAsync([id], cancellationToken);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Set.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await Set.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await Set.AnyAsync(predicate, cancellationToken);
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await Set.AddAsync(entity, cancellationToken);
    }

    public void Update(T entity)
    {
        Set.Update(entity);
    }

    public void Remove(T entity)
    {
        Set.Remove(entity);
    }
}
