using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities.Manages
{
    public class HoSoVanBanGiaHan : BaseEntity
    {
        public Guid HoSoVanBanId { get; set; }

        public Guid? BuocQuyTrinhId { get; set; }

        public Guid? NguoiGiaHanId { get; set; }

        public DateTime HanXuLyCu { get; set; }

        public DateTime HanXuLyMoi { get; set; }

        public int SoNgayGiaHan { get; set; }

        [StringLength(1000)]
        public string? LyDoGiaHan { get; set; }

        public Guid? AttachedFileGroupId { get; set; }

        public string? GhiChu { get; set; }
    }
}
