using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities.Manages
{
    public class HoSoVanBanYKienThanhVien : BaseEntity
    {
        public Guid DotLayYKienId { get; set; }

        public Guid? ThanhVienId { get; set; }

        [StringLength(250)]
        public string HoTenThanhVien { get; set; } = string.Empty;

        [StringLength(250)]
        public string? ChucVu { get; set; }

        public Guid? DonViId { get; set; }

        [StringLength(250)]
        public string? TenDonVi { get; set; }

        public int ThuTuHienThi { get; set; }

        public bool CoQuyenBieuQuyet { get; set; } = true;

        public DateTime? NgayGui { get; set; }

        public DateTime? HanPhanHoi { get; set; }

        [StringLength(30)]
        public string? KetQuaYKien { get; set; }

        public string? NoiDungYKien { get; set; }

        public string? NoiDungTiepThu { get; set; }

        public DateTime? NgayPhanHoi { get; set; }

        [StringLength(30)]
        public string TrangThaiPhanHoi { get; set; } = "CHUA_PHAN_HOI";

        public Guid? AttachedFileGroupId { get; set; }

        public string? GhiChu { get; set; }
    }
}
