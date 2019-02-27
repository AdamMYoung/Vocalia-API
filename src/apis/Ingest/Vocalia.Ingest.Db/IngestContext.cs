using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Ingest.Db
{

    public class IngestContext : DbContext
    {
        internal class GroupEntityTypeConfiguration : IEntityTypeConfiguration<Group>
        {
            public void Configure(EntityTypeBuilder<Group> builder)
            {
                builder.Property(c => c.ID).IsRequired();
                builder.Property(c => c.UID).IsRequired();
                builder.Property(c => c.Name).IsRequired();
                builder.Property(c => c.Description).IsRequired();

                builder.HasMany(l => l.UserGroups)
                 .WithOne(e => e.Group);

                builder.HasMany(l => l.Podcasts)
                 .WithOne(e => e.Group);
            }
        }

        internal class SessionEntityTypeConfiguration : IEntityTypeConfiguration<Session>
        {
            public void Configure(EntityTypeBuilder<Session> builder)
            {
                builder.Property(c => c.ID).IsRequired();
                builder.Property(c => c.UID).IsRequired();
                builder.Property(c => c.UID).IsRequired();
                builder.Property(c => c.PodcastID).IsRequired();
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

                builder.HasMany(c => c.Sessions)
                    .WithOne(c => c.Podcast);
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
                builder.Property(c => c.Active).IsRequired();

                builder.HasMany(l => l.UserGroups)
               .WithOne(e => e.User);
            }
        }

        public IngestContext(DbContextOptions<IngestContext> options) : base(options) { }

        public DbSet<Podcast> Podcasts { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Session> Sessions { get; set; }
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
            modelBuilder.ApplyConfiguration(new SessionEntityTypeConfiguration());
        }
    }
}
