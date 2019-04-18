using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vocalia.Editor.Db
{
    internal class MemberEntityTypeConfiguration : IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> builder)
        {
            builder.Property(x => x.ID).IsRequired();
            builder.Property(x => x.PodcastID).IsRequired();
            builder.Property(x => x.UserUID).IsRequired();
            builder.Property(x => x.IsAdmin).IsRequired();
        }
    }

    internal class PodcastEntityTypeConfiguration : IEntityTypeConfiguration<Podcast>
    {
        public void Configure(EntityTypeBuilder<Podcast> builder)
        {
            builder.Property(x => x.ID).IsRequired();
            builder.Property(x => x.UID).IsRequired();
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.ImageUrl).IsRequired();

            builder.HasMany(x => x.Sessions).WithOne(x => x.Podcast).IsRequired();
            builder.HasMany(x => x.Members).WithOne(x => x.Podcast).IsRequired();
        }
    }

    internal class SessionEntityTypeConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.Property(x => x.ID).IsRequired();
            builder.Property(x => x.UID).IsRequired();
            builder.Property(x => x.PodcastID).IsRequired();
            builder.Property(x => x.Date).IsRequired();
            builder.Property(x => x.IsFinishedEditing).IsRequired();
            builder.Property(x => x.IsActive).IsRequired();

            builder.HasMany(x => x.Clips).WithOne(x => x.Session).IsRequired();
        }
    }

    internal class TimelineEntryEntityTypeConfiguration : IEntityTypeConfiguration<TimelineEntry>
    {
        public void Configure(EntityTypeBuilder<TimelineEntry> builder)
        {
            builder.Property(x => x.ID).IsRequired();
            builder.Property(x => x.ClipID).IsRequired();
            builder.Property(x => x.SessionID).IsRequired();
            builder.Property(x => x.Position).IsRequired();

            builder.HasOne(x => x.Clip).WithOne(x => x.TimelineEntry).IsRequired();
            builder.HasOne(x => x.Session).WithMany(x => x.TimelineEntries).IsRequired();
        }
    }

    internal class ClipEntityTypeConfiguration : IEntityTypeConfiguration<Clip>
    {
        public void Configure(EntityTypeBuilder<Clip> builder)
        {
            builder.Property(x => x.ID).IsRequired();
            builder.Property(x => x.SessionID).IsRequired();
            builder.Property(x => x.Date).IsRequired();
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.UID).IsRequired();

            builder.HasMany(x => x.Media).WithOne(x => x.Clip).IsRequired();
            builder.HasOne(x => x.Edit).WithOne(x => x.Clip).IsRequired();
        }
    }

    internal class MediaEntityTypeConfiguration : IEntityTypeConfiguration<Media>
    {
        public void Configure(EntityTypeBuilder<Media> builder)
        {
            builder.Property(x => x.ID).IsRequired();
            builder.Property(x => x.ClipID).IsRequired();
            builder.Property(x => x.UserUID).IsRequired();
            builder.Property(x => x.Date).IsRequired();
            builder.Property(x => x.MediaUrl).IsRequired();
            builder.Property(x => x.UID).IsRequired();
        }
    }

    internal class EditEntityTypeConfiguration : IEntityTypeConfiguration<Edit>
    {
        public void Configure(EntityTypeBuilder<Edit> builder)
        {
            builder.Property(x => x.ID).IsRequired();
            builder.Property(x => x.ClipID).IsRequired();
            builder.Property(x => x.StartTrim).IsRequired();
            builder.Property(x => x.EndTrim).IsRequired();
            builder.Property(x => x.Gain).IsRequired();
        }
    }

    public class EditorContext : DbContext
    {
        public EditorContext(DbContextOptions<EditorContext> options) : base(options) { }

        public DbSet<Edit> Edits { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Podcast> Podcasts { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<TimelineEntry> TimelineEntries { get; set; }
        public DbSet<Clip> Clips { get; set; }
        public DbSet<Media> Media { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new EditEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new MemberEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PodcastEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SessionEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ClipEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new MediaEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TimelineEntryEntityTypeConfiguration());
        }
    }

}
