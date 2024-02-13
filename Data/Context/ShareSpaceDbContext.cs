﻿using Microsoft.EntityFrameworkCore;
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
                new Interest { Id = 5, InterestName = "Movies" }
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
