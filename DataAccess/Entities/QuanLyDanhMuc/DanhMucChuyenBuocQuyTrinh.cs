using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities.QuanLyDanhMuc
{
    public class DanhMucChuyenBuocQuyTrinh : BaseEntity
    {
        public Guid QuyTrinhSoanThaoId { get; set; }

        public Guid TuBuocId { get; set; }

        public Guid DenBuocId { get; set; }

        [Required(ErrorMessage = "Dieu kien ket qua khong duoc de trong")]
        public string DieuKienKetQua { get; set; } = string.Empty;

        public bool LaNhanhMacDinh { get; set; } = false;

        public string? MoTa { get; set; }

        public string? GhiChu { get; set; }
    }
}
