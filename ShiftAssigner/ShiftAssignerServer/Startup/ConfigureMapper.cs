using AutoMapper;
using ShiftAssignerServer.Models.Stuff;
using ShiftAssignerServer.Requests;

namespace ShiftAssignerServer.Startup;

public class ConfigureMapper : Profile
{
    public ConfigureMapper()
    {
        CreateMap<RegisterRequest, Worker>();
        CreateMap<RegisterRequest, ShiftLeader>();
        CreateMap<RegisterRequest, BossTenant>();

        // CreateMap<Chat, ChatDto>();
        //      //.ForMember(dest => dest.Messages, opt => opt.MapFrom(src => src.UserMessages));
    }
}