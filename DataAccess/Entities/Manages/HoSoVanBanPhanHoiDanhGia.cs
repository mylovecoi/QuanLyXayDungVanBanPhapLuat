namespace DataAccess.Entities.Manages
{
    public class HoSoVanBanPhanHoiDanhGia : BaseEntity
    {
        public Guid HoSoVanBanDanhGiaId { get; set; }

        public Guid HoSoVanBanId { get; set; }

        public int LanDanhGia { get; set; } = 1;

        public Guid DonViSoanThaoId { get; set; }

        public Guid? NguoiPhanHoiId { get; set; }

        public DateTime NgayPhanHoi { get; set; } = DateTime.Now;

        public string? NoiDungGiaiTrinh { get; set; }

        public Guid? AttachedFileGroupId { get; set; }

        public string? GhiChu { get; set; }
    }
}
