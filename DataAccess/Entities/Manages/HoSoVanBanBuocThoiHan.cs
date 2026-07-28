namespace DataAccess.Entities.Manages
{
    public class HoSoVanBanBuocThoiHan : BaseEntity
    {
        public Guid HoSoVanBanId { get; set; }

        public Guid BuocQuyTrinhId { get; set; }

        public int ThuTuSapXep { get; set; }

        public int? SoNgayXuLy { get; set; }

        public int? SoNgayCanhBaoSapHan { get; set; }

        public string? GhiChu { get; set; }
    }
}
