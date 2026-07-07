using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Infrastructure.Data;

namespace NaarNoor.Infrastructure.Repositories;

public class Repository<TEntity> : IRepository<TEntity>
    where TEntity : class
{
    private readonly ApplicationDbContext _context;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IQueryable<TEntity> Query()
        => _context.Set<TEntity>();

    public Task<TEntity?> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
        => _context.Set<TEntity>().FirstOrDefaultAsync(predicate, cancellationToken);

    public void Add(TEntity entity)
        => _context.Set<TEntity>().Add(entity);

    public void Remove(TEntity entity)
        => _context.Set<TEntity>().Remove(entity);

    public void Update(TEntity entity)
        => _context.Set<TEntity>().Update(entity);
}
