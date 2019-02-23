using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Social.Db
{

    public class SocialContext : DbContext
    {
        internal class GroupEntityTypeConfiguration : IEntityTypeConfiguration<Group>
        {
            public void Configure(EntityTypeBuilder<Group> builder)
            {
                builder.Property(c => c.ID).IsRequired();
                builder.Property(c => c.Name).IsRequired();
                builder.Property(c => c.Description).IsRequired();
                builder.Property(c => c.WebsiteUrl).IsRequired();

                builder.HasMany(l => l.UserGroups)
                 .WithOne(e => e.Group);

                builder.HasMany(l => l.Podcasts)
                 .WithOne(e => e.Group);
            }
        }

        internal class PodcastEntityTypeConfiguration : IEntityTypeConfiguration<Podcast>
        {
            public void Configure(EntityTypeBuilder<Podcast> builder)
            {
                builder.Property(c => c.ID).IsRequired();
                builder.Property(c => c.GroupID).IsRequired();
                builder.Property(c => c.Name).IsRequired();
                builder.Property(c => c.WebsiteUrl).IsRequired();
            }
        }

        internal class FollowEntityTypeConfiguration : IEntityTypeConfiguration<Follow>
        {
            public void Configure(EntityTypeBuilder<Follow> builder)
            {
                builder.Property(c => c.ID).IsRequired();
                builder.Property(c => c.UserUID).IsRequired();
                builder.Property(c => c.FollowUID).IsRequired();
            }
        }


        internal class ListenEntityTypeConfiguration : IEntityTypeConfiguration<Listen>
        {
            public void Configure(EntityTypeBuilder<Listen> builder)
            {
                builder.Property(c => c.ID).IsRequired();
                builder.Property(c => c.UserUID).IsRequired();
                builder.Property(c => c.RssUrl).IsRequired();
                builder.Property(c => c.EpisodeUrl).IsRequired();
                builder.Property(c => c.EpisodeName).IsRequired();
                builder.Property(c => c.Date).IsRequired();
                builder.Property(c => c.IsCompleted).IsRequired();
            }
        }

        internal class UserGroupEntityTypeConfiguration : IEntityTypeConfiguration<UserGroup>
        {
            public void Configure(EntityTypeBuilder<UserGroup> builder)
            {
                builder.Property(c => c.ID).IsRequired();
                builder.Property(c => c.GroupID).IsRequired();
                builder.Property(c => c.UserUID).IsRequired();
            }
        }

        internal class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
        {
            public void Configure(EntityTypeBuilder<User> builder)
            {
                builder.HasKey(x => x.UserUID);
                builder.Property(c => c.UserUID).IsRequired();
                builder.Property(c => c.FirstName).IsRequired();
                builder.Property(c => c.LastName).IsRequired();
                builder.Property(c => c.UserTag).IsRequired();
                builder.Property(c => c.Birthday).IsRequired();
                builder.Property(c => c.Active).IsRequired();

                builder.HasMany(l => l.UserGroups)
               .WithOne(e => e.User);

                builder.HasMany(l => l.Listens)
                .WithOne(e => e.User);

                builder.HasMany(l => l.Followers)
                    .WithOne(e => e.Following)
                    .HasForeignKey(e => e.FollowUID);

                builder.HasMany(l => l.Followings)
                    .WithOne(e => e.Follower)
                    .HasForeignKey(e => e.UserUID);
            }
        }

        public SocialContext(DbContextOptions<SocialContext> options) : base(options)  { }

        public DbSet<Podcast> Podcasts { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Listen> Listens { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserGroupEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new GroupEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PodcastEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ListenEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new FollowEntityTypeConfiguration());
        }
    }
}
