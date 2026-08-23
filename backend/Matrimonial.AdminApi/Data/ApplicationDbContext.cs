using System.Text;
using System.Text.RegularExpressions;
using Matrimonial.AdminApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matrimonial.AdminApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
    public DbSet<TenantSubscription> TenantSubscriptions => Set<TenantSubscription>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<EmailTemplate> EmailTemplates => Set<EmailTemplate>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();
    public DbSet<MemberPlan> MemberPlans => Set<MemberPlan>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<MemberSubscription> MemberSubscriptions => Set<MemberSubscription>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<UserPhoto> UserPhotos => Set<UserPhoto>();
    public DbSet<UserEducation> UserEducations => Set<UserEducation>();
    public DbSet<UserOccupation> UserOccupations => Set<UserOccupation>();
    public DbSet<UserFamilyDetail> UserFamilyDetails => Set<UserFamilyDetail>();
    public DbSet<UserLifestyle> UserLifestyles => Set<UserLifestyle>();
    public DbSet<UserLocation> UserLocations => Set<UserLocation>();
    public DbSet<UserPreference> UserPreferences => Set<UserPreference>();
    public DbSet<InterestRequest> InterestRequests => Set<InterestRequest>();
    public DbSet<Entities.Match> Matches => Set<Entities.Match>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<ProfileView> ProfileViews => Set<ProfileView>();
    public DbSet<Favorite> Favorites => Set<Favorite>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<BlockedUser> BlockedUsers => Set<BlockedUser>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<UserSubscription> UserSubscriptions => Set<UserSubscription>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AdminUser>(entity =>
        {
            entity.ToTable("AdminUsers");
            entity.HasKey(e => e.AdminId);
            entity.Property(e => e.AdminUserName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.HasIndex(e => e.AdminUserName).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.ToTable("Tenants");
            entity.HasKey(e => e.TenantId);
            entity.Property(e => e.TenantCode).HasMaxLength(50).IsRequired();
            entity.Property(e => e.CompanyName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.OwnerName).HasMaxLength(150).IsRequired();
            entity.Property(e => e.UserName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Password).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.ZipCode).HasMaxLength(20);
            entity.Property(e => e.LogoUrl).HasMaxLength(500);
            entity.Property(e => e.DatabaseName).HasMaxLength(100);
            entity.Property(e => e.DatabaseServer).HasMaxLength(200);
            entity.HasIndex(e => e.TenantCode).IsUnique();
            entity.HasOne(e => e.CreatedByAdmin)
                .WithMany(a => a.CreatedTenants)
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<SubscriptionPlan>(entity =>
        {
            entity.ToTable("SubscriptionPlans");
            entity.HasKey(e => e.PlanId);
            entity.Property(e => e.PlanName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Price).HasPrecision(18, 2);
        });

        modelBuilder.Entity<TenantSubscription>(entity =>
        {
            entity.ToTable("TenantSubscriptions");
            entity.HasKey(e => e.TenantSubscriptionsId);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.PaymentStatus).HasMaxLength(50);
            entity.Property(e => e.SubscriptionStatus).HasMaxLength(50);
            entity.HasOne(e => e.Tenant)
                .WithMany(t => t.Subscriptions)
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Plan)
                .WithMany(p => p.TenantSubscriptions)
                .HasForeignKey(e => e.PlanId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("Payments");
            entity.HasKey(e => e.PaymentId);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.Currency).HasMaxLength(10);
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.TransactionId).HasMaxLength(200);
            entity.Property(e => e.InvoiceNumber).HasMaxLength(100);
            entity.Property(e => e.PaymentGateway).HasMaxLength(100);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.HasOne(e => e.Subscription)
                .WithMany(s => s.Payments)
                .HasForeignKey(e => e.SubscriptionId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Tenant)
                .WithMany(t => t.Payments)
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EmailTemplate>(entity =>
        {
            entity.ToTable("EmailTemplates");
            entity.HasKey(e => e.TemplateId);
            entity.Property(e => e.TemplateName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Subject).HasMaxLength(500).IsRequired();
            entity.HasIndex(e => e.TemplateName).IsUnique();
        });

        modelBuilder.Entity<SystemSetting>(entity =>
        {
            entity.ToTable("SystemSettings");
            entity.HasKey(e => e.SettingId);
            entity.Property(e => e.SettingKey).HasMaxLength(200).IsRequired();
            entity.HasIndex(e => e.SettingKey).IsUnique();
        });

        modelBuilder.Entity<MemberPlan>(entity =>
        {
            entity.ToTable("MemberPlans");
            entity.HasKey(e => e.MemberPlanId);
            entity.Property(e => e.PlanName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.ToTable("Members");
            entity.HasKey(e => e.MemberId);
            entity.Property(e => e.UserCode).HasMaxLength(50).IsRequired();
            entity.Property(e => e.FullName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.ProfilePhotoUrl).HasMaxLength(500);
            entity.Property(e => e.ProfileStatus).HasMaxLength(50);
            entity.Property(e => e.PhotoStatus).HasMaxLength(50);
            entity.HasIndex(e => new { e.TenantId, e.UserCode }).IsUnique();
            entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MemberSubscription>(entity =>
        {
            entity.ToTable("MemberSubscriptions");
            entity.HasKey(e => e.MemberSubscriptionId);
            entity.Property(e => e.PaymentStatus).HasMaxLength(50);
            entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Member).WithMany(m => m.Subscriptions).HasForeignKey(e => e.MemberId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Plan).WithMany(p => p.Subscriptions).HasForeignKey(e => e.MemberPlanId).OnDelete(DeleteBehavior.Restrict);
        });

        ConfigureUserEntities(modelBuilder);

        ApplyPostgresSnakeCaseNaming(modelBuilder);
    }

    private static void ConfigureUserEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserCode).HasMaxLength(50).IsRequired();
            entity.Property(e => e.UserName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.HasIndex(e => new { e.TenantId, e.UserName }).IsUnique();
            entity.HasIndex(e => new { e.TenantId, e.UserCode }).IsUnique();
            entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.ToTable("UserProfiles");
            entity.HasKey(e => e.ProfileId);
            entity.Property(e => e.Gender).HasMaxLength(20);
            entity.Property(e => e.Height).HasPrecision(5, 2);
            entity.Property(e => e.Weight).HasPrecision(5, 2);
            entity.Property(e => e.MaritalStatus).HasMaxLength(50);
            entity.Property(e => e.Religion).HasMaxLength(100);
            entity.Property(e => e.Caste).HasMaxLength(100);
            entity.Property(e => e.SubCaste).HasMaxLength(100);
            entity.Property(e => e.MotherTongue).HasMaxLength(100);
            entity.Property(e => e.BloodGroup).HasMaxLength(20);
            entity.Property(e => e.ProfileStatus).HasMaxLength(50);
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.HasOne(e => e.User).WithOne(u => u.Profile).HasForeignKey<UserProfile>(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserPhoto>(entity =>
        {
            entity.ToTable("UserPhotos");
            entity.HasKey(e => e.PhotoId);
            entity.Property(e => e.PhotoUrl).HasMaxLength(500).IsRequired();
            entity.Property(e => e.ContentType).HasMaxLength(100);
            entity.Property(e => e.ImageData).HasColumnType("bytea");
            entity.HasOne(e => e.User).WithMany(u => u.Photos).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserEducation>(entity =>
        {
            entity.ToTable("UserEducations");
            entity.HasKey(e => e.EducationId);
            entity.Property(e => e.Qualification).HasMaxLength(150);
            entity.Property(e => e.College).HasMaxLength(200);
            entity.Property(e => e.University).HasMaxLength(200);
            entity.Property(e => e.EducationType).HasMaxLength(100);
            entity.HasOne(e => e.User).WithMany(u => u.Educations).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserOccupation>(entity =>
        {
            entity.ToTable("UserOccupations");
            entity.HasKey(e => e.OccupationId);
            entity.Property(e => e.Occupation).HasMaxLength(150);
            entity.Property(e => e.CompanyName).HasMaxLength(200);
            entity.Property(e => e.Designation).HasMaxLength(150);
            entity.Property(e => e.AnnualIncome).HasPrecision(18, 2);
            entity.Property(e => e.WorkLocation).HasMaxLength(200);
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.HasOne(e => e.User).WithOne(u => u.Occupation).HasForeignKey<UserOccupation>(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserFamilyDetail>(entity =>
        {
            entity.ToTable("UserFamilyDetails");
            entity.HasKey(e => e.FamilyId);
            entity.Property(e => e.FamilyType).HasMaxLength(50);
            entity.Property(e => e.FamilyStatus).HasMaxLength(100);
            entity.Property(e => e.FatherName).HasMaxLength(100);
            entity.Property(e => e.FatherOccupation).HasMaxLength(150);
            entity.Property(e => e.MotherName).HasMaxLength(100);
            entity.Property(e => e.MotherOccupation).HasMaxLength(150);
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.HasOne(e => e.User).WithOne(u => u.Family).HasForeignKey<UserFamilyDetail>(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserLifestyle>(entity =>
        {
            entity.ToTable("UserLifestyles");
            entity.HasKey(e => e.LifestyleId);
            entity.Property(e => e.Diet).HasMaxLength(50);
            entity.Property(e => e.LanguagesKnown).HasMaxLength(500);
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.HasOne(e => e.User).WithOne(u => u.Lifestyle).HasForeignKey<UserLifestyle>(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserLocation>(entity =>
        {
            entity.ToTable("UserLocations");
            entity.HasKey(e => e.LocationId);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Pincode).HasMaxLength(20);
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.HasOne(e => e.User).WithOne(u => u.Location).HasForeignKey<UserLocation>(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserPreference>(entity =>
        {
            entity.ToTable("UserPreferences");
            entity.HasKey(e => e.PreferenceId);
            entity.Property(e => e.MinHeight).HasPrecision(5, 2);
            entity.Property(e => e.MaxHeight).HasPrecision(5, 2);
            entity.Property(e => e.Religion).HasMaxLength(100);
            entity.Property(e => e.Caste).HasMaxLength(100);
            entity.Property(e => e.Education).HasMaxLength(200);
            entity.Property(e => e.Occupation).HasMaxLength(200);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.HasOne(e => e.User).WithOne(u => u.Preference).HasForeignKey<UserPreference>(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<InterestRequest>(entity =>
        {
            entity.ToTable("InterestRequests");
            entity.HasKey(e => e.InterestId);
            entity.Property(e => e.Status).HasMaxLength(30);
            entity.HasIndex(e => new { e.SenderUserId, e.ReceiverUserId }).IsUnique();
            entity.HasOne(e => e.Sender).WithMany().HasForeignKey(e => e.SenderUserId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Receiver).WithMany().HasForeignKey(e => e.ReceiverUserId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Entities.Match>(entity =>
        {
            entity.ToTable("Matches");
            entity.HasKey(e => e.MatchId);
            entity.Property(e => e.MatchPercentage).HasPrecision(5, 2);
            entity.HasIndex(e => new { e.UserId1, e.UserId2 }).IsUnique();
            entity.HasOne(e => e.User1).WithMany().HasForeignKey(e => e.UserId1).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.User2).WithMany().HasForeignKey(e => e.UserId2).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.ToTable("Messages");
            entity.HasKey(e => e.MessageId);
            entity.Property(e => e.MessageText).HasColumnName("Message");
            entity.HasOne(e => e.Sender).WithMany().HasForeignKey(e => e.SenderUserId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Receiver).WithMany().HasForeignKey(e => e.ReceiverUserId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProfileView>(entity =>
        {
            entity.ToTable("ProfileViews");
            entity.HasKey(e => e.ViewId);
            entity.HasOne(e => e.Viewer).WithMany().HasForeignKey(e => e.ViewerUserId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Viewed).WithMany().HasForeignKey(e => e.ViewedUserId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.ToTable("Favorites");
            entity.HasKey(e => e.FavoriteId);
            entity.HasIndex(e => new { e.UserId, e.FavoriteUserId }).IsUnique();
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.FavoriteUser).WithMany().HasForeignKey(e => e.FavoriteUserId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<UserSession>(entity =>
        {
            entity.ToTable("UserSessions");
            entity.HasKey(e => e.SessionId);
            entity.Property(e => e.DeviceName).HasMaxLength(200);
            entity.Property(e => e.Browser).HasMaxLength(100);
            entity.Property(e => e.IPAddress).HasMaxLength(50);
            entity.HasOne(e => e.User).WithMany(u => u.Sessions).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("Notifications");
            entity.HasKey(e => e.NotificationId);
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
            entity.Property(e => e.MessageText).HasColumnName("Message");
            entity.HasOne(e => e.User).WithMany(u => u.Notifications).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BlockedUser>(entity =>
        {
            entity.ToTable("BlockedUsers");
            entity.HasKey(e => e.BlockedId);
            entity.HasIndex(e => new { e.UserId, e.BlockedUserId }).IsUnique();
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Blocked).WithMany().HasForeignKey(e => e.BlockedUserId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.ToTable("Reports");
            entity.HasKey(e => e.ReportId);
            entity.Property(e => e.Reason).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.HasOne(e => e.Reporter).WithMany().HasForeignKey(e => e.ReporterUserId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Reported).WithMany().HasForeignKey(e => e.ReportedUserId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<UserSubscription>(entity =>
        {
            entity.ToTable("UserSubscriptions");
            entity.HasKey(e => e.UserSubscriptionId);
            entity.Property(e => e.PaymentStatus).HasMaxLength(50);
            entity.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.User).WithMany(u => u.Subscriptions).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Plan).WithMany().HasForeignKey(e => e.MemberPlanId).OnDelete(DeleteBehavior.Restrict);
        });
    }

    /// <summary>
    /// Maps PascalCase C# names to snake_case PostgreSQL identifiers (best practice).
    /// Unquoted SQL like "SELECT * FROM admin_users" works naturally.
    /// </summary>
    private static void ApplyPostgresSnakeCaseNaming(ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entity.GetTableName();
            if (!string.IsNullOrEmpty(tableName))
                entity.SetTableName(ToSnakeCase(tableName));

            foreach (var property in entity.GetProperties())
            {
                var columnName = property.GetColumnName();
                if (!string.IsNullOrEmpty(columnName))
                    property.SetColumnName(ToSnakeCase(columnName));
            }

            foreach (var key in entity.GetKeys())
            {
                var keyName = key.GetName();
                if (!string.IsNullOrEmpty(keyName))
                    key.SetName(ToSnakeCase(keyName));
            }

            foreach (var foreignKey in entity.GetForeignKeys())
            {
                var constraintName = foreignKey.GetConstraintName();
                if (!string.IsNullOrEmpty(constraintName))
                    foreignKey.SetConstraintName(ToSnakeCase(constraintName));
            }

            foreach (var index in entity.GetIndexes())
            {
                var indexName = index.GetDatabaseName();
                if (!string.IsNullOrEmpty(indexName))
                    index.SetDatabaseName(ToSnakeCase(indexName));
            }
        }
    }

    private static string ToSnakeCase(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;

        var normalized = Regex.Replace(input.Trim(), @"[\s\-]+", "_");
        var snake = Regex.Replace(normalized, @"([a-z0-9])([A-Z])", "$1_$2");
        snake = Regex.Replace(snake, @"([A-Z]+)([A-Z][a-z0-9]+)", "$1_$2");
        return snake.ToLowerInvariant();
    }
}
