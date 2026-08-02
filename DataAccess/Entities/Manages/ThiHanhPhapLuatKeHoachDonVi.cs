using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities.Manages
{
    public class ThiHanhPhapLuatKeHoachDonVi : BaseEntity
    {
        public Guid KeHoachId { get; set; }

        public Guid DonViId { get; set; }

        [StringLength(30)]
        public string VaiTro { get; set; } = "PHOI_HOP";

        public DateTime? NgayNhanKeHoach { get; set; }

        public bool DaXem { get; set; } = false;

        public DateTime? NgayXem { get; set; }

        public string? GhiChu { get; set; }
    }
}
