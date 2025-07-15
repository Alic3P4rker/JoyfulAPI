using Joyful.API.Abstractions.Repositories;
using Joyful.API.Repositories;

namespace Joyful.API.Services;

public static class BuildInfranstructure
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<IPlannerRepository, PlannerRepository>();
        services.AddScoped<IPollRepository, PollRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IVoteRepository, VoteRepository>();
        services.AddScoped<IPlannerGroupRepository, PlannerGroupRepository>();
        services.AddScoped<IThemeRepository, ThemeRepository>();
        services.AddScoped<IChatRepository, ChatRepository>();
        services.AddScoped<IUserFriendsRepository, UserFriendsRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IPasswordService, PasswordService>();

        return services;
    }
}