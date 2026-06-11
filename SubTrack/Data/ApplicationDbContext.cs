using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SubTrack.Models;

namespace SubTrack.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Subscription> Subscriptions => Set<Subscription>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(user => user.FullName)
                .HasMaxLength(120)
                .IsRequired();

            entity.Property(user => user.CurrencyCode)
                .HasMaxLength(3)
                .HasDefaultValue("USD")
                .IsRequired();
        });

        builder.Entity<Subscription>(entity =>
        {
            entity.Property(subscription => subscription.Name)
                .HasMaxLength(80)
                .IsRequired();

            entity.Property(subscription => subscription.Description)
                .HasMaxLength(500);

            entity.Property(subscription => subscription.Price)
                .HasPrecision(18, 2);

            entity.Property(subscription => subscription.CurrencyCode)
                .HasMaxLength(3)
                .IsRequired();

            entity.Property(subscription => subscription.StartDate)
                .HasColumnType("date");

            entity.Property(subscription => subscription.NextBillingDate)
                .HasColumnType("date");

            entity.Property(subscription => subscription.TrialStartDate)
                .HasColumnType("date");

            entity.Property(subscription => subscription.TrialEndDate)
                .HasColumnType("date");

            entity.Property(subscription => subscription.WebsiteUrl)
                .HasMaxLength(120);

            entity.Property(subscription => subscription.Notes)
                .HasMaxLength(1000);

            entity.HasOne(subscription => subscription.ApplicationUser)
                .WithMany(user => user.Subscriptions)
                .HasForeignKey(subscription => subscription.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(subscription => subscription.ApplicationUserId);
            entity.HasIndex(subscription => new { subscription.ApplicationUserId, subscription.Category });
            entity.HasIndex(subscription => new { subscription.ApplicationUserId, subscription.Status });
            entity.HasIndex(subscription => new { subscription.ApplicationUserId, subscription.NextBillingDate });
        });
    }
}
