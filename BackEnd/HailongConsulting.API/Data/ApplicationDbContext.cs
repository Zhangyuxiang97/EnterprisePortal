using HailongConsulting.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HailongConsulting.API.Data;

/// <summary>
/// 应用程序数据库上下文
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // 用户权限管理模块
    public DbSet<User> Users { get; set; }
    public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }

    // 附件管理模块
    public DbSet<Attachment> Attachments { get; set; }

    // 公告信息模块（统一公告表）
    public DbSet<Announcement> Announcements { get; set; }

    // 信息发布模块（统一信息发布表）
    public DbSet<InfoPublication> InfoPublications { get; set; }

    // 系统配置模块
    public DbSet<CompanyProfile> CompanyProfiles { get; set; }
    public DbSet<BusinessScope> BusinessScopes { get; set; }
    public DbSet<CompanyQualification> CompanyQualifications { get; set; }
    public DbSet<MajorAchievement> MajorAchievements { get; set; }
    public DbSet<CompanyHonor> CompanyHonors { get; set; }
    public DbSet<CarouselBanner> CarouselBanners { get; set; }
    public DbSet<FriendlyLink> FriendlyLinks { get; set; }

    // 统计模块
    public DbSet<VisitStatistic> VisitStatistics { get; set; }

    // 区域字典
    public DbSet<RegionDictionary> RegionDictionaries { get; set; }

    // 系统日志
    public DbSet<SystemLog> SystemLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User配置
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email);
            entity.HasIndex(e => e.RefreshToken);
            entity.HasIndex(e => e.LockoutUntil);
            entity.HasIndex(e => e.Role);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.IsDeleted);
        });

        modelBuilder.Entity<UserRefreshToken>(entity =>
        {
            entity.HasIndex(e => e.TokenHash).IsUnique();
            entity.HasIndex(e => new { e.UserId, e.RevokedAt });
            entity.HasIndex(e => e.ExpiresAt);
        });

        // Attachment配置
        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.ToTable("attachments");
            
            // 配置主键为自增
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();
            
            // 配置必填字段
            entity.Property(e => e.FileName)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("file_name");
            
            entity.Property(e => e.FilePath)
                .IsRequired()
                .HasMaxLength(500)
                .HasColumnName("file_path");
            
            entity.Property(e => e.FileUrl)
                .IsRequired()
                .HasMaxLength(500)
                .HasColumnName("file_url");
            
            entity.Property(e => e.IsDeleted)
                .IsRequired()
                .HasColumnName("is_deleted")
                .HasDefaultValue((sbyte)0);
            
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            // 配置索引
            entity.HasIndex(e => new { e.RelatedType, e.RelatedId })
                .HasDatabaseName("idx_related_type_id");
            entity.HasIndex(e => e.Category)
                .HasDatabaseName("idx_category");
            entity.HasIndex(e => e.UploadUserId)
                .HasDatabaseName("idx_upload_user_id");
            entity.HasIndex(e => e.IsDeleted)
                .HasDatabaseName("idx_is_deleted");
        });

        // Announcement配置（统一公告表）
        modelBuilder.Entity<Announcement>(entity =>
        {
            entity.HasIndex(e => e.BusinessType);
            entity.HasIndex(e => e.NoticeType);
            entity.HasIndex(e => e.Province);
            entity.HasIndex(e => e.City);
            entity.HasIndex(e => e.District);
            entity.HasIndex(e => e.ProjectRegion);
            entity.HasIndex(e => e.PublishTime);
            entity.HasIndex(e => e.Deadline);
            entity.HasIndex(e => e.BudgetAmount);
            entity.HasIndex(e => e.IsTop);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.IsDeleted);
        });

        // InfoPublication配置（统一信息发布表）
        modelBuilder.Entity<InfoPublication>(entity =>
        {
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.Author);
            entity.HasIndex(e => e.PublishTime);
            entity.HasIndex(e => e.IsTop);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.IsDeleted);
        });

        // CompanyProfile配置
        modelBuilder.Entity<CompanyProfile>(entity =>
        {
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.IsDeleted);
        });

        // BusinessScope配置
        modelBuilder.Entity<BusinessScope>(entity =>
        {
            entity.HasIndex(e => e.SortOrder);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.IsDeleted);
        });

        // CompanyQualification配置
        modelBuilder.Entity<CompanyQualification>(entity =>
        {
            entity.HasIndex(e => e.IssueDate);
            entity.HasIndex(e => e.SortOrder);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.IsDeleted);
        });

        // MajorAchievement配置
        modelBuilder.Entity<MajorAchievement>(entity =>
        {
            entity.HasIndex(e => e.ProjectType);
            entity.HasIndex(e => e.CompletionDate);
            entity.HasIndex(e => e.SortOrder);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.IsDeleted);
        });

        // CompanyHonor配置
        modelBuilder.Entity<CompanyHonor>(entity =>
        {
            entity.HasIndex(e => e.AwardDate);
            entity.HasIndex(e => e.HonorLevel);
            entity.HasIndex(e => e.SortOrder);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.IsDeleted);
        });

        // CarouselBanner配置
        modelBuilder.Entity<CarouselBanner>(entity =>
        {
            entity.HasIndex(e => e.SortOrder);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.IsDeleted);
        });

        // FriendlyLink配置
        modelBuilder.Entity<FriendlyLink>(entity =>
        {
            entity.HasIndex(e => e.SortOrder);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.IsDeleted);
        });

        // VisitStatistic配置
        modelBuilder.Entity<VisitStatistic>(entity =>
        {
            entity.HasIndex(e => e.VisitDate);
            entity.HasIndex(e => e.PageUrl);
            entity.HasIndex(e => e.VisitorIp);
            entity.HasIndex(e => e.IsDeleted);
        });

        // RegionDictionary配置
        modelBuilder.Entity<RegionDictionary>(entity =>
        {
            entity.HasIndex(e => e.RegionCode).IsUnique();
            entity.HasIndex(e => e.RegionLevel);
            entity.HasIndex(e => e.ParentCode);
            entity.HasIndex(e => e.SortOrder);
            entity.HasIndex(e => e.IsDeleted);
        });

        // SystemLog配置
        modelBuilder.Entity<SystemLog>(entity =>
        {
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Action);
            entity.HasIndex(e => e.Module);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
        });
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.Entity.GetType().GetProperty("UpdatedAt") != null)
            {
                entry.Property("UpdatedAt").CurrentValue = DateTime.Now;
            }

            if (entry.State == EntityState.Added && 
                entry.Entity.GetType().GetProperty("CreatedAt") != null)
            {
                entry.Property("CreatedAt").CurrentValue = DateTime.Now;
            }
        }
    }
}
