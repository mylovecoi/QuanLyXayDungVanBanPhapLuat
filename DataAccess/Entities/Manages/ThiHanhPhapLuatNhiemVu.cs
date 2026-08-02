using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities.Manages
{
    public class ThiHanhPhapLuatNhiemVu : BaseEntity
    {
        public Guid KeHoachId { get; set; }

        [StringLength(50)]
        public string MaNhiemVu { get; set; } = string.Empty;

        [StringLength(500)]
        public string TenNhiemVu { get; set; } = string.Empty;

        public string? NoiDungNhiemVu { get; set; }

        public Guid DonViChuTriId { get; set; }

        public Guid? NguoiDieuPhoiId { get; set; }

        public DateTime? NgayBatDau { get; set; }

        public DateTime? HanHoanThanh { get; set; }

        [StringLength(20)]
        public string MucDoUuTien { get; set; } = "TRUNG_BINH";

        [StringLength(30)]
        public string TrangThai { get; set; } = "CHUA_THUC_HIEN";

        public int ThuTuSapXep { get; set; } = 0;

        public bool YeuCauBaoCao { get; set; } = true;

        public string? GhiChu { get; set; }
    }
}
