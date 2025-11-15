using System;

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
    Task<bool> RemoveAsync(Predicate<TModel> selector);
    bool Remove(Predicate<TModel> selector);
    TModel FirstOrDefault(Predicate<TModel> selector);
    IEnumerable<TModel> GetAll(Func<TModel, bool> selector);
    bool Update(Predicate<TModel> selector, Action<TModel> updateCallback);
}


public abstract class RepositoryBase<TModel> : IRepositoryBase<TModel> where TModel : IAutoMapperEntities 
{

    protected HashSet<TModel> Models;

    public RepositoryBase()
    {
        Models = new HashSet<TModel>();
    }

    public async virtual Task<bool> RemoveAsync(Predicate<TModel> selector)
    {
        var result = false;
        foreach (var model in Models)
        {
            if (selector(model))
            {
                result = Models.Remove(model);
            }
        }

        return result;
    }

    public virtual bool Remove(Predicate<TModel> selector)
    {
        var result = false;
        foreach (var model in Models)
        {
            if (selector(model))
            {
                result = Models.Remove(model);
            }
        }

        return result;
    }

    public async Task<TModel> FirstOrDefualtAsync(Predicate<TModel> selector)
    {
        var result = default(TModel);

        var model = CoreGet(selector);
        if (model is null)
        {
            return result;
        }

        return model;
    }

    public TModel FirstOrDefault(Predicate<TModel> selector)
    {
        var result = default(TModel);

        var model = CoreGet(selector);
        if (model is null)
        {
            return result;
        }

        return model;
    }


    protected virtual TModel CoreGet(Predicate<TModel> selector)
    {
        var result = default(TModel);
        foreach (var model in Models)
        {
            if (selector(model))
            {
                result = model;
            }
        }

        return result;
    }

    public virtual async Task<TModel> InsertAsync(TModel model)
    {
        return Insert(model);
    }

    public async Task<IEnumerable<TModel>> GetAllAsync()
    {
        return Models.ToArray();
    }
    /// <summary>
    /// Auto Id Generator
    /// </summary>
    /// <param name="instance"></param>
    /// <returns></returns>
    public virtual TModel Insert(TModel instance)
    {

        if (Models.Add(instance) == false)
        {
            throw new Exception("Key already present.");
        }

        return instance;
    }

    public virtual IEnumerable<TModel> GetAll()
    {
        return Models.ToArray();
    }

    public async Task<IEnumerable<TModel>> GetAllAsync(Func<TModel, bool> selector)
    {
        return Models.Where(x => selector(x)).ToArray();
    }

    public IEnumerable<TModel> GetAll(Func<TModel, bool> selector)
    {
        return Models.Where(x => selector(x)).ToArray();
    }

    public async Task<bool> UpdateAsync(Predicate<TModel> selector, Action<TModel> updateCallback)
    {
        var model = CoreGet(selector);

        if (model is not null)
        {
            updateCallback(model);
            return true;
        }

        return false;
    }

    public bool Update(Predicate<TModel> selector, Action<TModel> updateCallback)
    {
        var model = CoreGet(selector);

        if (model is not null)
        {
            updateCallback(model);
            return true;
        }

        return false;
    }
}

