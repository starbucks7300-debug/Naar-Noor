using System.Linq.Expressions;

namespace NaarNoor.Application.Common.Interfaces;

public interface IRepository<TEntity>
    where TEntity : class
{
    IQueryable<TEntity> Query();

    /// <summary>
    /// Returns the first entity matching the predicate, or null.
    /// Use for single-entity lookups in commands (update / delete).
    /// For projected reads use Query().Where(…).Select(…).FirstOrDefaultAsync(…).
    /// </summary>
    Task<TEntity?> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    void Add(TEntity entity);
    void Remove(TEntity entity);
    void Update(TEntity entity);
}
