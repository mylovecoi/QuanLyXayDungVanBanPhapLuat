using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities.Manages
{
    public class HoSoVanBanDuThaoVersion : BaseEntity
    {
        public Guid HoSoVanBanId { get; set; }

        public int LanVersion { get; set; } = 1;

        public int SoLanTraLai { get; set; } = 0;

        [StringLength(250)]
        public string TenVersion { get; set; } = string.Empty;

        public Guid AttachedFileGroupId { get; set; }

        public Guid? DonViTaoId { get; set; }

        public Guid? NguoiTaoId { get; set; }

        public DateTime NgayTaoVersion { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string LoaiVersion { get; set; } = "GUI_THAM_DINH";

        public string? GhiChu { get; set; }
    }
}
