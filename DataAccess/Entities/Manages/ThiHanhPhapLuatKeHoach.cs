using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities.Manages
{
    public class ThiHanhPhapLuatKeHoach : BaseEntity
    {
        [StringLength(50)]
        public string MaKeHoach { get; set; } = string.Empty;

        [StringLength(500)]
        public string TenKeHoach { get; set; } = string.Empty;

        public int Nam { get; set; }

        public Guid? DanhMucVanBanId { get; set; }

        [StringLength(255)]
        public string? SoKyHieuVanBanCanCu { get; set; }

        public DateTime? NgayBanHanhVanBanCanCu { get; set; }

        [StringLength(1000)]
        public string? TrichYeuVanBanCanCu { get; set; }

        [StringLength(500)]
        public string? CoQuanBanHanhVanBanCanCu { get; set; }

        public Guid DonViChuTriId { get; set; }

        public DateTime? NgayBatDau { get; set; }

        public DateTime? NgayKetThuc { get; set; }

        public DateTime? NgayCongBo { get; set; }

        [StringLength(30)]
        public string TrangThai { get; set; } = "NHAP";

        public string? MoTa { get; set; }

        public string? GhiChu { get; set; }

        public Guid? AttachedFileGroupId { get; set; }
    }
}
