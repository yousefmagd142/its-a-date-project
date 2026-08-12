using Microsoft.EntityFrameworkCore;
using its_a_date_project.Models;

namespace its_a_date_project.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Theme> Themes => Set<Theme>();
        public DbSet<Invite> Invites => Set<Invite>();
        public DbSet<InviteText> InviteTexts => Set<InviteText>();
        public DbSet<DateSubmission> DateSubmissions => Set<DateSubmission>();
        public DbSet<AdminSetting> AdminSettings => Set<AdminSetting>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Invite>()
                .HasIndex(i => i.Slug)
                .IsUnique();

            modelBuilder.Entity<Invite>()
                .HasOne(i => i.Theme)
                .WithMany(t => t.Invites)
                .HasForeignKey(i => i.ThemeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InviteText>()
                .HasIndex(t => new { t.InviteId, t.Language, t.Key })
                .IsUnique();

            modelBuilder.Entity<InviteText>()
                .HasOne(t => t.Invite)
                .WithMany(i => i.Texts)
                .HasForeignKey(t => t.InviteId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DateSubmission>()
                .HasOne(s => s.Invite)
                .WithMany(i => i.Submissions)
                .HasForeignKey(s => s.InviteId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
