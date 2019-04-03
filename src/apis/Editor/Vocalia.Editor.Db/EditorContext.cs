using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

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

            builder.HasMany(x => x.Sessions).WithOne(x => x.Podcast);
            builder.HasMany(x => x.Members).WithOne(x => x.Podcast);
        }
    }

    internal class SessionEntityTypeConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.Property(x => x.ID).IsRequired();
            builder.Property(x => x.UID).IsRequired();
            builder.Property(x => x.PodcastID).IsRequired();
            builder.Property(x => x.IsFinishedEditing).IsRequired();

            builder.HasMany(x => x.Users).WithOne(x => x.Session);
        }
    }

    internal class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(x => x.ID).IsRequired();
            builder.Property(x => x.SessionID).IsRequired();
            builder.Property(x => x.UserUID).IsRequired();
            builder.Property(x => x.Name).IsRequired();

            builder.HasMany(x => x.Edits).WithOne(x => x.User);
        }
    }

    internal class EditTypeEntityTypeConfiguration : IEntityTypeConfiguration<EditType>
    {
        public void Configure(EntityTypeBuilder<EditType> builder)
        {
            builder.Property(x => x.ID).IsRequired();
            builder.Property(x => x.Name).IsRequired();
        }
    }

    internal class EditEntityTypeConfiguration : IEntityTypeConfiguration<Edit>
    {
        public void Configure(EntityTypeBuilder<Edit> builder)
        {
            builder.Property(x => x.ID).IsRequired();
            builder.Property(x => x.UserID).IsRequired();
            builder.Property(x => x.StartTimestamp).IsRequired();
            builder.Property(x => x.EndTimestamp).IsRequired();
            builder.Property(x => x.EditTypeID).IsRequired();

            builder.HasOne(x => x.EditType).WithMany(x => x.Edits);
        }
    }

    public class EditorContext : DbContext
    {
        public EditorContext(DbContextOptions<EditorContext> options) : base(options) { }

        public DbSet<Edit> Edits { get; set; }
        public DbSet<EditType> EditTypes { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Podcast> Podcasts { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new EditEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new EditTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new MemberEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PodcastEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SessionEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
        }
    }

}
