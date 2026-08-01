using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities.Manages
{
    public class HoSoVanBanDuThao : BaseEntity
    {
        public Guid HoSoVanBanId { get; set; }

        [StringLength(500)]
        public string TenDuThao { get; set; } = string.Empty;

        public int SoLanDuThao { get; set; } = 1;

        public DateTime? NgayCapNhatDuThao { get; set; }

        [StringLength(50)]
        public string TrangThaiDuThao { get; set; } = "CHUA_CAP_NHAT";

        public string? NoiDungTomTat { get; set; }

        [StringLength(50)]
        public string KetQuaThucHien { get; set; } = "CHUA_HOAN_THANH";

        public DateTime? NgayBaoCaoKetQua { get; set; }

        public string? NoiDungBaoCao { get; set; }

        public bool DaDuDieuKienChuyenBuoc { get; set; } = false;

        public string? GhiChu { get; set; }
    }
}
