using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vocalia.Publishing.Db
{
    internal class UnassignedEpisodeEntityTypeConfiguration : IEntityTypeConfiguration<UnassignedEpisode>
    {
        public void Configure(EntityTypeBuilder<UnassignedEpisode> builder)
        {
            builder.Property(c => c.ID).IsRequired();
            builder.Property(c => c.UnassignedPodcastID).IsRequired();
            builder.Property(c => c.UID).IsRequired();
            builder.Property(c => c.IsCompleted).IsRequired();
            builder.Property(c => c.Date).IsRequired();

            builder.HasMany(c => c.Clips).WithOne(c => c.Episode).IsRequired();
        }
    }

    internal class UnassignedEpisodeClipEntityTypeConfiguration : IEntityTypeConfiguration<UnassignedEpisodeClip>
    {
        public void Configure(EntityTypeBuilder<UnassignedEpisodeClip> builder)
        {
            builder.Property(c => c.ID).IsRequired();
            builder.Property(c => c.UnassignedEpisodeID).IsRequired();
            builder.Property(c => c.Position).IsRequired();
            builder.Property(c => c.MediaUrl).IsRequired();
        }
    }

    internal class UnassignedPodcastEntityTypeConfiguration : IEntityTypeConfiguration<UnassignedPodcast>
    {
        public void Configure(EntityTypeBuilder<UnassignedPodcast> builder)
        {
            builder.Property(c => c.ID).IsRequired();
            builder.Property(c => c.UID).IsRequired();
            builder.Property(c => c.Name).IsRequired();
            builder.Property(c => c.ImageUrl).IsRequired();
            builder.Property(c => c.IsCompleted).IsRequired();

            builder.HasMany(c => c.Episodes).WithOne(c => c.Podcast).IsRequired();
            builder.HasMany(c => c.Members).WithOne(c => c.Podcast).IsRequired();
        }
    }

    internal class UnassignedPodcastMemberEntityTypeConfiguration : IEntityTypeConfiguration<UnassignedPodcastMember>
    {
        public void Configure(EntityTypeBuilder<UnassignedPodcastMember> builder)
        {
            builder.Property(c => c.ID).IsRequired();
            builder.Property(c => c.UnassignedPodcastID).IsRequired();
            builder.Property(c => c.UserUID).IsRequired();
        }
    }

    internal class CategoryEntityTypeConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(c => c.ID).IsRequired();
            builder.Property(c => c.ITunesID).IsRequired();
            builder.Property(c => c.GPodderTag).IsRequired();
            builder.Property(c => c.Title).IsRequired();

            builder.HasMany(c => c.Podcasts).WithOne(c => c.Category).IsRequired();
        }
    }

    internal class LanguageEntityTypeConfiguration : IEntityTypeConfiguration<Language>
    {
        public void Configure(EntityTypeBuilder<Language> builder)
        {
            builder.Property(c => c.ID).IsRequired();
            builder.Property(c => c.Name).IsRequired();
            builder.Property(c => c.ISOCode).IsRequired();

            builder.HasMany(c => c.Podcasts).WithOne(c => c.Language).IsRequired();
        }
    }

    internal class MemberEntityTypeConfiguration : IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> builder)
        {
            builder.Property(c => c.ID).IsRequired();
            builder.Property(c => c.PodcastID).IsRequired();
            builder.Property(c => c.UserUID).IsRequired();
        }
    }

    internal class EpisodeEntityTypeConfiguration : IEntityTypeConfiguration<Episode>
    {
        public void Configure(EntityTypeBuilder<Episode> builder)
        {
            builder.Property(c => c.ID).IsRequired();
            builder.Property(c => c.PodcastID).IsRequired();
            builder.Property(c => c.UID).IsRequired();
            builder.Property(c => c.Title).IsRequired();
            builder.Property(c => c.Description).IsRequired();
            builder.Property(c => c.RssUrl).IsRequired();
            builder.Property(c => c.PublishDate).IsRequired();
            builder.Property(c => c.MediaUrl).IsRequired();
            builder.Property(c => c.IsActive).IsRequired();
        }
    }

    internal class PodcastEntityTypeConfiguration : IEntityTypeConfiguration<Podcast>
    {
        public void Configure(EntityTypeBuilder<Podcast> builder)
        {
            builder.Property(c => c.ID).IsRequired();
            builder.Property(c => c.UID).IsRequired();
            builder.Property(c => c.CategoryID).IsRequired();
            builder.Property(c => c.LanguageID).IsRequired();
            builder.Property(c => c.Title).IsRequired();
            builder.Property(c => c.Description).IsRequired();
            builder.Property(c => c.ImageUrl).IsRequired();
            builder.Property(c => c.IsActive).IsRequired();
            builder.Property(c => c.IsExplicit).IsRequired();
            builder.Property(c => c.RssUrl).IsRequired();

            builder.HasMany(c => c.Episodes).WithOne(c => c.Podcast).IsRequired();
            builder.HasMany(c => c.Members).WithOne(c => c.Podcast).IsRequired();
        }
    }

    public class PublishContext : DbContext
    {
        public PublishContext(DbContextOptions<PublishContext> options) : base(options) { }

        public DbSet<Episode> Episodes { get; set; }
        public DbSet<Podcast> Podcasts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<UnassignedPodcastMember> UnassignedPodcastMembers { get; set; }
        public DbSet<UnassignedPodcast> UnassignedPodcasts { get; set; }
        public DbSet<UnassignedEpisode> UnassignedEpisodes { get; set; }
        public DbSet<UnassignedEpisodeClip> UnassignedEpisodeClips { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new CategoryEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new MemberEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new LanguageEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PodcastEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new EpisodeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UnassignedPodcastMemberEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UnassignedEpisodeClipEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UnassignedPodcastEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UnassignedEpisodeEntityTypeConfiguration());
        }
    }
}
