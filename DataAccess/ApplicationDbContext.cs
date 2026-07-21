using DataAccess.Entities;
using DataAccess.Entities.Settings;
using DataAccess.Entities.Settings.DanhMucGia;
using DataAccess.Entities.DinhGiaHHDV;
using DataAccess.Entities.DinhGiaHHDV.ChiTiet;
using DataAccess.Entities.KeKhaiDangKyGia;
using DataAccess.Entities.Manages;
using DataAccess.Entities.Manages.ThongTinHoSo;
using DataAccess.Entities.Systems;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using DataAccess.Entities.ThamDinhGia;

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

            // Cấu hình MoMoPayment với ConcurrencyToken
            modelBuilder.Entity<MoMoPayment>()
                .Property(p => p.RowVersion)
                .IsRowVersion();

            // NEW: Unique constraint for EAV values per (HoSoId, DanhMucHopDongChiTietId)
            modelBuilder.Entity<HoSoCCCTChiTiet>()
                .HasIndex(e => new { e.HoSoId, e.DanhMucHopDongChiTietId })
                .IsUnique();

            // NEW: Configure delete behaviors to avoid multiple cascade paths in SQL Server
            modelBuilder.Entity<HoSoCCCTChiTiet>()
                .HasOne(e => e.HoSo)
                .WithMany(h => h.HoSoCCCTChiTiets)
                .HasForeignKey(e => e.HoSoId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HoSoCCCTChiTiet>()
                .HasOne(e => e.Field)
                .WithMany()
                .HasForeignKey(e => e.DanhMucHopDongChiTietId)
                .OnDelete(DeleteBehavior.Cascade);
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
        public DbSet<OptionData> OptionDatas { get; set; }
        public DbSet<DanhMucHopDong> DanhMucHopDongs { get; set; }
        public DbSet<DanhMucHopDongChiTiet> DanhMucHopDongChiTiets { get; set; }
        public DbSet<DanhMucPhongBan> DanhMucPhongBans { get; set; }
        public DbSet<DanhMucCanBo> DanhMucCanBos { get; set; }
        public DbSet<DanhMucPhiLePhi> DanhMucPhiLePhis { get; set; }
        public DbSet<DanhMucDonViTinh> DanhMucDonViTinhs { get; set; }
        public DbSet<DanhMucKinhDoanh> DanhMucKinhDoanhs { get; set; }
        public DbSet<DanhMucGiaChung> DanhMucGiaChungs { get; set; }
        public DbSet<DanhMucGiaChungCt> DanhMucGiaChungCts { get; set; }
        public DbSet<DanhMucNuocSach> DanhMucNuocSachs { get; set; }
        public DbSet<DanhMucNuocSachCt> DanhMucNuocSachCts { get; set; }
        public DbSet<DanhMucGiaThueTaiNguyen> DanhMucGiaThueTaiNguyens { get; set; }
        public DbSet<DanhMucGiaThueTaiNguyenCt> DanhMucGiaThueTaiNguyenCts { get; set; }
        #endregion

        #region Manages
        public DbSet<AttachedFile> AttachedFiles { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<ThongTinNganChan> ThongTinNganChans { get; set; }
        public DbSet<HoSoCCCT> HoSoCCCTs { get; set; }
        public DbSet<HoSoCCCTChiPhi> HoSoCCCTChiPhis { get; set; }
        public DbSet<HoSoCCCTHistory> HoSoCCCTHistories { get; set; }
        public DbSet<HoSoCCCTChiTiet> HoSoCCCTChiTiets { get; set; }
        public DbSet<ThuTucHanhChinh> ThuTucHanhChinhs { get; set; }
        public DbSet<MoMoPayment> MoMoPayments { get; set; }
        #endregion

        #region DinhGiaHHDV
        public DbSet<GiaThiTruong> GiaThiTruongs { get; set; }
        public DbSet<GiaThiTruongCt> GiaThiTruongCts { get; set; }
        public DbSet<GiaThiTruongDanhMuc> GiaThiTruongDanhMucs { get; set; }
        public DbSet<GiaThiTruongDanhMucCt> GiaThiTruongDanhMucCts { get; set; }
        public DbSet<GiaThiTruongTongHop> GiaThiTruongTongHops { get; set; }
        public DbSet<GiaThiTruongTongHopCt> GiaThiTruongTongHopCts { get; set; }
        public DbSet<DinhGia> DinhGias { get; set; }
        public DbSet<ChiTietGiaChung> ChiTietGiaChungs { get; set; }
        public DbSet<ChiTietNuocSach> ChiTietNuocSachs { get; set; }
        public DbSet<ChiTietGiaThueTaiNguyen> ChiTietGiaThueTaiNguyens { get; set; }

        #endregion

        #region KeKhaiDangKyGia
        public DbSet<DoanhNghiep> DoanhNghieps { get; set; }
        public DbSet<DoanhNghiepLvKd> DoanhNghiepLvKds { get; set; }
        public DbSet<KeKhaiDangKyGiaDMDT> KeKhaiDangKyGiaDMDTs { get; set; }
        public DbSet<KeKhaiDangKyGiaDMKH> KeKhaiDangKyGiaDMKHs { get; set; }
        public DbSet<KeKhaiDangKyGiaDMHH> KeKhaiDangKyGiaDMHHs { get; set; }
        public DbSet<KeKhaiDangKyGia> KeKhaiDangKyGias { get; set; }
        public DbSet<KeKhaiDangKyGiaCt> KeKhaiDangKyGiaCts { get; set; }

        #endregion

        #region ThamDinhGia
        public DbSet<ThamDinhGiaDanhMucDonVi> ThamDinhGiaDanhMucDonVis { get; set; }
        public DbSet<ThamDinhGiaDanhMucHangHoa> ThamDinhGiaDanhMucHangHoas { get; set; }
        public DbSet<ThamDinhGiaDanhMucHangHoaCt> ThamDinhGiaDanhMucHangHoaCts { get; set; }
        public DbSet<ThamDinhGiaHoiDong> ThamDinhGiaHoiDongs { get; set; }
        public DbSet<ThamDinhGiaHoiDongCt> ThamDinhGiaHoiDongCts { get; set; }
        public DbSet<ThamDinhGia> ThamDinhGias { get; set; }
        public DbSet<ThamDinhGiaCt> ThamDinhGiaCts { get; set; }

        #endregion
    }
}