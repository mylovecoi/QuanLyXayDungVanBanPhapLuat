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

            modelBuilder.Entity<HoSoVanBan>()
                .HasOne<DanhMucDonVi>()
                .WithMany()
                .HasForeignKey(x => x.CoQuanBanHanhId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HoSoVanBan>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.NguoiKyId)
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

            modelBuilder.Entity<HoSoVanBanGiaHan>()
                .HasOne<HoSoVanBan>()
                .WithMany()
                .HasForeignKey(x => x.HoSoVanBanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HoSoVanBanGiaHan>()
                .HasOne<DanhMucBuocQuyTrinh>()
                .WithMany()
                .HasForeignKey(x => x.BuocQuyTrinhId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HoSoVanBanGiaHan>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.NguoiGiaHanId)
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

            modelBuilder.Entity<HoSoVanBanDotLayYKien>()
                .HasOne<HoSoVanBan>()
                .WithMany()
                .HasForeignKey(x => x.HoSoVanBanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HoSoVanBanDotLayYKien>()
                .HasOne<DanhMucBuocQuyTrinh>()
                .WithMany()
                .HasForeignKey(x => x.BuocQuyTrinhId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HoSoVanBanDotLayYKien>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.NguoiTongHopId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HoSoVanBanDotLayYKien>()
                .HasIndex(x => new { x.HoSoVanBanId, x.BuocQuyTrinhId, x.CoQuanLayYKien, x.LanLayYKien })
                .IsUnique();

            modelBuilder.Entity<HoSoVanBanDotLayYKien>()
                .Property(x => x.TyLeDongY)
                .HasPrecision(5, 2);

            modelBuilder.Entity<HoSoVanBan>()
                .Property(x => x.TyLeThoiGianXayDung)
                .HasPrecision(10, 2);

            modelBuilder.Entity<HoSoVanBan>()
                .Property(x => x.DiemTienDoXayDung)
                .HasPrecision(10, 2);

            modelBuilder.Entity<HoSoVanBan>()
                .Property(x => x.DiemChatLuongVanBan)
                .HasPrecision(10, 2);

            modelBuilder.Entity<HoSoVanBan>()
                .Property(x => x.TongDiemDanhGia)
                .HasPrecision(10, 2);

            modelBuilder.Entity<DanhMucTieuChiDiem>()
                .HasIndex(x => x.MaTieuChi)
                .IsUnique();

            modelBuilder.Entity<DanhMucTieuChiDiem>()
                .Property(x => x.DiemToiDa)
                .HasPrecision(10, 2);

            modelBuilder.Entity<DanhMucTieuChiDiemMuc>()
                .HasOne<DanhMucTieuChiDiem>()
                .WithMany()
                .HasForeignKey(x => x.DanhMucTieuChiDiemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DanhMucTieuChiDiemMuc>()
                .Property(x => x.TuGiaTri)
                .HasPrecision(18, 2);

            modelBuilder.Entity<DanhMucTieuChiDiemMuc>()
                .Property(x => x.DenGiaTri)
                .HasPrecision(18, 2);

            modelBuilder.Entity<DanhMucTieuChiDiemMuc>()
                .Property(x => x.Diem)
                .HasPrecision(10, 2);

            modelBuilder.Entity<HoSoVanBanYKienThanhVien>()
                .HasOne<HoSoVanBanDotLayYKien>()
                .WithMany()
                .HasForeignKey(x => x.DotLayYKienId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HoSoVanBanYKienThanhVien>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.ThanhVienId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HoSoVanBanYKienThanhVien>()
                .HasOne<DanhMucDonVi>()
                .WithMany()
                .HasForeignKey(x => x.DonViId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HoSoVanBanYKienThanhVien>()
                .HasIndex(x => new { x.DotLayYKienId, x.ThanhVienId })
                .IsUnique()
                .HasFilter("[ThanhVienId] IS NOT NULL");

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

            modelBuilder.Entity<HoSoVanBanChamDiem>()
                .HasOne<HoSoVanBan>()
                .WithMany()
                .HasForeignKey(x => x.HoSoVanBanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HoSoVanBanChamDiem>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.NguoiChamDiemId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HoSoVanBanChamDiem>()
                .HasIndex(x => x.HoSoVanBanId)
                .IsUnique();

            modelBuilder.Entity<HoSoVanBanChamDiem>()
                .Property(x => x.TongDiem)
                .HasPrecision(10, 2);

            modelBuilder.Entity<HoSoVanBanChamDiemChiTiet>()
                .HasOne<HoSoVanBanChamDiem>()
                .WithMany()
                .HasForeignKey(x => x.HoSoVanBanChamDiemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HoSoVanBanChamDiemChiTiet>()
                .HasOne<DanhMucTieuChiDiem>()
                .WithMany()
                .HasForeignKey(x => x.DanhMucTieuChiDiemId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HoSoVanBanChamDiemChiTiet>()
                .Property(x => x.GiaTriTinhDiem)
                .HasPrecision(18, 2);

            modelBuilder.Entity<HoSoVanBanChamDiemChiTiet>()
                .Property(x => x.DiemDeXuat)
                .HasPrecision(10, 2);

            modelBuilder.Entity<HoSoVanBanChamDiemChiTiet>()
                .Property(x => x.DiemChinhThuc)
                .HasPrecision(10, 2);

            modelBuilder.Entity<HoSoVanBanChamDiemChiTiet>()
                .Property(x => x.DiemToiDa)
                .HasPrecision(10, 2);

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

            modelBuilder.Entity<HoSoVanBanDuThao>()
                .HasOne<HoSoVanBan>()
                .WithMany()
                .HasForeignKey(x => x.HoSoVanBanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HoSoVanBanDuThao>()
                .HasIndex(x => x.HoSoVanBanId)
                .IsUnique();

            modelBuilder.Entity<HoSoVanBanDuThaoVersion>()
                .HasOne<HoSoVanBan>()
                .WithMany()
                .HasForeignKey(x => x.HoSoVanBanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ThiHanhPhapLuatKeHoach>()
                .HasOne<DanhMucVanBan>()
                .WithMany()
                .HasForeignKey(x => x.DanhMucVanBanId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ThiHanhPhapLuatKeHoach>()
                .HasOne<DanhMucDonVi>()
                .WithMany()
                .HasForeignKey(x => x.DonViChuTriId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ThiHanhPhapLuatKeHoach>()
                .HasIndex(x => x.MaKeHoach)
                .IsUnique();

            modelBuilder.Entity<ThiHanhPhapLuatKeHoachDonVi>()
                .HasOne<ThiHanhPhapLuatKeHoach>()
                .WithMany()
                .HasForeignKey(x => x.KeHoachId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ThiHanhPhapLuatKeHoachDonVi>()
                .HasOne<DanhMucDonVi>()
                .WithMany()
                .HasForeignKey(x => x.DonViId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ThiHanhPhapLuatKeHoachDonVi>()
                .HasIndex(x => new { x.KeHoachId, x.DonViId, x.VaiTro })
                .IsUnique();

            modelBuilder.Entity<ThiHanhPhapLuatNhiemVu>()
                .HasOne<ThiHanhPhapLuatKeHoach>()
                .WithMany()
                .HasForeignKey(x => x.KeHoachId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ThiHanhPhapLuatNhiemVu>()
                .HasOne<DanhMucDonVi>()
                .WithMany()
                .HasForeignKey(x => x.DonViChuTriId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ThiHanhPhapLuatNhiemVu>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.NguoiDieuPhoiId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ThiHanhPhapLuatNhiemVu>()
                .HasIndex(x => new { x.KeHoachId, x.MaNhiemVu })
                .IsUnique();

            modelBuilder.Entity<ThiHanhPhapLuatChiTietNhiemVu>()
                .HasOne<ThiHanhPhapLuatNhiemVu>()
                .WithMany()
                .HasForeignKey(x => x.NhiemVuId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ThiHanhPhapLuatChiTietNhiemVu>()
                .HasOne<DanhMucDonVi>()
                .WithMany()
                .HasForeignKey(x => x.DonViThucHienId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ThiHanhPhapLuatChiTietNhiemVu>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.NguoiPhuTrachChinhId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ThiHanhPhapLuatChiTietNhiemVu>()
                .HasIndex(x => new { x.NhiemVuId, x.MaChiTiet })
                .IsUnique();

            modelBuilder.Entity<ThiHanhPhapLuatChiTietNhiemVu>()
                .Property(x => x.GiaTriChiTieu)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ThiHanhPhapLuatChiTietPhoiHop>()
                .HasOne<ThiHanhPhapLuatChiTietNhiemVu>()
                .WithMany()
                .HasForeignKey(x => x.ChiTietNhiemVuId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ThiHanhPhapLuatChiTietPhoiHop>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.NguoiDungId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ThiHanhPhapLuatChiTietPhoiHop>()
                .HasIndex(x => new { x.ChiTietNhiemVuId, x.NguoiDungId })
                .IsUnique();

            modelBuilder.Entity<ThiHanhPhapLuatTienDo>()
                .HasOne<ThiHanhPhapLuatChiTietNhiemVu>()
                .WithMany()
                .HasForeignKey(x => x.ChiTietNhiemVuId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ThiHanhPhapLuatTienDo>()
                .HasOne<DanhMucDonVi>()
                .WithMany()
                .HasForeignKey(x => x.DonViCapNhatId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ThiHanhPhapLuatTienDo>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.NguoiCapNhatId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ThiHanhPhapLuatDanhGia>()
                .HasOne<ThiHanhPhapLuatKeHoach>()
                .WithMany()
                .HasForeignKey(x => x.KeHoachId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ThiHanhPhapLuatDanhGia>()
                .HasOne<ThiHanhPhapLuatNhiemVu>()
                .WithMany()
                .HasForeignKey(x => x.NhiemVuId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ThiHanhPhapLuatDanhGia>()
                .HasOne<ThiHanhPhapLuatChiTietNhiemVu>()
                .WithMany()
                .HasForeignKey(x => x.ChiTietNhiemVuId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ThiHanhPhapLuatDanhGia>()
                .HasOne<DanhMucDonVi>()
                .WithMany()
                .HasForeignKey(x => x.DonViDuocDanhGiaId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ThiHanhPhapLuatDanhGia>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.NguoiDanhGiaId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ThiHanhPhapLuatTongHop>()
                .HasOne<ThiHanhPhapLuatKeHoach>()
                .WithMany()
                .HasForeignKey(x => x.KeHoachId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ThiHanhPhapLuatTongHop>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.NguoiTongHopId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ThiHanhPhapLuatTongHop>()
                .Property(x => x.TyLeHoanThanh)
                .HasPrecision(10, 2);
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
        public DbSet<DanhMucTieuChiDiem> DanhMucTieuChiDiems { get; set; }
        public DbSet<DanhMucTieuChiDiemMuc> DanhMucTieuChiDiemMucs { get; set; }
        public DbSet<DanhMucVanBan> DanhMucVanBans { get; set; }
        public DbSet<OptionData> OptionDatas { get; set; }
       
        public DbSet<DanhMucPhongBan> DanhMucPhongBans { get; set; }
        public DbSet<DanhMucCanBo> DanhMucCanBos { get; set; }
        #endregion

        #region Manages
        public DbSet<AttachedFile> AttachedFiles { get; set; }
        public DbSet<HoSoVanBan> HoSoVanBans { get; set; }
        public DbSet<HoSoVanBanBuocThoiHan> HoSoVanBanBuocThoiHans { get; set; }
        public DbSet<HoSoVanBanGiaHan> HoSoVanBanGiaHans { get; set; }
        public DbSet<HoSoVanBanXuLy> HoSoVanBanXuLys { get; set; }
        public DbSet<HoSoVanBanLayYKien> HoSoVanBanLayYKiens { get; set; }
        public DbSet<HoSoVanBanDotLayYKien> HoSoVanBanDotLayYKiens { get; set; }
        public DbSet<HoSoVanBanYKienThanhVien> HoSoVanBanYKienThanhViens { get; set; }
        public DbSet<HoSoVanBanDanhGia> HoSoVanBanDanhGias { get; set; }
        public DbSet<HoSoVanBanChamDiem> HoSoVanBanChamDiems { get; set; }
        public DbSet<HoSoVanBanChamDiemChiTiet> HoSoVanBanChamDiemChiTiets { get; set; }
        public DbSet<HoSoVanBanPhanHoiDanhGia> HoSoVanBanPhanHoiDanhGias { get; set; }
        public DbSet<HoSoVanBanDuThao> HoSoVanBanDuThaos { get; set; }
        public DbSet<HoSoVanBanDuThaoVersion> HoSoVanBanDuThaoVersions { get; set; }
        public DbSet<Notification> Notifications { get; set; }       
        public DbSet<ThuTucHanhChinh> ThuTucHanhChinhs { get; set; }       
        public DbSet<ThiHanhPhapLuatKeHoach> ThiHanhPhapLuatKeHoachs { get; set; }
        public DbSet<ThiHanhPhapLuatKeHoachDonVi> ThiHanhPhapLuatKeHoachDonVis { get; set; }
        public DbSet<ThiHanhPhapLuatNhiemVu> ThiHanhPhapLuatNhiemVus { get; set; }
        public DbSet<ThiHanhPhapLuatChiTietNhiemVu> ThiHanhPhapLuatChiTietNhiemVus { get; set; }
        public DbSet<ThiHanhPhapLuatChiTietPhoiHop> ThiHanhPhapLuatChiTietPhoiHops { get; set; }
        public DbSet<ThiHanhPhapLuatTienDo> ThiHanhPhapLuatTienDos { get; set; }
        public DbSet<ThiHanhPhapLuatDanhGia> ThiHanhPhapLuatDanhGias { get; set; }
        public DbSet<ThiHanhPhapLuatTongHop> ThiHanhPhapLuatTongHops { get; set; }
        #endregion
    }
}
