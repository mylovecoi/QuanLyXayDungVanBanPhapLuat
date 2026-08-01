using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities.QuanLyDanhMuc
{
    public class DanhMucQuyTrinhSoanThao : BaseEntity
    {
        [Required(ErrorMessage = "Ma quy trinh khong duoc de trong")]
        public string MaQuyTrinh { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ten quy trinh khong duoc de trong")]
        public string TenQuyTrinh { get; set; } = string.Empty;

        [Required(ErrorMessage = "Loai quy trinh khong duoc de trong")]
        public string LoaiQuyTrinh { get; set; } = "XayDung";

        public Guid? DanhMucVanBanId { get; set; }

        public string? DanhMucVanBanIds { get; set; }

        public string? CapApDung { get; set; }

        public int PhienBan { get; set; } = 1;

        public bool TrangThai { get; set; } = true;

        public string? MoTa { get; set; }

        public string? GhiChu { get; set; }
    }
}
