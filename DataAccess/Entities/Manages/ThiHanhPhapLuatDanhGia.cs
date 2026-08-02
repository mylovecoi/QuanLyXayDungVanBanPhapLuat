using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities.Manages
{
    public class ThiHanhPhapLuatDanhGia : BaseEntity
    {
        public Guid KeHoachId { get; set; }

        public Guid? NhiemVuId { get; set; }

        public Guid? ChiTietNhiemVuId { get; set; }

        public Guid DonViDuocDanhGiaId { get; set; }

        public Guid? NguoiDanhGiaId { get; set; }

        public DateTime NgayDanhGia { get; set; } = DateTime.Now;

        [StringLength(30)]
        public string KetQuaDanhGia { get; set; } = "CHUA_THUC_HIEN";

        [StringLength(30)]
        public string MucDoCanhBao { get; set; } = "BINH_THUONG";

        public string? NoiDungDanhGia { get; set; }

        public string? KienNghiXuLy { get; set; }

        public string? YeuCauBoSung { get; set; }

        [StringLength(20)]
        public string TrangThai { get; set; } = "NHAP";

        public string? GhiChu { get; set; }
    }
}
