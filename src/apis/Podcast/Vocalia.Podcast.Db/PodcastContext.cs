using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vocalia.Podcast.Db
{
    internal class CategoryEntityTypeConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(c => c.ITunesID).IsRequired();
            builder.Property(c => c.GpodderTag).IsRequired();
            builder.Property(c => c.Title).IsRequired();
            builder.Property(c => c.IconUrl).IsRequired();
        }
    }

    internal class IntegrationTypeEntityTypeConfiguration : IEntityTypeConfiguration<IntegrationType>
    {
        public void Configure(EntityTypeBuilder<IntegrationType> builder)
        {
            builder.Property(i => i.Name).IsRequired();
            builder.Property(i => i.LogoUrl).IsRequired();
        }
    }

    internal class LanguageEntityTypeConfiguration : IEntityTypeConfiguration<Language>
    {
        public void Configure(EntityTypeBuilder<Language> builder)
        {
            builder.Property(l => l.Name).IsRequired();
            builder.HasMany(l => l.Categories)
                .WithOne(e => e.Language);
        }
    }

    internal class PodcastEntityTypeConfiguration : IEntityTypeConfiguration<Podcast>
    {
        public void Configure(EntityTypeBuilder<Podcast> builder)
        {
            builder.Property(p => p.UID).IsRequired();
            builder.Property(p => p.GroupID).IsRequired();
            builder.Property(p => p.Title).IsRequired();
            builder.Property(p => p.Description).IsRequired();
            builder.Property(p => p.RSS).IsRequired();
            builder.Property(p => p.ImageUrl).IsRequired();
            builder.Property(p => p.Active).IsRequired();
            builder.Property(p => p.Subscribers).IsRequired();
            builder.Property(p => p.IsExplicit).IsRequired();

            builder.HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryID)
                .IsRequired();

            builder.HasOne(p => p.Language)
                .WithMany()
                .HasForeignKey(p => p.LanguageID)
                .IsRequired();
        }
    }

    internal class PodcastIntegrationEntityTypeConfiguration : IEntityTypeConfiguration<PodcastIntegration>
    {
        public void Configure(EntityTypeBuilder<PodcastIntegration> builder)
        {
            builder.Property(p => p.Url).IsRequired();

            builder.HasOne(p => p.Podcast)
                .WithMany()
                .HasForeignKey(p => p.PodcastID)
                .IsRequired();

            builder.HasOne(p => p.Type)
                .WithMany()
                .HasForeignKey(p => p.IntegrationTypeID)
                .IsRequired();
        }
    }

    internal class ListenTypeEntityTypeConfiguration : IEntityTypeConfiguration<Listen>
    {
        public void Configure(EntityTypeBuilder<Listen> builder)
        {
            builder.Property(i => i.UserUID).IsRequired();
            builder.Property(i => i.RssUrl).IsRequired();
            builder.Property(i => i.EpisodeUrl).IsRequired();
            builder.Property(i => i.EpisodeName).IsRequired();  
            builder.Property(i => i.Time).IsRequired();
            builder.Property(i => i.IsCompleted).IsRequired();
            builder.Property(i => i.Duration).IsRequired();
        }
    }

    internal class SubscriptionTypeEntityTypeConfiguration : IEntityTypeConfiguration<Subscription>
    {
        public void Configure(EntityTypeBuilder<Subscription> builder)
        {
            builder.Property(i => i.UserUID).IsRequired();
            builder.Property(i => i.RssUrl).IsRequired();
            builder.Property(i => i.Name).IsRequired();
            builder.Property(i => i.ImageUrl).IsRequired();
        }
    }

    public class PodcastContext : DbContext
    {
        public PodcastContext(DbContextOptions<PodcastContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<IntegrationType> IntegrationTypes { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Podcast> Podcasts { get; set; }
        public DbSet<PodcastIntegration> PodcastIntegrations { get; set; }
        public DbSet<Listen> Listens { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new CategoryEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new IntegrationTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new LanguageEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PodcastEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PodcastIntegrationEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SubscriptionTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ListenTypeEntityTypeConfiguration());
        }
    }
}
