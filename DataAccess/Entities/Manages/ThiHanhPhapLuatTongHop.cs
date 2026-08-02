using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities.Manages
{
    public class ThiHanhPhapLuatTongHop : BaseEntity
    {
        public Guid KeHoachId { get; set; }

        public Guid? NguoiTongHopId { get; set; }

        public DateTime NgayTongHop { get; set; } = DateTime.Now;

        public int TongSoChiTietNhiemVu { get; set; } = 0;

        public int SoChiTietDaHoanThanh { get; set; } = 0;

        public int SoChiTietChuaHoanThanh { get; set; } = 0;

        public int SoChiTietChamTienDo { get; set; } = 0;

        public int SoChiTietQuaHan { get; set; } = 0;

        public int SoChiTietChuaNhapLieu { get; set; } = 0;

        public decimal TyLeHoanThanh { get; set; } = 0;

        public string? NhanXetTongHop { get; set; }

        public string? KetLuan { get; set; }

        public string? KienNghi { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; } = "NHAP";

        public Guid? AttachedFileGroupId { get; set; }

        public string? GhiChu { get; set; }
    }
}
