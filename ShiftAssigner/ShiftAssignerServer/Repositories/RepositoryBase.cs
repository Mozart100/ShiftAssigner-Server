using System;
using ShiftAssignerServer.Services.Validation;

namespace ShiftAssignerServer.Repositories;

public interface IAutoMapperEntities
{

}

public interface IRepositoryBase<TModel> where TModel : IAutoMapperEntities 
{
    TModel Insert(TModel instance);

    Task<TModel> InsertAsync(TModel instance);



    IEnumerable<TModel> GetAll();

    Task<TModel> FirstOrDefualtAsync(Predicate<TModel> selector);


    Task<IEnumerable<TModel>> GetAllAsync(Func<TModel, bool> selector);

    Task<IEnumerable<TModel>> GetAllAsync();



    Task<bool> UpdateAsync(Predicate<TModel> selector, Action<TModel> updateCallback);
    bool Update(Predicate<TModel> selector, Action<TModel> updateCallback);
    TModel FirstOrDefault(Predicate<TModel> selector);
    IEnumerable<TModel> GetAll(Func<TModel, bool> selector);
}


public abstract class RepositoryBase<TModel> : IRepositoryBase<TModel> where TModel : IAutoMapperEntities 
{
    protected readonly HashSet<TModel> Models;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public RepositoryBase()
    {
        Models = new HashSet<TModel>();
    }

    public async Task<TModel> FirstOrDefualtAsync(Predicate<TModel> selector)
    {
        return await Task.FromResult(CoreGet(selector));
    }

    public TModel FirstOrDefault(Predicate<TModel> selector)
    {
        return CoreGet(selector);
    }


    protected virtual TModel CoreGet(Predicate<TModel> selector)
    {
        _semaphore.Wait();
        try
        {
            var result = default(TModel);
            foreach (var model in Models)
            {
                if (selector(model))
                {
                    result = model;
                    break;
                }
            }
            return result;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public virtual async Task<TModel> InsertAsync(TModel model)
    {
        return await Task.FromResult(Insert(model));
    }

    public async Task<IEnumerable<TModel>> GetAllAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            return Models.ToArray();
        }
        finally
        {
            _semaphore.Release();
        }
    }
    /// <summary>
    /// Auto Id Generator
    /// </summary>
    /// <param name="instance"></param>
    /// <returns></returns>
    public virtual TModel Insert(TModel instance)
    {
        _semaphore.Wait();
        try
        {
            if (Models.Add(instance) == false)
            {
                var error = new ShiftAssignmentError("ID", "A record with this ID already exists.");
                throw new ShiftAssignmentException(error);
            }
            return instance;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public virtual IEnumerable<TModel> GetAll()
    {
        _semaphore.Wait();
        try
        {
            return Models.ToArray();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<IEnumerable<TModel>> GetAllAsync(Func<TModel, bool> selector)
    {
        await _semaphore.WaitAsync();
        try
        {
            return Models.Where(x => selector(x)).ToArray();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public IEnumerable<TModel> GetAll(Func<TModel, bool> selector)
    {
        _semaphore.Wait();
        try
        {
            return Models.Where(x => selector(x)).ToArray();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<bool> UpdateAsync(Predicate<TModel> selector, Action<TModel> updateCallback)
    {
        await _semaphore.WaitAsync();
        try
        {
            var result = default(TModel);
            foreach (var model in Models)
            {
                if (selector(model))
                {
                    result = model;
                    break;
                }
            }
            
            if (result is not null)
            {
                updateCallback(result);
                return true;
            }
            return false;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public bool Update(Predicate<TModel> selector, Action<TModel> updateCallback)
    {
        _semaphore.Wait();
        try
        {
            var result = default(TModel);
            foreach (var model in Models)
            {
                if (selector(model))
                {
                    result = model;
                    break;
                }
            }
            
            if (result is not null)
            {
                updateCallback(result);
                return true;
            }
            return false;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}

