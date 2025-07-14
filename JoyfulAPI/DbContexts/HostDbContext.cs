using Joyful.API.Configurations;
using Joyful.API.Entities;
using Microsoft.EntityFrameworkCore;

public class HostDbContext : DbContext
{
    public HostDbContext(DbContextOptions<HostDbContext> options) : base(options) { }
    public DbSet<AccountEntity> Accounts { get; set; }
    public DbSet<EventEntity> Events { get; set; }
    public DbSet<GroupEntity> Groups { get; set; }
    public DbSet<LocationEntity> Locations { get; set; }
    public DbSet<PlannerEntity> Planners { get; set; }
    public DbSet<PollEntity> Polls { get; set; }
    public DbSet<UserEntity> User { get; set; }
    public DbSet<VoteEntity> Votes { get; set; }
    public DbSet<ThemeEntity> Themes { get; set; }
    public DbSet<PlannerGroupEntity> PlannerGroups { get; set; }
    public DbSet<ChatEntity> Chats { get; set; }
    public DbSet<MessageEntity> Messages { get; set; }
    public DbSet<UserFriendsEntity> UserFriends { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new EventConfiguration());
        modelBuilder.ApplyConfiguration(new GroupConfiguration());
        modelBuilder.ApplyConfiguration(new LocationConfiguration());
        modelBuilder.ApplyConfiguration(new PlannerConfiguration());
        modelBuilder.ApplyConfiguration(new PollConfiguration());
        modelBuilder.ApplyConfiguration(new VoteConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new PlannerGroupConfiguration());
        modelBuilder.ApplyConfiguration(new ThemeConfiguration());
        modelBuilder.ApplyConfiguration(new UserFriendsEntityConfiguration());
    }

}