using Microsoft.EntityFrameworkCore;
using SprintIQ.API.Models;

namespace SprintIQ.API.Data;

public class SprintIQDbContext : DbContext
{
    public SprintIQDbContext(DbContextOptions<SprintIQDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();
    public DbSet<Sprint> Sprints => Set<Sprint>();
    public DbSet<SprintTask> SprintTasks => Set<SprintTask>();
    public DbSet<SprintBurndown> SprintBurndowns => Set<SprintBurndown>();
    public DbSet<DailyStandup> DailyStandups => Set<DailyStandup>();
    public DbSet<Blocker> Blockers => Set<Blocker>();
    public DbSet<Badge> Badges => Set<Badge>();
    public DbSet<UserBadge> UserBadges => Set<UserBadge>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // TeamMember configuration
        modelBuilder.Entity<TeamMember>(entity =>
        {
            entity.HasOne(tm => tm.Team)
                .WithMany(t => t.Members)
                .HasForeignKey(tm => tm.TeamId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(tm => tm.User)
                .WithMany(u => u.TeamMemberships)
                .HasForeignKey(tm => tm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.TeamId, e.UserId }).IsUnique();
        });

        // Sprint configuration
        modelBuilder.Entity<Sprint>(entity =>
        {
            entity.HasOne(s => s.Team)
                .WithMany(t => t.Sprints)
                .HasForeignKey(s => s.TeamId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // SprintTask configuration
        modelBuilder.Entity<SprintTask>(entity =>
        {
            entity.HasOne(st => st.Sprint)
                .WithMany(s => s.Tasks)
                .HasForeignKey(st => st.SprintId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(st => st.Assignee)
                .WithMany(u => u.AssignedTasks)
                .HasForeignKey(st => st.AssigneeId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // SprintBurndown configuration
        modelBuilder.Entity<SprintBurndown>(entity =>
        {
            entity.HasOne(sb => sb.Sprint)
                .WithMany(s => s.BurndownData)
                .HasForeignKey(sb => sb.SprintId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.SprintId, e.Date }).IsUnique();
        });

        // DailyStandup configuration
        modelBuilder.Entity<DailyStandup>(entity =>
        {
            entity.HasOne(ds => ds.User)
                .WithMany(u => u.Standups)
                .HasForeignKey(ds => ds.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ds => ds.Sprint)
                .WithMany(s => s.Standups)
                .HasForeignKey(ds => ds.SprintId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.UserId, e.SprintId, e.Date }).IsUnique();
        });

        // Blocker configuration
        modelBuilder.Entity<Blocker>(entity =>
        {
            entity.HasOne(b => b.Task)
                .WithMany(t => t.Blockers)
                .HasForeignKey(b => b.TaskId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(b => b.Sprint)
                .WithMany()
                .HasForeignKey(b => b.SprintId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // UserBadge configuration
        modelBuilder.Entity<UserBadge>(entity =>
        {
            entity.HasOne(ub => ub.User)
                .WithMany(u => u.Badges)
                .HasForeignKey(ub => ub.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ub => ub.Badge)
                .WithMany()
                .HasForeignKey(ub => ub.BadgeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed default badges
        SeedBadges(modelBuilder);
    }

    private void SeedBadges(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Badge>().HasData(
            new Badge { Id = 1, Name = "First Steps", Description = "Complete your first task", Icon = "🎯", Color = "#4CAF50", Type = BadgeType.TaskCompletion, CriteriaValue = 1, PointValue = 25 },
            new Badge { Id = 2, Name = "Task Master", Description = "Complete 10 tasks", Icon = "⚡", Color = "#2196F3", Type = BadgeType.TaskCompletion, CriteriaValue = 10, PointValue = 100 },
            new Badge { Id = 3, Name = "Productivity Pro", Description = "Complete 50 tasks", Icon = "🚀", Color = "#9C27B0", Type = BadgeType.TaskCompletion, CriteriaValue = 50, PointValue = 500 },
            new Badge { Id = 4, Name = "Standup Starter", Description = "Submit 5 daily standups", Icon = "📋", Color = "#FF9800", Type = BadgeType.StandupStreak, CriteriaValue = 5, PointValue = 50 },
            new Badge { Id = 5, Name = "Consistency King", Description = "7-day standup streak", Icon = "🔥", Color = "#F44336", Type = BadgeType.StandupStreak, CriteriaValue = 7, PointValue = 150 },
            new Badge { Id = 6, Name = "Streak Legend", Description = "30-day standup streak", Icon = "👑", Color = "#FFD700", Type = BadgeType.StandupStreak, CriteriaValue = 30, PointValue = 500 },
            new Badge { Id = 7, Name = "Blocker Buster", Description = "Resolve 5 blockers", Icon = "🔓", Color = "#00BCD4", Type = BadgeType.BlockerBuster, CriteriaValue = 5, PointValue = 100 },
            new Badge { Id = 8, Name = "Sprint Champion", Description = "Complete a sprint at 100%", Icon = "🏆", Color = "#FFD700", Type = BadgeType.SprintChampion, CriteriaValue = 1, PointValue = 300 },
            new Badge { Id = 9, Name = "Early Bird", Description = "Submit standup before 9 AM 5 times", Icon = "🌅", Color = "#FFEB3B", Type = BadgeType.EarlyBird, CriteriaValue = 5, PointValue = 75 },
            new Badge { Id = 10, Name = "Team Player", Description = "Help resolve 3 team blockers", Icon = "🤝", Color = "#E91E63", Type = BadgeType.TeamPlayer, CriteriaValue = 3, PointValue = 100 }
        );
    }
}
