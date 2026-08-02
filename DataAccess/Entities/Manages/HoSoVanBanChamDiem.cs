using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities.Manages
{
    public class HoSoVanBanChamDiem : BaseEntity
    {
        public Guid HoSoVanBanId { get; set; }
        public Guid? NguoiChamDiemId { get; set; }
        public DateTime NgayChamDiem { get; set; } = DateTime.Now;
        public decimal TongDiem { get; set; } = 0;

        [StringLength(100)]
        public string? XepLoai { get; set; }

        [StringLength(30)]
        public string TrangThai { get; set; } = "NHAP";

        public string? GhiChu { get; set; }
    }
}
