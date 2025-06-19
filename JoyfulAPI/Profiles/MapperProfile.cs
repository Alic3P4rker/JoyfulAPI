using AutoMapper;
using Joyful.API.Entities;
using Joyful.API.Models;

namespace Joyful.API.Profiles;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<EventDto, EventEntity>();
        CreateMap<GroupDto, GroupEntity>();
        CreateMap<LocationDto, LocationEntity>();
        CreateMap<PlannerDto, PlannerEntity>();
        CreateMap<PollDto, PollEntity>();
        CreateMap<UserDto, UserEntity>();
        CreateMap<VoteDto, VoteEntity>();
    }
}