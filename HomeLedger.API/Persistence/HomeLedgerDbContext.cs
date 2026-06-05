using HomeLedger.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace HomeLedger.API.Persistence;

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
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Account> Accounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User Configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.UserId).UseIdentityAlwaysColumn();

            // String properties with max lengths
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256); // Good practice to set max length for email

            entity.Property(e => e.Password).IsRequired().HasMaxLength(500); // For hashed passwords

            entity.Property(e => e.FullName).HasMaxLength(200);

            // DateTime properties
            entity.Property(e => e.LastLogin);
            entity.Property(e => e.IsSuperUser);

            // Add unique indexes
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

            entity
                .HasMany(e => e.OwnedBills)
                .WithOne(e => e.Owner)
                .HasForeignKey(e => e.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1:M
            entity
                .HasMany(e => e.Accounts)
                .WithOne(e => e.Owner)
                .HasForeignKey(e => e.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasMany(e => e.Categories)
                .WithOne(e => e.Owner)
                .HasForeignKey(e => e.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasMany(e => e.Expenses)
                .WithOne(e => e.Owner)
                .HasForeignKey(e => e.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasMany(e => e.Incomes)
                .WithOne(e => e.Owner)
                .HasForeignKey(e => e.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasMany(e => e.Budgets)
                .WithOne(e => e.Owner)
                .HasForeignKey(e => e.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasMany(e => e.BillPayments)
                .WithOne(e => e.PaidBy)
                .HasForeignKey(e => e.PaidById)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // UserProfile Configuration
        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.UserProfileId);

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
            entity.HasKey(e => e.BillId);

            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);

            entity.Property(e => e.Amount).IsRequired().HasPrecision(18, 2);

            entity.Property(e => e.DueDate).IsRequired();

            entity.Property(e => e.IsPaid).HasDefaultValue(false);

            entity.Property(e => e.IsRecurring).HasDefaultValue(false);

            entity.Property(e => e.RecurringFrequency).HasConversion<int>();

            entity.Property(e => e.Notes).HasMaxLength(1000);

            entity.Property(e => e.Status);

            entity.Property(e => e.OwnerId).IsRequired();

            entity
                .HasOne(b => b.Catgory)
                .WithMany(c => c.Bills)
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(e => e.Owner)
                .WithMany(u => u.OwnedBills)
                .HasForeignKey(e => e.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // M:M - TRASH
            // entity
            //     .HasMany(e => e.SharedWith)
            //     .WithMany(u => u.SharedBills)
            //     .UsingEntity<Dictionary<string, object>>(
            //         "BillShares",
            //         j => j.HasOne<User>().WithMany().HasForeignKey("OwnerId"),
            //         j => j.HasOne<Bill>().WithMany().HasForeignKey("BillId"),
            //         j =>
            //         {
            //             j.HasKey("BillId", "OwnerId");
            //             j.ToTable("BillShares");
            //             j.HasIndex("OwnerId").HasDatabaseName("IX_BillShares_OwnerId");
            //             j.HasIndex("BillId").HasDatabaseName("IX_BillShares_BillId");
            //         }
            //     );

            entity.HasIndex(e => e.OwnerId).HasDatabaseName("IX_Bills_OwnerId");

            entity.HasIndex(e => e.CategoryId).HasDatabaseName("IX_Bills_CategoryId");

            entity.HasIndex(e => e.DueDate).HasDatabaseName("IX_Bills_DueDate");

            entity.HasIndex(e => e.IsPaid).HasDatabaseName("IX_Bills_IsPaid");
        });

        // BillPayment Configuration
        modelBuilder.Entity<BillPayment>(entity =>
        {
            entity.HasKey(e => e.BillPaymentId);

            entity.Property(e => e.Amount).IsRequired().HasPrecision(18, 2);

            entity.Property(e => e.PaidDate).IsRequired();

            entity.Property(e => e.Notes).HasMaxLength(500);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");

            // Foreign key relationships
            entity
                .HasOne(e => e.Bill)
                .WithMany(b => b.BillPayments)
                .HasForeignKey(e => e.BillId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(e => e.PaidBy)
                .WithMany(u => u.BillPayments)
                .HasForeignKey(e => e.PaidById)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes

            entity.HasIndex(e => e.BillPaymentId).IsUnique().HasDatabaseName("uq_bill_payments");

            entity.HasIndex(e => e.BillId).HasDatabaseName("IX_BillPayments_BillId");

            entity.HasIndex(e => e.PaidById).HasDatabaseName("IX_BillPayments_PaidById");

            entity.HasIndex(e => e.PaidDate).HasDatabaseName("IX_BillPayments_PaidDate");
        });

        // Budget Configuration
        modelBuilder.Entity<Budget>(entity =>
        {
            entity.HasKey(e => e.BudgetId);

            entity.Property(e => e.LimitAmount).IsRequired().HasPrecision(18, 2);

            entity.Property(e => e.Period).HasConversion<int>();

            entity.Property(e => e.StartDate);
            entity.Property(e => e.EndDate);

            entity.Property(e => e.AlertThreshold).HasDefaultValue(80);

            // Foreign key relationships
            entity
                .HasOne(e => e.Owner)
                .WithMany(u => u.Budgets)
                .HasForeignKey(e => e.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(e => e.Category)
                .WithMany(c => c.Budgets)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            entity.HasIndex(e => e.OwnerId).HasDatabaseName("IX_Budgets_OwnerId");

            entity.HasIndex(e => e.CategoryId).HasDatabaseName("IX_Budgets_CategoryId");

            entity
                .HasIndex(e => new { e.OwnerId, e.CategoryId })
                .HasDatabaseName("IX_Budgets_User_Category");

            // Ensure no overlapping budgets for same user/category
            entity
                .HasIndex(e => new { e.OwnerId, e.CategoryId })
                .HasDatabaseName("IX_Budgets_DateRange");
        });

        // Category Configuration (updated with Budgets relationship)
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId);

            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);

            entity.Property(e => e.Color).HasMaxLength(7).HasDefaultValue("#6366f1");

            entity.Property(e => e.Icon).HasMaxLength(50);

            entity.Property(e => e.Type).HasConversion<int>();

            entity.Property(e => e.OwnerId).IsRequired();

            // Relationships
            entity
                .HasOne(e => e.Owner)
                .WithMany(u => u.Categories)
                .HasForeignKey(e => e.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            entity.HasIndex(e => e.OwnerId).HasDatabaseName("IX_Categories_OwnerId");

            entity
                .HasIndex(e => new { e.OwnerId, e.Name })
                .IsUnique()
                .HasDatabaseName("IX_Categories_User_Name");
        });

        // Expense Configuration
        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.ExpenseId);

            entity.Property(e => e.Amount).IsRequired().HasPrecision(18, 2);

            entity.Property(e => e.Date).IsRequired();

            entity.Property(e => e.Description).HasMaxLength(500);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");

            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");

            // Foreign key relationships
            entity
                .HasOne(e => e.Owner)
                .WithMany(u => u.Expenses)
                .HasForeignKey(e => e.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(e => e.Category)
                .WithMany(c => c.Expenses)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes

            // entity.HasIndex(e => e.ExpenseId).IsUnique().HasDatabaseName("uq_expense");
            entity.HasIndex(e => e.OwnerId).HasDatabaseName("IX_Expenses_OwnerId");

            entity.HasIndex(e => e.CategoryId).HasDatabaseName("IX_Expenses_CategoryId");

            entity.HasIndex(e => e.Date).HasDatabaseName("IX_Expenses_Date");

            entity
                .HasIndex(e => new { e.OwnerId, e.Date })
                .HasDatabaseName("IX_Expenses_User_Date");
        });

        // Income Configuration
        modelBuilder.Entity<Income>(entity =>
        {
            entity.HasKey(e => e.IncomeId);

            entity.Property(e => e.Amount).IsRequired().HasPrecision(18, 2);

            entity.Property(e => e.Date).IsRequired();

            entity.Property(e => e.Description).HasMaxLength(500);

            entity.Property(e => e.IsRecurring).HasDefaultValue(false);

            entity.Property(e => e.RecurringFrequency).HasConversion<int>();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");

            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");

            // Foreign key relationships
            entity
                .HasOne(e => e.Owner)
                .WithMany() // Add Incomes collection to User if needed
                .HasForeignKey(e => e.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            entity.HasIndex(e => e.OwnerId).HasDatabaseName("IX_Incomes_UserId");

            entity.HasIndex(e => e.Date).HasDatabaseName("IX_Incomes_Date");

            entity.HasIndex(e => e.IsRecurring).HasDatabaseName("IX_Incomes_IsRecurring");

            entity.HasIndex(e => new { e.OwnerId, e.Date }).HasDatabaseName("IX_Incomes_User_Date");
        });

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId);

            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.Currency);
            entity.Property(e => e.AccountBalance).IsRequired().HasPrecision(18, 2);

            entity
                .HasOne(e => e.Owner)
                .WithMany()
                .HasForeignKey(e => e.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasIndex(a => new { a.OwnerId, a.Name })
                .IsUnique()
                .HasDatabaseName("uq_account");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId);

            entity.Property(e => e.Amount).IsRequired();
            entity.Property(e => e.Date).IsRequired();
            entity.Property(e => e.Description);

            entity
                .HasOne(e => e.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(e => e.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasIndex(t => new
                {
                    t.AccountId,
                    t.Date,
                    t.Amount,
                    t.Description,
                })
                .IsUnique()
                .HasDatabaseName("uq_transaction");
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
        var entries = ChangeTracker.Entries<BaseAuditableEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.IsActive = true;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
