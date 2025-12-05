using AutoMapper;
using ShiftAssignerServer.Models.Stuff;
using ShiftAssignerServer.Requests;
using static ShiftAssignerServer.Models.Stuff.Worker;

namespace ShiftAssignerServer.Startup;

public class ConfigureMapper : Profile
{
    public ConfigureMapper()
    {
        CreateMap<RegisterRequest, Worker>();
        CreateMap<RegisterRequest, ShiftLeader>();

        CreateMap<Models.Company, AllTenantsResponse>();
        CreateMap<ShiftLeader, PubShiftLeader>();
        CreateMap<Worker, PubWorker>();

        CreateMap<TenantRegisterRequest, BossTenant>();
        CreateMap<LoginShiftLeaderRequest, ShiftLeader>();
        CreateMap<RegisteringShiftLeaderRequest, ShiftLeader>();


        

    }
}