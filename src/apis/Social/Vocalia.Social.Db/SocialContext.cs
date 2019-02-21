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
                builder.Property(c => c.UserUID).IsRequired();
                builder.Property(c => c.FirstName).IsRequired();
                builder.Property(c => c.LastName).IsRequired();
                builder.Property(c => c.Birthday).IsRequired();
                builder.Property(c => c.Active).IsRequired();

                builder.HasMany(l => l.UserGroups)
               .WithOne(e => e.User);
            }
        }

        public SocialContext(DbContextOptions<SocialContext> options) : base(options)  { }

        public DbSet<Podcast> Podcasts { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserGroupEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new GroupEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PodcastEntityTypeConfiguration());
        }
    }
}
