namespace DataAccess.Entities.Manages
{
    public class HoSoVanBanXuLy : BaseEntity
    {
        public Guid HoSoVanBanId { get; set; }

        public Guid BuocQuyTrinhId { get; set; }

        public int LanXuLy { get; set; } = 1;

        public Guid DonViXuLyId { get; set; }

        public Guid? NguoiXuLyId { get; set; }

        public DateTime NgayNhan { get; set; } = DateTime.Now;

        public DateTime? HanXuLy { get; set; }

        public DateTime? NgayXuLy { get; set; }

        public string? KetQuaXuLy { get; set; }

        public string? NoiDungXuLy { get; set; }

        public Guid? DanhMucTrangThaiId { get; set; }

        public bool IsCurrent { get; set; } = true;

        public string? GhiChu { get; set; }
    }
}
