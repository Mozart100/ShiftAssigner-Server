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
        CreateMap<TenantRegisterRequest, BossTenant>();
        CreateMap<Models.Tenant, AllTenantsResponse>();
        CreateMap<ShiftLeader, PubShiftLeader>();
        CreateMap<Worker, PubWorker>();

        // CreateMap<Chat, ChatDto>();
        //      //.ForMember(dest => dest.Messages, opt => opt.MapFrom(src => src.UserMessages));
    }
}