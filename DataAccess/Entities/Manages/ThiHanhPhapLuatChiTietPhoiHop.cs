using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities.Manages
{
    public class ThiHanhPhapLuatChiTietPhoiHop : BaseEntity
    {
        public Guid ChiTietNhiemVuId { get; set; }

        public Guid NguoiDungId { get; set; }

        [StringLength(20)]
        public string VaiTro { get; set; } = "PHOI_HOP";

        public string? GhiChu { get; set; }
    }
}
