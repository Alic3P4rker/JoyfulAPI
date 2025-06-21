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

        CreateMap<UserCreateDto, UserEntity>().ReverseMap();
        CreateMap<UserEntity, UserDetailsDto>();

        CreateMap<VoteDto, VoteEntity>();
    }
}