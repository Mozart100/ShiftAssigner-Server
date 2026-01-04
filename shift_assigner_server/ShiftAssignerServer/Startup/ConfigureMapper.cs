using AutoMapper;
using ShiftAssignerServer.Models.Stuff;
using ShiftAssignerServer.Models.WorkerScheduling;
using ShiftAssignerServer.Requests;
using static ShiftAssignerServer.Models.Stuff.Worker;

namespace ShiftAssignerServer.Startup;

public class ConfigureMapper : Profile
{
    public ConfigureMapper()
    {
        CreateMap<RegisterRequest, Worker>();
        CreateMap<RegisterRequest, ShiftLeader>();

        CreateMap<Models.Schema, AllTenantsResponse>();
        CreateMap<ShiftLeader, PubShiftLeader>();
        CreateMap<Worker, PubWorker>();
        CreateMap<WorkerRegisteringRequest, Worker>();

        CreateMap<TenantRegisterRequest, BossTenant>();
        CreateMap<LoginShiftLeaderRequest, ShiftLeader>();
        CreateMap<RegisteringShiftLeaderRequest, ShiftLeader>();

        // CreateMap<TenantShiftScheduling, TenantRegisterRequest.TenantShiftInfo>();
        CreateMap<TenantRegisterRequest, TenantShiftScheduling>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()); // Let database generate the ID


    }
}