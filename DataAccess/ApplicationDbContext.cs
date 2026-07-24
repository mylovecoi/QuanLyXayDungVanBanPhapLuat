using DataAccess.Entities;
using DataAccess.Entities.Settings;
using DataAccess.Entities.QuanLyDanhMuc;
using DataAccess.Entities.Manages;
using DataAccess.Entities.Systems;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);            
        }

        public override int SaveChanges()
        {
            UpdateAuditFields();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateAuditFields()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var session = httpContext?.Session;
            Guid? userId = null;
            if (session != null)
            {
                userId = Helper.GetSsAdminGuid(session);
            }
            if (userId == null || userId == Guid.Empty)
            {
                var userClaim = httpContext?.User?.FindFirst("UserId");
                if (userClaim != null && Guid.TryParse(userClaim.Value, out Guid tokenUserId))
                {
                    userId = tokenUserId;
                }
            }

            if (userId != null && userId != Guid.Empty)
            {
                var entries = ChangeTracker.Entries()
                    .Where(e => e.Entity is BaseEntity &&
                           (e.State == EntityState.Added || e.State == EntityState.Modified))
                    .ToList();

                foreach (var entry in entries)
                {
                    var entity = (BaseEntity)entry.Entity;
                    entity.UpdatedDate = DateTime.Now;
                    entity.UpdatedBy = userId.Value;

                    if (entry.State == EntityState.Added)
                    {
                        entity.CreatedDate = DateTime.Now;
                        entity.CreatedBy = userId.Value;
                    }
                    else
                    {
                        // Giữ nguyên CreatedDate và CreatedBy
                        entry.Property(nameof(BaseEntity.CreatedDate)).IsModified = false;
                        entry.Property(nameof(BaseEntity.CreatedBy)).IsModified = false;
                    }
                }
            }
        }

        #region Systems
        public DbSet<Log> Logs { get; set; }
        public DbSet<SystemInfo> SystemInfo { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<GroupPermision> GroupsPermision { get; set; }
        public DbSet<Permission> Permission { get; set; }
        public DbSet<RoleAction> RoleActions { get; set; }
        public DbSet<QuestionAnswer> QuestionAnswers { get; set; }
        #endregion

        #region Settings
        public DbSet<DanhMucDonVi> DanhMucDonVis { get; set; }
        public DbSet<DanhMucDiaDanh> DanhMucDiaDanhs { get; set; }
        public DbSet<DanhMucTrangThai> DanhMucTrangThais { get; set; }
        public DbSet<DanhMucVanBan> DanhMucVanBans { get; set; }
        public DbSet<OptionData> OptionDatas { get; set; }
       
        public DbSet<DanhMucPhongBan> DanhMucPhongBans { get; set; }
        public DbSet<DanhMucCanBo> DanhMucCanBos { get; set; }
        #endregion

        #region Manages
        public DbSet<AttachedFile> AttachedFiles { get; set; }
        public DbSet<Notification> Notifications { get; set; }       
        public DbSet<ThuTucHanhChinh> ThuTucHanhChinhs { get; set; }       
        #endregion
    }
}
