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

            modelBuilder.Entity<DanhMucBuocQuyTrinh>()
                .HasOne<DanhMucQuyTrinhSoanThao>()
                .WithMany()
                .HasForeignKey(x => x.QuyTrinhSoanThaoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DanhMucBuocQuyTrinh>()
                .HasOne<DanhMucDonVi>()
                .WithMany()
                .HasForeignKey(x => x.DonViTiepNhanMacDinhId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<DanhMucChuyenBuocQuyTrinh>()
                .HasOne<DanhMucQuyTrinhSoanThao>()
                .WithMany()
                .HasForeignKey(x => x.QuyTrinhSoanThaoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DanhMucChuyenBuocQuyTrinh>()
                .HasOne<DanhMucBuocQuyTrinh>()
                .WithMany()
                .HasForeignKey(x => x.TuBuocId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<DanhMucChuyenBuocQuyTrinh>()
                .HasOne<DanhMucBuocQuyTrinh>()
                .WithMany()
                .HasForeignKey(x => x.DenBuocId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<DanhMucQuyTrinhSoanThao>()
                .HasOne<DanhMucVanBan>()
                .WithMany()
                .HasForeignKey(x => x.DanhMucVanBanId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HoSoVanBan>()
                .HasOne<DanhMucVanBan>()
                .WithMany()
                .HasForeignKey(x => x.DanhMucVanBanId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HoSoVanBan>()
                .HasOne<DanhMucQuyTrinhSoanThao>()
                .WithMany()
                .HasForeignKey(x => x.QuyTrinhSoanThaoId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HoSoVanBan>()
                .HasOne<DanhMucBuocQuyTrinh>()
                .WithMany()
                .HasForeignKey(x => x.BuocHienTaiId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HoSoVanBan>()
                .HasOne<DanhMucTrangThai>()
                .WithMany()
                .HasForeignKey(x => x.DanhMucTrangThaiId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HoSoVanBan>()
                .HasOne<DanhMucDonVi>()
                .WithMany()
                .HasForeignKey(x => x.DonViSoanThaoId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HoSoVanBan>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.NguoiTaoId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HoSoVanBanXuLy>()
                .HasOne<HoSoVanBan>()
                .WithMany()
                .HasForeignKey(x => x.HoSoVanBanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HoSoVanBanXuLy>()
                .HasOne<DanhMucBuocQuyTrinh>()
                .WithMany()
                .HasForeignKey(x => x.BuocQuyTrinhId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HoSoVanBanXuLy>()
                .HasOne<DanhMucDonVi>()
                .WithMany()
                .HasForeignKey(x => x.DonViXuLyId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HoSoVanBanXuLy>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.NguoiXuLyId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HoSoVanBanXuLy>()
                .HasOne<DanhMucTrangThai>()
                .WithMany()
                .HasForeignKey(x => x.DanhMucTrangThaiId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HoSoVanBanBuocThoiHan>()
                .HasOne<HoSoVanBan>()
                .WithMany()
                .HasForeignKey(x => x.HoSoVanBanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HoSoVanBanBuocThoiHan>()
                .HasOne<DanhMucBuocQuyTrinh>()
                .WithMany()
                .HasForeignKey(x => x.BuocQuyTrinhId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HoSoVanBanLayYKien>()
                .HasOne<HoSoVanBan>()
                .WithMany()
                .HasForeignKey(x => x.HoSoVanBanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HoSoVanBanLayYKien>()
                .HasOne<DanhMucBuocQuyTrinh>()
                .WithMany()
                .HasForeignKey(x => x.BuocQuyTrinhId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HoSoVanBanLayYKien>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.NguoiDuocLayYKienId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HoSoVanBanLayYKien>()
                .HasOne<DanhMucDonVi>()
                .WithMany()
                .HasForeignKey(x => x.DonViDuocLayYKienId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HoSoVanBanDanhGia>()
                .HasOne<HoSoVanBan>()
                .WithMany()
                .HasForeignKey(x => x.HoSoVanBanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HoSoVanBanDanhGia>()
                .HasOne<DanhMucBuocQuyTrinh>()
                .WithMany()
                .HasForeignKey(x => x.BuocQuyTrinhId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HoSoVanBanDanhGia>()
                .HasOne<DanhMucDonVi>()
                .WithMany()
                .HasForeignKey(x => x.DonViDanhGiaId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HoSoVanBanDanhGia>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.NguoiDanhGiaId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HoSoVanBanDanhGia>()
                .HasOne<DanhMucBuocQuyTrinh>()
                .WithMany()
                .HasForeignKey(x => x.TraLaiBuocId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HoSoVanBanPhanHoiDanhGia>()
                .HasOne<HoSoVanBanDanhGia>()
                .WithMany()
                .HasForeignKey(x => x.HoSoVanBanDanhGiaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HoSoVanBanPhanHoiDanhGia>()
                .HasOne<HoSoVanBan>()
                .WithMany()
                .HasForeignKey(x => x.HoSoVanBanId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HoSoVanBanPhanHoiDanhGia>()
                .HasOne<DanhMucDonVi>()
                .WithMany()
                .HasForeignKey(x => x.DonViSoanThaoId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HoSoVanBanPhanHoiDanhGia>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.NguoiPhanHoiId)
                .OnDelete(DeleteBehavior.NoAction);
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
        public DbSet<DanhMucQuyTrinhSoanThao> DanhMucQuyTrinhSoanThaos { get; set; }
        public DbSet<DanhMucBuocQuyTrinh> DanhMucBuocQuyTrinhs { get; set; }
        public DbSet<DanhMucChuyenBuocQuyTrinh> DanhMucChuyenBuocQuyTrinhs { get; set; }
        public DbSet<DanhMucTrangThai> DanhMucTrangThais { get; set; }
        public DbSet<DanhMucVanBan> DanhMucVanBans { get; set; }
        public DbSet<OptionData> OptionDatas { get; set; }
       
        public DbSet<DanhMucPhongBan> DanhMucPhongBans { get; set; }
        public DbSet<DanhMucCanBo> DanhMucCanBos { get; set; }
        #endregion

        #region Manages
        public DbSet<AttachedFile> AttachedFiles { get; set; }
        public DbSet<HoSoVanBan> HoSoVanBans { get; set; }
        public DbSet<HoSoVanBanBuocThoiHan> HoSoVanBanBuocThoiHans { get; set; }
        public DbSet<HoSoVanBanXuLy> HoSoVanBanXuLys { get; set; }
        public DbSet<HoSoVanBanLayYKien> HoSoVanBanLayYKiens { get; set; }
        public DbSet<HoSoVanBanDanhGia> HoSoVanBanDanhGias { get; set; }
        public DbSet<HoSoVanBanPhanHoiDanhGia> HoSoVanBanPhanHoiDanhGias { get; set; }
        public DbSet<Notification> Notifications { get; set; }       
        public DbSet<ThuTucHanhChinh> ThuTucHanhChinhs { get; set; }       
        #endregion
    }
}
