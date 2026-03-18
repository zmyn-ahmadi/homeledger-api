using HomeLedger.API.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeLedger.API.Data;

public class HomeLedgerDbContext(DbContextOptions<HomeLedgerDbContext> options) : DbContext(options)
{
  // Define DbSets for entities
  public DbSet<User> Users { get; set; }
  public DbSet<Category> Categories { get; set; }
  public DbSet<Bill> Bills { get; set; }
  public DbSet<BillPayment> Payments { get; set; }
  public DbSet<Budget> Budgets { get; set; }
  public DbSet<Expense> Expenses { get; set; }
  public DbSet<Income> Incomes { get; set; }
  public DbSet<UserProfile> UserProfiles { get; set; }
  public DbSet<RefreshToken> RefreshTokens { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    // User Configuration
    modelBuilder.Entity<User>(entity =>
    {
      entity.Property(e => e.Id).UseIdentityAlwaysColumn();

      // String properties with max lengths
      entity.Property(e => e.UserName).IsRequired().HasMaxLength(150);

      entity.Property(e => e.Email).IsRequired().HasMaxLength(256); // Good practice to set max length for email

      entity.Property(e => e.Password).IsRequired().HasMaxLength(500); // For hashed passwords

      entity.Property(e => e.FirstName).HasMaxLength(100);

      entity.Property(e => e.LastName).HasMaxLength(100);

      // DateTime properties
      entity.Property(e => e.LastLogin);

      entity.Property(e => e.IsSuperUser);

      // PostgreSQL uses NOW() for current timestamp
      entity.Property(e => e.DateJoined).HasDefaultValueSql("NOW()");

      // Add unique indexes
      entity.HasIndex(e => e.UserName).IsUnique().HasDatabaseName("IX_Users_UserName");

      entity.HasIndex(e => e.Email).IsUnique().HasDatabaseName("IX_Users_Email");

      // Configure relationship with RefreshTokens
      entity
              .HasMany(e => e.RefreshTokens)
              .WithOne(e => e.User)
              .HasForeignKey(e => e.UserId)
              .OnDelete(DeleteBehavior.Cascade);

      entity
              .HasOne(e => e.Profile)
              .WithOne(e => e.User)
              .HasForeignKey<UserProfile>(e => e.UserId)
              .OnDelete(DeleteBehavior.Cascade);
    });

    // UserProfile Configuration
    modelBuilder.Entity<UserProfile>(entity =>
    {
      entity.HasKey(e => e.Id);

      // Foreign key to User
      entity.Property(e => e.UserId).IsRequired();

      entity.Property(e => e.Phone).HasMaxLength(20); // Reasonable length for phone numbers

      entity.Property(e => e.DOB);

      entity.Property(e => e.Address).HasMaxLength(500);

      // PostgreSQL uses NOW() for timestamps
      entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");

      entity.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");

      // One-to-one relationship (already configured in User)
      // This ensures UserId is unique (one profile per user)
      entity.HasIndex(e => e.UserId).IsUnique().HasDatabaseName("IX_UserProfiles_UserId");

      // Optional: Add index on frequently queried fields
      entity.HasIndex(e => e.Phone).HasDatabaseName("IX_UserProfiles_Phone");
    });

    // Bill Configuration
    modelBuilder.Entity<Bill>(entity =>
    {
      entity.HasKey(e => e.Id);

      entity.Property(e => e.Name).IsRequired().HasMaxLength(200);

      entity.Property(e => e.Amount).IsRequired().HasPrecision(18, 2);

      entity.Property(e => e.DueDate).IsRequired();

      entity.Property(e => e.IsPaid).HasDefaultValue(false);

      entity.Property(e => e.IsRecurring).HasDefaultValue(false);

      entity.Property(e => e.RecurringFrequency).HasConversion<int>();

      entity.Property(e => e.Notes).HasMaxLength(1000);

      entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");

      entity.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");

      entity.Property(e => e.OwnerId).IsRequired();

      entity.Property(e => e.CategoryId).IsRequired();

      entity
              .HasOne(e => e.Owner)
              .WithMany()
              .HasForeignKey(e => e.OwnerId)
              .OnDelete(DeleteBehavior.Restrict);

      entity
              .HasOne(e => e.Category)
              .WithMany()
              .HasForeignKey(e => e.CategoryId)
              .OnDelete(DeleteBehavior.Restrict);

      entity
              .HasMany(e => e.SharedWith)
              .WithMany()
              .UsingEntity<Dictionary<string, object>>(
                  "BillShares",
                  j => j.HasOne<User>().WithMany().HasForeignKey("UserId"),
                  j => j.HasOne<Bill>().WithMany().HasForeignKey("BillId"),
                  j =>
                  {
                    j.HasKey("BillId", "UserId");
                    j.ToTable("BillShares");
                    j.HasIndex("UserId").HasDatabaseName("IX_BillShares_UserId");
                    j.HasIndex("BillId").HasDatabaseName("IX_BillShares_BillId");
                  }
              );

      entity.HasIndex(e => e.OwnerId).HasDatabaseName("IX_Bills_OwnerId");

      entity.HasIndex(e => e.CategoryId).HasDatabaseName("IX_Bills_CategoryId");

      entity.HasIndex(e => e.DueDate).HasDatabaseName("IX_Bills_DueDate");

      entity.HasIndex(e => e.IsPaid).HasDatabaseName("IX_Bills_IsPaid");
    });

    // BillPayment Configuration
    modelBuilder.Entity<BillPayment>(entity =>
    {
      entity.HasKey(e => e.Id);

      entity.Property(e => e.Amount).IsRequired().HasPrecision(18, 2);

      entity.Property(e => e.PaidDate).IsRequired();

      entity.Property(e => e.Notes).HasMaxLength(500);

      entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");

      // Foreign key relationships
      entity
              .HasOne(e => e.Bill)
              .WithMany() // Add BillPayments collection to Bill if needed
              .HasForeignKey(e => e.BillId)
              .OnDelete(DeleteBehavior.Restrict);

      entity
              .HasOne(e => e.PaidBy)
              .WithMany() // Add BillPayments collection to User if needed
              .HasForeignKey(e => e.PaidById)
              .OnDelete(DeleteBehavior.Restrict);

      // Indexes
      entity.HasIndex(e => e.BillId).HasDatabaseName("IX_BillPayments_BillId");

      entity.HasIndex(e => e.PaidById).HasDatabaseName("IX_BillPayments_PaidById");

      entity.HasIndex(e => e.PaidDate).HasDatabaseName("IX_BillPayments_PaidDate");
    });

    // Budget Configuration
    modelBuilder.Entity<Budget>(entity =>
    {
      entity.HasKey(e => e.Id);

      entity.Property(e => e.Amount).IsRequired().HasPrecision(18, 2);

      entity.Property(e => e.Period).HasConversion<int>();

      entity.Property(e => e.StartDate).IsRequired();

      entity.Property(e => e.EndDate).IsRequired();

      entity.Property(e => e.AlertThreshold).HasDefaultValue(80);

      entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");

      entity.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");

      // Foreign key relationships
      entity
              .HasOne(e => e.User)
              .WithMany() // Add Budgets collection to User if needed
              .HasForeignKey(e => e.UserId)
              .OnDelete(DeleteBehavior.Restrict);

      entity
              .HasOne(e => e.Category)
              .WithMany() // Add Budgets collection to Category if needed
              .HasForeignKey(e => e.CategoryId)
              .OnDelete(DeleteBehavior.Restrict);

      // Indexes
      entity.HasIndex(e => e.UserId).HasDatabaseName("IX_Budgets_UserId");

      entity.HasIndex(e => e.CategoryId).HasDatabaseName("IX_Budgets_CategoryId");

      entity
              .HasIndex(e => new { e.UserId, e.CategoryId })
              .HasDatabaseName("IX_Budgets_User_Category");

      // Ensure no overlapping budgets for same user/category
      entity
              .HasIndex(e => new
              {
                e.UserId,
                e.CategoryId,
                e.StartDate,
                e.EndDate,
              })
              .HasDatabaseName("IX_Budgets_DateRange");
    });

    // Category Configuration (updated with Budgets relationship)
    modelBuilder.Entity<Category>(entity =>
    {
      entity.HasKey(e => e.Id);

      entity.Property(e => e.Name).IsRequired().HasMaxLength(100);

      entity.Property(e => e.Color).HasMaxLength(7).HasDefaultValue("#6366f1");

      entity.Property(e => e.Icon).HasMaxLength(50);

      entity.Property(e => e.Type).HasConversion<int>();

      entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");

      entity.Property(e => e.UserId).IsRequired();

      // Relationships
      entity
              .HasOne(e => e.User)
              .WithMany() // Add Categories collection to User if needed
              .HasForeignKey(e => e.UserId)
              .OnDelete(DeleteBehavior.Restrict);

      // Indexes
      entity.HasIndex(e => e.UserId).HasDatabaseName("IX_Categories_UserId");

      entity
              .HasIndex(e => new { e.UserId, e.Name })
              .IsUnique()
              .HasDatabaseName("IX_Categories_User_Name");
    });

    // Expense Configuration
    modelBuilder.Entity<Expense>(entity =>
    {
      entity.HasKey(e => e.Id);

      entity.Property(e => e.Amount).IsRequired().HasPrecision(18, 2);

      entity.Property(e => e.Date).IsRequired();

      entity.Property(e => e.Description).HasMaxLength(500);

      entity.Property(e => e.IsRecurring).HasDefaultValue(false);

      entity.Property(e => e.RecurringFrequency).HasConversion<int>();

      entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");

      entity.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");

      // Foreign key relationships
      entity
              .HasOne(e => e.User)
              .WithMany() // Add Expenses collection to User if needed
              .HasForeignKey(e => e.UserId)
              .OnDelete(DeleteBehavior.Restrict);

      entity
              .HasOne(e => e.Category)
              .WithMany() // Add Expenses collection to Category if needed
              .HasForeignKey(e => e.CategoryId)
              .OnDelete(DeleteBehavior.Restrict);

      // Indexes
      entity.HasIndex(e => e.UserId).HasDatabaseName("IX_Expenses_UserId");

      entity.HasIndex(e => e.CategoryId).HasDatabaseName("IX_Expenses_CategoryId");

      entity.HasIndex(e => e.Date).HasDatabaseName("IX_Expenses_Date");

      entity.HasIndex(e => e.IsRecurring).HasDatabaseName("IX_Expenses_IsRecurring");

      entity.HasIndex(e => new { e.UserId, e.Date }).HasDatabaseName("IX_Expenses_User_Date");
    });

    // Income Configuration
    modelBuilder.Entity<Income>(entity =>
    {
      entity.HasKey(e => e.Id);

      entity.Property(e => e.Source).IsRequired().HasMaxLength(200);

      entity.Property(e => e.Amount).IsRequired().HasPrecision(18, 2);

      entity.Property(e => e.Date).IsRequired();

      entity.Property(e => e.Description).HasMaxLength(500);

      entity.Property(e => e.IsRecurring).HasDefaultValue(false);

      entity.Property(e => e.RecurringFrequency).HasConversion<int>();

      entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");

      entity.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");

      // Foreign key relationships
      entity
              .HasOne(e => e.User)
              .WithMany() // Add Incomes collection to User if needed
              .HasForeignKey(e => e.UserId)
              .OnDelete(DeleteBehavior.Restrict);

      entity
              .HasOne(e => e.Category)
              .WithMany() // Add Incomes collection to Category if needed
              .HasForeignKey(e => e.CategoryId)
              .OnDelete(DeleteBehavior.Restrict);

      // Indexes
      entity.HasIndex(e => e.UserId).HasDatabaseName("IX_Incomes_UserId");

      entity.HasIndex(e => e.CategoryId).HasDatabaseName("IX_Incomes_CategoryId");

      entity.HasIndex(e => e.Date).HasDatabaseName("IX_Incomes_Date");

      entity.HasIndex(e => e.IsRecurring).HasDatabaseName("IX_Incomes_IsRecurring");

      entity.HasIndex(e => new { e.UserId, e.Date }).HasDatabaseName("IX_Incomes_User_Date");
    });

    // RefreshToken Configuration
    modelBuilder.Entity<RefreshToken>(entity =>
    {
      entity.HasKey(e => e.Id);

      entity.Property(e => e.Token).IsRequired().HasMaxLength(500);

      entity.Property(e => e.Expiry).IsRequired();

      entity.Property(e => e.Created).HasDefaultValueSql("NOW()");

      entity.Property(e => e.IsRevoked);

      entity.Property(e => e.UserId).IsRequired();

      entity
              .HasOne(e => e.User)
              .WithMany(u => u.RefreshTokens)
              .HasForeignKey(e => e.UserId)
              .OnDelete(DeleteBehavior.Cascade);

      entity.HasIndex(e => e.Token).IsUnique().HasDatabaseName("IX_RefreshTokens_Token");

      entity.HasIndex(e => e.UserId).HasDatabaseName("IX_RefreshTokens_UserId");
    });
  }

  public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    var entries = ChangeTracker
        .Entries()
        .Where(e =>
            (
                e.Entity is Bill
                || e.Entity is Income
                || e.Entity is Expense
                || e.Entity is Budget
                || e.Entity is UserProfile
            )
            && e.State == EntityState.Modified
        );

    foreach (var entityEntry in entries)
    {
      switch (entityEntry.Entity)
      {
        case Bill bill:
          bill.UpdatedAt = DateTime.UtcNow;
          break;
        case UserProfile profile:
          profile.UpdatedAt = DateTime.UtcNow;
          break;
        case Budget budget:
          budget.UpdatedAt = DateTime.UtcNow;
          break;
        case Expense expense:
          expense.UpdatedAt = DateTime.UtcNow;
          break;
        case Income income:
          income.UpdatedAt = DateTime.UtcNow;
          break;
      }
    }

    return base.SaveChangesAsync(cancellationToken);
  }
}
