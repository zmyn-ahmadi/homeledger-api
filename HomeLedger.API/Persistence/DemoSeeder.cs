using HomeLedger.API.Models;
using HomeLedger.API.Utils;
using Microsoft.EntityFrameworkCore;

namespace HomeLedger.API.Persistence;

// Local demo only, enabled with Demo__Enabled=true. Creates the schema from the
// model and adds an invented household. Real databases use EF Core migrations.
public static class DemoSeeder
{
    public const string Email = "demo@homeledger.local";
    public const string Password = "Demo1234!";

    public static async Task RunAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<HomeLedgerDbContext>();

        await db.Database.EnsureCreatedAsync();
        if (await db.Users.AnyAsync())
            return;

        var demo = new User { Email = Email, Password = PasswordManager.Hash(Password), FullName = "Alex Demo" };
        var partner = new User { Email = "sam@homeledger.local", Password = PasswordManager.Hash(Password), FullName = "Sam Demo" };
        db.Users.AddRange(demo, partner);
        await db.SaveChangesAsync();

        var categories = new[]
        {
            ("Groceries", "#22c55e", "shopping-cart", 95m),
            ("Eating out", "#f97316", "utensils", 48m),
            ("Transport", "#3b82f6", "fuel", 62m),
            ("Entertainment", "#a855f7", "film", 35m),
        }.Select(c => (Category: new Category { Name = c.Item1, Color = c.Item2, Icon = c.Item3, OwnerId = demo.UserId }, Weekly: c.Item4)).ToList();
        var rent = new Category { Name = "Housing", Color = "#ef4444", Icon = "home", OwnerId = demo.UserId };
        var utilities = new Category { Name = "Utilities", Color = "#eab308", Icon = "zap", OwnerId = demo.UserId };
        var salary = new Category { Name = "Salary", Color = "#10b981", Icon = "briefcase", OwnerId = demo.UserId };
        db.Categories.AddRange(categories.Select(c => c.Category));
        db.Categories.AddRange(rent, utilities, salary);
        await db.SaveChangesAsync();

        var now = DateTime.UtcNow;
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        for (var week = 0; week < 12; week++)
        {
            var date = now.AddDays(-week * 7 - 1);
            foreach (var (category, weekly) in categories)
            {
                db.Expenses.Add(new Expense
                {
                    Amount = weekly + week * 3 % 17,
                    Date = date.AddDays(-categories.IndexOf((category, weekly))),
                    Description = category.Name,
                    Note = "",
                    OwnerId = demo.UserId,
                    CategoryId = category.CategoryId,
                });
            }
        }

        for (var fortnight = 0; fortnight < 6; fortnight++)
        {
            db.Incomes.Add(new Income
            {
                Amount = 2650m,
                Date = now.AddDays(-fortnight * 14 - 2),
                Description = "Salary",
                Source = "Sample Employer",
                Note = "",
                IsRecurring = true,
                RecurringFrequency = RecurringFrequency.Weekly,
                OwnerId = demo.UserId,
                CategoryId = salary.CategoryId,
            });
        }

        db.Budgets.AddRange(categories.Take(3).Select(c => new Budget
        {
            Name = c.Category.Name,
            LimitAmount = c.Weekly * 5,
            Period = Period.Monthly,
            StartDate = monthStart,
            OwnerId = demo.UserId,
            CategoryId = c.Category.CategoryId,
        }));

        db.Bills.AddRange(
            new Bill { Name = "Internet", Amount = 89m, DueDate = now.AddDays(5), IsRecurring = true, Scope = "Personal", OwnerId = demo.UserId, CategoryId = utilities.CategoryId },
            new Bill { Name = "Phone", Amount = 45m, DueDate = now.AddDays(12), IsRecurring = true, Scope = "Personal", OwnerId = demo.UserId, CategoryId = utilities.CategoryId });

        var family = new Family { Name = "Demo household" };
        family.Members.Add(new FamilyMember { UserId = demo.UserId, Role = "Owner" });
        family.Members.Add(new FamilyMember { UserId = partner.UserId, Role = "Member" });
        db.Add(family);
        await db.SaveChangesAsync();

        db.Bills.AddRange(
            new Bill { Name = "Rent", Amount = 2100m, DueDate = now.AddDays(3), IsRecurring = true, Scope = "Family", FamilyId = family.FamilyId, CategoryId = rent.CategoryId },
            new Bill { Name = "Electricity", Amount = 310m, DueDate = now.AddDays(9), IsRecurring = true, Scope = "Family", FamilyId = family.FamilyId, CategoryId = utilities.CategoryId });
        await db.SaveChangesAsync();
    }
}
