using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities.Manages
{
    public class HoSoVanBan : BaseEntity
    {
        [Required(ErrorMessage = "Ma ho so khong duoc de trong")]
        public string MaHoSo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ten ho so khong duoc de trong")]
        public string TenHoSo { get; set; } = string.Empty;

        public Guid DanhMucVanBanId { get; set; }

        public Guid QuyTrinhSoanThaoId { get; set; }

        public Guid? BuocHienTaiId { get; set; }

        public Guid? DanhMucTrangThaiId { get; set; }

        public Guid DonViSoanThaoId { get; set; }

        public Guid NguoiTaoId { get; set; }

        public DateTime NgayTaoHoSo { get; set; } = DateTime.Now;

        public DateTime? HanXuLy { get; set; }

        public DateTime? NgayHoanThanh { get; set; }

        public int SoLanTraLaiHienTai { get; set; } = 0;

        public Guid? AttachedFileGroupId { get; set; }

        public string? MoTa { get; set; }

        public string? GhiChu { get; set; }
    }
}
