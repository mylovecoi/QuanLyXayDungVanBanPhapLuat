using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities.Manages
{
    public class HoSoVanBanDotLayYKien : BaseEntity
    {
        public Guid HoSoVanBanId { get; set; }

        public Guid BuocQuyTrinhId { get; set; }

        public int LanLayYKien { get; set; } = 1;

        [StringLength(20)]
        public string CoQuanLayYKien { get; set; } = string.Empty;

        [StringLength(20)]
        public string CheDoNhapYKien { get; set; } = "TONG_HOP";

        [StringLength(30)]
        public string? HinhThucLayYKien { get; set; }

        [StringLength(100)]
        public string? SoVanBanLayYKien { get; set; }

        public DateTime? NgayGuiLayYKien { get; set; }

        public DateTime? HanPhanHoi { get; set; }

        public DateTime? NgayCoKetQua { get; set; }

        public string? NoiDungYeuCau { get; set; }

        public int? TongSoThanhVien { get; set; }

        public int? SoDongY { get; set; }

        public int? SoDongYCoYKien { get; set; }

        public int? SoKhongDongY { get; set; }

        public int? SoKhongPhanHoi { get; set; }

        public decimal? TyLeDongY { get; set; }

        [StringLength(50)]
        public string? KetQuaChung { get; set; }

        public string? NoiDungTongHop { get; set; }

        public string? NoiDungTiepThu { get; set; }

        public Guid? NguoiTongHopId { get; set; }

        public DateTime? NgayTongHop { get; set; }

        [StringLength(30)]
        public string TrangThai { get; set; } = "NHAP";

        public Guid? AttachedFileGroupId { get; set; }

        public string? GhiChu { get; set; }
    }
}
