using AutoMapper;
using Joyful.API.Entities;
using Joyful.API.Models;

namespace Joyful.API.Profiles;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<GroupDto, GroupEntity>().ReverseMap();
        CreateMap<LocationDto, LocationEntity>().ReverseMap();
        CreateMap<PlannerDto, PlannerEntity>().ReverseMap();

        CreateMap<UserCreateDto, UserEntity>().ReverseMap();
        CreateMap<UserEntity, UserDetailsDto>();

        CreateMap<VoteDto, VoteEntity>();

        CreateMap<ThemeCreateDto, ThemeEntity>();
        CreateMap<ThemeDetailsDto, ThemeEntity>().ReverseMap();

        CreateMap<EventCreateDto, EventEntity>();
        CreateMap<EventDetailsDto, EventEntity>().ReverseMap();

        CreateMap<PollCreateDto, PollEntity>();
        CreateMap<PollDetailsDto, PollEntity>().ReverseMap();

        CreateMap<UserFriendsCreateDto, UserFriendsEntity>();
        CreateMap<UserFriendsDetailsDto, UserFriendsEntity>().ReverseMap();
    }
}