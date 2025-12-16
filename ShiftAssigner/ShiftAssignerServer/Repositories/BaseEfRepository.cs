using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ShiftAssignerServer.Data;
using ShiftAssignerServer.Models;

namespace ShiftAssignerServer.Repositories;

public interface IRepositoryBase<T> where T : class, IActiveEntity
{
    // CREATE
    Task<T> InsertAsync(T entity, CancellationToken cancellationToken = default);
    T Insert(T entity);

    // READ
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    T? FirstOrDefault(Expression<Func<T, bool>> predicate);

    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    IEnumerable<T> GetAll();

    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate);

    Task<IEnumerable<T>> GetAllActiveAsync(CancellationToken cancellationToken = default);

    // UPDATE
    Task<bool> UpdateAsync(Expression<Func<T, bool>> predicate, Action<T> update, CancellationToken cancellationToken = default);
    bool Update(Expression<Func<T, bool>> predicate, Action<T> update);

    bool HasDataBaseChanged{get;}

    // DELETE
    // Task<bool> DeleteAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    // bool Delete(Expression<Func<T, bool>> predicate);
}

public abstract class BaseRepository<T> : IRepositoryBase<T> where T : class, IActiveEntity
{
    protected readonly ApplicationDbContext Context;
    protected DbSet<T> DbSet => Context.Set<T>();
    private bool _hasDataBaseChanged = false;

    public bool HasDataBaseChanged => _hasDataBaseChanged;

    protected BaseRepository(ApplicationDbContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
    }

    // ---------------- CREATE ----------------
    public virtual async Task<T> InsertAsync(T entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await DbSet.AddAsync(entity, cancellationToken);
        _hasDataBaseChanged = true;
        return entity;
    }

    public virtual T Insert(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        DbSet.Add(entity);
        _hasDataBaseChanged = true;
        return entity;
    }

    // ---------------- READ ----------------
    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await DbSet.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public virtual T? FirstOrDefault(Expression<Func<T, bool>> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return DbSet.AsNoTracking().FirstOrDefault(predicate);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().ToListAsync(cancellationToken);
    }

    public virtual IEnumerable<T> GetAll()
    {
        return DbSet.AsNoTracking().ToList();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await DbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
    }

    public virtual IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return DbSet.AsNoTracking().Where(predicate).ToList();
    }

    public virtual async Task<IEnumerable<T>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().Where(x => x.IsActive).ToListAsync(cancellationToken);
    }

    // ---------------- UPDATE ----------------
    public virtual async Task<bool> UpdateAsync(
        Expression<Func<T, bool>> predicate,
        Action<T> update,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(update);

        var entity = await DbSet.FirstOrDefaultAsync(predicate, cancellationToken);
        if (entity == null)
            return false;

        update(entity);
        _hasDataBaseChanged = true;
        return true;
    }

    public virtual bool Update(Expression<Func<T, bool>> predicate, Action<T> update)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(update);

        var entity = DbSet.FirstOrDefault(predicate);
        if (entity == null)
            return false;

        update(entity);
        _hasDataBaseChanged = true;
        return true;
    }

    // ---------------- DELETE ----------------
    // public virtual async Task<bool> DeleteAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    // {
    //     ArgumentNullException.ThrowIfNull(predicate);

    //     var entity = await DbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    //     if (entity == null)
    //         return false;

    //     DbSet.Remove(entity);
    //     await Context.SaveChangesAsync(cancellationToken);
    //     return true;
    // }

    // public virtual bool Delete(Expression<Func<T, bool>> predicate)
    // {
    //     ArgumentNullException.ThrowIfNull(predicate);

    //     var entity = DbSet.FirstOrDefault(predicate);
    //     if (entity == null)
    //         return false;

    //     DbSet.Remove(entity);
    //     Context.SaveChanges();
    //     return true;
    // }
}
