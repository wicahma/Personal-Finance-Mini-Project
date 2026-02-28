using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PersonalFinance.Domain.Abstractions;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Infrastructure.Identity;

namespace PersonalFinance.Infrastructure.Persistence;

public class FinanceDbContext : IdentityDbContext<ApplicationUser>
{
    public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options) { }

    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<TransactionTag> TransactionTags => Set<TransactionTag>();
    public DbSet<Budget> Budgets => Set<Budget>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserProfile>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.IdentityUserId).IsUnique();
            e.Property(x => x.IdentityUserId).IsRequired().HasMaxLength(450);
            e.Property(x => x.DisplayName).HasMaxLength(256);
            e.Property(x => x.DefaultCurrency).HasMaxLength(10);
            e.Property(x => x.CreatedBy).HasMaxLength(450);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<Account>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(256);
            e.Property(x => x.Type).HasConversion<string>().HasMaxLength(32);
            e.Property(x => x.CurrentBalance).HasColumnType("decimal(18,2)");
            e.Property(x => x.CreatedBy).HasMaxLength(450);
            e.HasOne(x => x.UserProfile)
                .WithMany(x => x.Accounts)
                .HasForeignKey(x => x.UserProfileId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<Category>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(256);
            e.Property(x => x.Type).HasConversion<string>().HasMaxLength(32);
            e.Property(x => x.IconOrColor).HasMaxLength(128);
            e.Property(x => x.CreatedBy).HasMaxLength(450);
            e.HasOne(x => x.UserProfile)
                .WithMany(x => x.Categories)
                .HasForeignKey(x => x.UserProfileId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<Tag>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(128);
            e.Property(x => x.Color).HasMaxLength(32);
            e.Property(x => x.CreatedBy).HasMaxLength(450);
            e.HasOne(x => x.UserProfile)
                .WithMany(x => x.Tags)
                .HasForeignKey(x => x.UserProfileId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<Transaction>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Amount).HasColumnType("decimal(18,2)");
            e.Property(x => x.Notes).HasMaxLength(1024);
            e.Property(x => x.Type).HasConversion<string>().HasMaxLength(32);
            e.Property(x => x.CreatedBy).HasMaxLength(450);

            e.HasOne(x => x.TransferPair)
                .WithOne()
                .HasForeignKey<Transaction>(x => x.TransferPairId)
                .OnDelete(DeleteBehavior.NoAction);

            e.HasOne(x => x.UserProfile)
                .WithMany(x => x.Transactions)
                .HasForeignKey(x => x.UserProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Account)
                .WithMany(x => x.Transactions)
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Category)
                .WithMany(x => x.Transactions)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<TransactionTag>(e =>
        {
            e.HasKey(x => new { x.TransactionId, x.TagId });

            e.HasOne(x => x.Transaction)
                .WithMany(x => x.TransactionTags)
                .HasForeignKey(x => x.TransactionId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Tag)
                .WithMany(x => x.TransactionTags)
                .HasForeignKey(x => x.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Budget>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Month).IsRequired().HasMaxLength(7); // YYYY-MM
            e.Property(x => x.Amount).HasColumnType("decimal(18,2)");
            e.Property(x => x.CreatedBy).HasMaxLength(450);

            e.HasIndex(x => new { x.UserProfileId, x.CategoryId, x.Month }).IsUnique();

            e.HasOne(x => x.UserProfile)
                .WithMany(x => x.Budgets)
                .HasForeignKey(x => x.UserProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Category)
                .WithMany(x => x.Budgets)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasQueryFilter(x => !x.IsDeleted);
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        foreach (EntityEntry entry in ChangeTracker.Entries())
        {
            if (entry.Entity is IAuditableEntity auditable)
            {
                if (entry.State == EntityState.Added)
                {
                    auditable.CreatedAt = now;
                    auditable.UpdatedAt = now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    auditable.UpdatedAt = now;
                }
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
