using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities.Manages
{
    public class HoSoVanBanChamDiemChiTiet : BaseEntity
    {
        public Guid HoSoVanBanChamDiemId { get; set; }
        public Guid DanhMucTieuChiDiemId { get; set; }

        [StringLength(100)]
        public string MaTieuChi { get; set; } = string.Empty;

        [StringLength(250)]
        public string TenTieuChi { get; set; } = string.Empty;

        [StringLength(50)]
        public string LoaiTieuChi { get; set; } = string.Empty;

        public decimal? GiaTriTinhDiem { get; set; }
        public decimal? DiemDeXuat { get; set; }
        public decimal DiemChinhThuc { get; set; } = 0;
        public decimal DiemToiDa { get; set; } = 0;
        public string? DienGiaiGiaTri { get; set; }
        public string? GhiChu { get; set; }
    }
}
