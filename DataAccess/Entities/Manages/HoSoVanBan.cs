using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities.Manages
{
    public class HoSoVanBan : BaseEntity
    {
        [Required(ErrorMessage = "Ma ho so khong duoc de trong")]
        public string MaHoSo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ten ho so khong duoc de trong")]
        public string TenHoSo { get; set; } = string.Empty;

        public Guid DanhMucVanBanId { get; set; }

        public Guid QuyTrinhSoanThaoId { get; set; }

        public Guid? BuocHienTaiId { get; set; }

        public Guid? DanhMucTrangThaiId { get; set; }

        public Guid DonViSoanThaoId { get; set; }

        public Guid NguoiTaoId { get; set; }

        public DateTime NgayTaoHoSo { get; set; } = DateTime.Now;

        public DateTime? HanXuLy { get; set; }

        public DateTime? NgayHoanThanh { get; set; }

        public int SoLanTraLaiHienTai { get; set; } = 0;

        public int? TongThoiGianXayDungNgay { get; set; }

        public int? TongThoiGianQuyDinhNgay { get; set; }

        public decimal? TyLeThoiGianXayDung { get; set; }

        public decimal? DiemTienDoXayDung { get; set; }

        public decimal? DiemChatLuongVanBan { get; set; }

        public decimal? TongDiemDanhGia { get; set; }

        [StringLength(100)]
        public string? XepLoaiDanhGia { get; set; }

        public DateTime? NgayChamDiem { get; set; }

        public Guid? AttachedFileGroupId { get; set; }

        [StringLength(30)]
        public string? LoaiVanBanBanHanh { get; set; }

        [StringLength(100)]
        public string? SoKyHieuBanHanh { get; set; }

        [StringLength(1000)]
        public string? TrichYeuBanHanh { get; set; }

        public Guid? CoQuanBanHanhId { get; set; }

        public Guid? NguoiKyId { get; set; }

        [StringLength(250)]
        public string? HoTenNguoiKy { get; set; }

        [StringLength(250)]
        public string? ChucVuNguoiKy { get; set; }

        public DateTime? NgayKy { get; set; }

        public DateTime? NgayBanHanh { get; set; }

        public DateTime? NgayCoHieuLuc { get; set; }

        public DateTime? NgayHetHieuLuc { get; set; }

        [StringLength(30)]
        public string TrangThaiBanHanh { get; set; } = "CHUA_BAN_HANH";

        public Guid? VanBanPhapLuatId { get; set; }

        public DateTime? NgayCongKhai { get; set; }

        [StringLength(500)]
        public string? DuongDanCongKhai { get; set; }

        public string? MoTa { get; set; }

        public string? GhiChu { get; set; }
    }
}
