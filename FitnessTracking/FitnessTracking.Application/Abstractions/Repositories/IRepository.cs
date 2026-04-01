using CSharpFunctionalExtensions;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Abstractions.Repositories;

public interface IRepository<TEntity, in TId>
{
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken);

    Task<UnitResult<Error>> AddAsync(TEntity entity, CancellationToken cancellationToken);

    Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(TId id, CancellationToken cancellationToken);
}