using Microsoft.EntityFrameworkCore;
using ShareSpaceApi.Data.Models;

namespace ShareSpaceApi.Data.Context;

public class ShareSpaceDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<LikedPost> LikedPosts { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Follower> Followers { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<NotificationType> NotificationTypes { get; set; }
    public DbSet<PostTag> PostTags { get; set; }
    public DbSet<UserInterest> UserInterests { get; set; }
    public DbSet<Interest> Interests { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<ViewedPost> ViewedPosts { get; set; }
    public DbSet<PostImage> PostImages { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }

    public ShareSpaceDbContext(DbContextOptions<ShareSpaceDbContext> options)
        : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<UserInterest>().HasKey(lp => new { lp.UserId, lp.InterestId });
        modelBuilder.Entity<ViewedPost>().HasKey(lp => new { lp.UserId, lp.PostId });

        modelBuilder.Entity<Follower>(f =>
        {
            f.Property(f => f.CreatedAt).HasDefaultValueSql("Now()");
            f.HasKey(f => new { f.FollowedId, f.FollowerId });
            f.HasOne("ShareSpaceApi.Data.Models.User", "FollowedUser")
                .WithMany("Followers")
                .HasForeignKey("FollowedId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });

        modelBuilder.Entity<User>(u =>
        {
            u.Property(e => e.CreatedAt).HasDefaultValueSql("Now()");
            u.Property(e => e.UserId).HasDefaultValueSql("uuid_generate_v4()");
            u.Property(e => e.OnlineStatus).HasDefaultValue(false);
            u.HasIndex(e => e.UserName).IsUnique();
            u.HasIndex(e => e.Email).IsUnique();
        });

        modelBuilder.Entity<RefreshToken>(p =>
        {
            p.Property(i => i.Id).HasDefaultValueSql("uuid_generate_v4()");
        });

        modelBuilder.Entity<Post>(p =>
        {
            p.Property(c => c.CreatedAt).HasDefaultValueSql("Now()");
            p.Property(i => i.Id).HasDefaultValueSql("uuid_generate_v4()");
            p.Property(l => l.Likes).HasDefaultValue("0");
            p.Property(l => l.Views).HasDefaultValue("0");
        });

        modelBuilder.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<LikedPost>(lp =>
        {
            lp.HasKey(lp => new { lp.UserId, lp.PostId });
            lp.Property(lp => lp.CreatedAt).HasDefaultValueSql("Now()");
        });

        modelBuilder.Entity<Comment>(c =>
        {
            c.Property(c => c.CreatedAt).HasDefaultValueSql("Now()");
            c.Property(i => i.Id).HasDefaultValueSql("uuid_generate_v4()");
            c.Property(l => l.Likes).HasDefaultValue("0");
        });

        modelBuilder.Entity<PostTag>(c =>
        {
            c.Property(c => c.Id).HasDefaultValueSql("uuid_generate_v4()");
        });

        modelBuilder.Entity<Message>(m =>
        {
            m.Property(c => c.CreatedAt).HasDefaultValueSql("Now()");
            m.Property(i => i.MessageId).HasDefaultValueSql("uuid_generate_v4()");
            m.Property(l => l.Seen).HasDefaultValue("FALSE");
            m.HasOne("ShareSpaceApi.Data.Models.User", "Receiver")
                .WithMany("Messages")
                .HasForeignKey("ReceiverId")
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired();

            m.HasOne("ShareSpaceApi.Data.Models.User", "Sender")
                .WithMany()
                .HasForeignKey("SenderId")
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired();
        });

        modelBuilder.Entity<Notification>(n =>
        {
            n.Property(c => c.CreatedAt).HasDefaultValueSql("Now()");
            n.Property(i => i.Id).HasDefaultValueSql("uuid_generate_v4()");
            n.Property(l => l.Seen).HasDefaultValue("FALSE");
            n.HasOne("ShareSpaceApi.Data.Models.User", "GetterUser")
                .WithMany("Notifications")
                .HasForeignKey("UserId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });

        modelBuilder
            .Entity<Interest>()
            .HasData(
                new Interest { Id = 1, InterestName = "Sports" },
                new Interest { Id = 2, InterestName = "Photography" },
                new Interest { Id = 3, InterestName = "Travel" },
                new Interest { Id = 4, InterestName = "Cooking" },
                new Interest { Id = 5, InterestName = "Movies" },
                new Interest { Id = 6, InterestName = "Music" },
                new Interest { Id = 7, InterestName = "Art" },
                new Interest { Id = 8, InterestName = "Reading" },
                new Interest { Id = 9, InterestName = "Writing" },
                new Interest { Id = 10, InterestName = "Dancing" },
                new Interest { Id = 11, InterestName = "Gaming" },
                new Interest { Id = 12, InterestName = "Gardening" },
                new Interest { Id = 13, InterestName = "Hiking" },
                new Interest { Id = 14, InterestName = "Cycling" },
                new Interest { Id = 15, InterestName = "Fishing" },
                new Interest { Id = 16, InterestName = "Yoga" },
                new Interest { Id = 17, InterestName = "Meditation" },
                new Interest { Id = 18, InterestName = "Photography" },
                new Interest { Id = 19, InterestName = "Knitting" },
                new Interest { Id = 20, InterestName = "Sewing" },
                new Interest { Id = 21, InterestName = "Pottery" },
                new Interest { Id = 22, InterestName = "Woodworking" },
                new Interest { Id = 23, InterestName = "Baking" },
                new Interest { Id = 24, InterestName = "Astronomy" },
                new Interest { Id = 25, InterestName = "Bird Watching" },
                new Interest { Id = 26, InterestName = "Scuba Diving" },
                new Interest { Id = 27, InterestName = "Rock Climbing" },
                new Interest { Id = 28, InterestName = "Surfing" },
                new Interest { Id = 29, InterestName = "Running" },
                new Interest { Id = 30, InterestName = "Swimming" },
                new Interest { Id = 31, InterestName = "Puzzles" },
                new Interest { Id = 32, InterestName = "Chess" },
                new Interest { Id = 33, InterestName = "Magic Tricks" },
                new Interest { Id = 34, InterestName = "Origami" },
                new Interest { Id = 35, InterestName = "Painting" },
                new Interest { Id = 36, InterestName = "Sculpture" },
                new Interest { Id = 37, InterestName = "Calligraphy" },
                new Interest { Id = 38, InterestName = "Graffiti" },
                new Interest { Id = 39, InterestName = "Cosplay" },
                new Interest { Id = 40, InterestName = "Stand-up Comedy" },
                new Interest { Id = 41, InterestName = "Podcasting" },
                new Interest { Id = 42, InterestName = "Blogging" },
                new Interest { Id = 43, InterestName = "Vlogging" },
                new Interest { Id = 44, InterestName = "Programming" },
                new Interest { Id = 45, InterestName = "Robotics" },
                new Interest { Id = 46, InterestName = "3D Printing" },
                new Interest { Id = 47, InterestName = "Astrology" },
                new Interest { Id = 48, InterestName = "Tarot Reading" },
                new Interest { Id = 49, InterestName = "Martial Arts" },
                new Interest { Id = 50, InterestName = "Parkour" },
                new Interest { Id = 51, InterestName = "Wine Tasting" },
                new Interest { Id = 52, InterestName = "Brewing Beer" },
                new Interest { Id = 53, InterestName = "Coffee Roasting" },
                new Interest { Id = 54, InterestName = "Cooking Exotic Dishes" },
                new Interest { Id = 55, InterestName = "Learning New Languages" },
                new Interest { Id = 56, InterestName = "Volunteering" },
                new Interest { Id = 57, InterestName = "Archaeology" },
                new Interest { Id = 58, InterestName = "Genealogy" },
                new Interest { Id = 59, InterestName = "Meteorology" },
                new Interest { Id = 60, InterestName = "Philosophy" }
            );

        modelBuilder
            .Entity<NotificationType>()
            .HasData(
                new NotificationType { Id = -1, Name = "Unfollow" },
                new NotificationType { Id = 1, Name = "Follow" }
            );

        modelBuilder
            .Entity<Role>()
            .HasData(
                new Role { RoleId = 1, RoleName = "user" },
                new Role { RoleId = 2, RoleName = "admin" }
            );
    }
}
