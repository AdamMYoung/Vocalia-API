using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Ingest.Db
{

    public class IngestContext : DbContext
    {
        internal class SessionEntityTypeConfiguration : IEntityTypeConfiguration<Session>
        {
            public void Configure(EntityTypeBuilder<Session> builder)
            {
                builder.Property(c => c.ID).IsRequired();
                builder.Property(c => c.UID).IsRequired();
                builder.Property(c => c.PodcastID).IsRequired();
                builder.Property(c => c.InProgress).IsRequired();

                builder.HasMany(c => c.MediaEntries)
                   .WithOne(c => c.Session);
            }
        }

        internal class SessionMediaEntityTypeConfiguration : IEntityTypeConfiguration<SessionMedia>
        {
            public void Configure(EntityTypeBuilder<SessionMedia> builder)
            {
                builder.Property(c => c.ID).IsRequired();
                builder.Property(c => c.SessionID).IsRequired();
                builder.Property(c => c.UserUID).IsRequired();
                builder.Property(c => c.Timestamp).IsRequired();
                builder.Property(c => c.MediaUrl).IsRequired();
            }
        }

        internal class PodcastEntityTypeConfiguration : IEntityTypeConfiguration<Podcast>
        {
            public void Configure(EntityTypeBuilder<Podcast> builder)
            {
                builder.Property(c => c.ID).IsRequired();
                builder.Property(c => c.UID).IsRequired();
                builder.Property(c => c.Name).IsRequired();
                builder.Property(c => c.Description).IsRequired();
                builder.Property(c => c.ImageUrl).IsRequired();

                builder.HasMany(c => c.Sessions)
                    .WithOne(c => c.Podcast);

                builder.HasMany(l => l.Users)
                 .WithOne(e => e.Podcast);

                builder.HasMany(l => l.Invites)
                    .WithOne(l => l.Podcast);
            }
        }

        internal class PodcastInviteEntityTypeConfiguration : IEntityTypeConfiguration<PodcastInvite>
        {
            public void Configure(EntityTypeBuilder<PodcastInvite> builder)
            {
                builder.Property(c => c.ID).IsRequired();
                builder.Property(c => c.PodcastID).IsRequired();
            }
        }

        internal class SessionUserEntityTypeConfiguration : IEntityTypeConfiguration<SessionUser>
        {
            public void Configure(EntityTypeBuilder<SessionUser> builder)
            {
                builder.Property(c => c.ID).IsRequired();
                builder.Property(c => c.SessionID).IsRequired();
                builder.Property(c => c.UserUID).IsRequired();
            }
        }

        internal class PodcastUserEntityTypeConfiguration : IEntityTypeConfiguration<PodcastUser>
        {
            public void Configure(EntityTypeBuilder<PodcastUser> builder)
            {
                builder.Property(c => c.ID).IsRequired();
                builder.Property(c => c.PodcastID).IsRequired();
                builder.Property(c => c.UserUID).IsRequired();
            }
        }

        public IngestContext(DbContextOptions<IngestContext> options) : base(options) { }

        public DbSet<Podcast> Podcasts { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<SessionUser> SessionUsers { get; set; }
        public DbSet<SessionMedia> SessionMedia { get; set; }
        public DbSet<PodcastUser> PodcastUsers { get; set; }
        public DbSet<PodcastInvite> PodcastInvites { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new PodcastUserEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PodcastEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SessionEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SessionMediaEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SessionUserEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PodcastInviteEntityTypeConfiguration());
        }
    }
}
