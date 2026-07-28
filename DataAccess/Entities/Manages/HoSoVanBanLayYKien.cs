namespace DataAccess.Entities.Manages
{
    public class HoSoVanBanLayYKien : BaseEntity
    {
        public Guid HoSoVanBanId { get; set; }

        public Guid BuocQuyTrinhId { get; set; }

        public Guid? NguoiDuocLayYKienId { get; set; }

        public Guid? DonViDuocLayYKienId { get; set; }

        public string? NoiDungYeuCau { get; set; }

        public string? NoiDungPhanHoi { get; set; }

        public DateTime NgayGui { get; set; } = DateTime.Now;

        public DateTime? HanPhanHoi { get; set; }

        public DateTime? NgayPhanHoi { get; set; }

        public string? TrangThaiPhanHoi { get; set; }

        public Guid? AttachedFileGroupId { get; set; }

        public string? GhiChu { get; set; }
    }
}
