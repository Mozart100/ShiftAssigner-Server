using AutoMapper;
using Microsoft.AspNetCore.Identity.Data;
using ShiftAssignerServer.Models;

namespace ShiftAssignerServer.Startup;

public class ConfigureMapper : Profile
{
    public ConfigureMapper()
    {
        CreateMap<RegisterRequest, Worker>();
        CreateMap<RegisterRequest, ShiftLeader>();

        // CreateMap<Chat, ChatDto>();
        //      //.ForMember(dest => dest.Messages, opt => opt.MapFrom(src => src.UserMessages));
    }
}