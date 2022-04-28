using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DirectoryApi.Repositories;

public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;

    public BaseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IQueryable<T> FindAll()
    {
        return _context.Set<T>().AsNoTracking();
    }

    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
    {
        return _context.Set<T>().Where(expression).AsNoTracking();
    }

    public void Create(T entity)
    {
        _context.Set<T>().Add(entity);
    }

    public void Update(T entity)
    {
        _context.Set<T>().Update(entity);
    }

    public void Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }

    public virtual void DetachLocal(T entity, Func<T, bool> expression)
    {
        var local = _context.Set<T>()
                                .Local
                                .FirstOrDefault(expression);
        if (local != null)
        {
            _context.Entry(local).State = EntityState.Detached;
        }
    }
}