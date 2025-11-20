using System;
using ShiftAssignerServer.Models;

namespace ShiftAssignerServer.Repositories;

public interface ITenantRepository : IRepositoryBase<Tenant>
{
    
}

public class TenantRepository : RepositoryBase<Tenant> , ITenantRepository
{
    public TenantRepository()
    {
    }
}
