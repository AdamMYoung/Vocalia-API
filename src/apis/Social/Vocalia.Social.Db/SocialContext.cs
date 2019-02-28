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
                builder.Property(c => c.UID).IsRequired();
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
                builder.Property(c => c.UID).IsRequired();
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

        public SocialContext(DbContextOptions<SocialContext> options) : base(options)  { }

        public DbSet<Podcast> Podcasts { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Listen> Listens { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new UserGroupEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new GroupEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PodcastEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ListenEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new FollowEntityTypeConfiguration());
        }
    }
}
