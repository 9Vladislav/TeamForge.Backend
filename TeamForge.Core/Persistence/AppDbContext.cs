using Microsoft.EntityFrameworkCore;
using TeamForge.Core.Domain.Entities;

namespace TeamForge.Core.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Game> Games => Set<Game>();
    public DbSet<UserGame> UserGames => Set<UserGame>();
    public DbSet<ActivityPeriod> ActivityPeriods => Set<ActivityPeriod>();
    public DbSet<Chat> Chats => Set<Chat>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<GameInvite> GameInvites => Set<GameInvite>();
    public DbSet<Friend> Friends => Set<Friend>();
    public DbSet<FriendRequest> FriendRequests => Set<FriendRequest>();
    public DbSet<Rating> Ratings => Set<Rating>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}