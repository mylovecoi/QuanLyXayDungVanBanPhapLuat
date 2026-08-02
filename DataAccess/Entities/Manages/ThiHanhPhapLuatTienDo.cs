using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities.Manages
{
    public class ThiHanhPhapLuatTienDo : BaseEntity
    {
        public Guid ChiTietNhiemVuId { get; set; }

        public Guid DonViCapNhatId { get; set; }

        public Guid? NguoiCapNhatId { get; set; }

        public DateTime NgayCapNhat { get; set; } = DateTime.Now;

        public int TyLeHoanThanh { get; set; } = 0;

        public string? KetQuaThucHien { get; set; }

        public string? NoiDungBaoCao { get; set; }

        public string? KhoKhanVuongMac { get; set; }

        public string? DeXuatKienNghi { get; set; }

        [StringLength(20)]
        public string TrangThaiBaoCao { get; set; } = "NHAP";

        public Guid? AttachedFileGroupId { get; set; }

        public string? GhiChu { get; set; }
    }
}
