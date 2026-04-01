using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracking.Infrastructure.Persistence.Repositories;

public abstract class Repository<TEntity, TId> : IRepository<TEntity, TId>
    where TEntity : class, IDocument
{
    private readonly Func<TId, string> _idConverter;

    protected Repository(
        FitnessTrackingDbContext dbContext,
        DbSet<TEntity> entities,
        Func<TId, string>? idConverter = null)
    {
        DbContext = dbContext;
        Entities = entities;
        _idConverter = idConverter ?? (id => id?.ToString() ?? string.Empty);
    }

    protected FitnessTrackingDbContext DbContext { get; }

    protected DbSet<TEntity> Entities { get; }

    public virtual async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken)
    {
        return await Entities.FindAsync([_idConverter(id)], cancellationToken);
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        await Entities.AddAsync(entity, cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public virtual async Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        var exists = await Entities.AnyAsync(e => e.Id == entity.Id, cancellationToken);
        if (!exists)
        {
            return false;
        }

        Entities.Update(entity);
        await DbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public virtual async Task<bool> DeleteAsync(TId id, CancellationToken cancellationToken)
    {
        var entity = await Entities.FindAsync([_idConverter(id)], cancellationToken);
        if (entity is null)
        {
            return false;
        }

        Entities.Remove(entity);
        await DbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}

