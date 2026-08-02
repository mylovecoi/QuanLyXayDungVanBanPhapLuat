using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities.Manages
{
    public class ThiHanhPhapLuatChiTietNhiemVu : BaseEntity
    {
        public Guid NhiemVuId { get; set; }

        [StringLength(50)]
        public string MaChiTiet { get; set; } = string.Empty;

        [StringLength(500)]
        public string TenChiTiet { get; set; } = string.Empty;

        public string? NoiDungChiTiet { get; set; }

        [StringLength(30)]
        public string LoaiChiTiet { get; set; } = "NHIEM_VU_CON";

        public Guid DonViThucHienId { get; set; }

        public Guid? NguoiPhuTrachChinhId { get; set; }

        public DateTime? NgayBatDau { get; set; }

        public DateTime? HanHoanThanh { get; set; }

        [StringLength(30)]
        public string TrangThai { get; set; } = "CHUA_THUC_HIEN";

        public int TyLeHoanThanh { get; set; } = 0;

        public string? KetQuaYeuCau { get; set; }

        public decimal? GiaTriChiTieu { get; set; }

        [StringLength(100)]
        public string? DonViTinh { get; set; }

        public int ThuTuSapXep { get; set; } = 0;

        public string? GhiChu { get; set; }
    }
}
