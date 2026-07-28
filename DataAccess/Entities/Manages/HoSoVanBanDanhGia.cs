namespace DataAccess.Entities.Manages
{
    public class HoSoVanBanDanhGia : BaseEntity
    {
        public Guid HoSoVanBanId { get; set; }

        public Guid BuocQuyTrinhId { get; set; }

        public int LanDanhGia { get; set; } = 1;

        public Guid DonViDanhGiaId { get; set; }

        public Guid? NguoiDanhGiaId { get; set; }

        public DateTime NgayDanhGia { get; set; } = DateTime.Now;

        public string? KetQuaDanhGia { get; set; }

        public string? NoiDungDanhGia { get; set; }

        public string? YeuCauChinhSua { get; set; }

        public Guid? AttachedFileGroupId { get; set; }

        public Guid? TraLaiBuocId { get; set; }

        public string? GhiChu { get; set; }
    }
}
