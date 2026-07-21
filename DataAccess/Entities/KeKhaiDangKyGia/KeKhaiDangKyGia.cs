using System.ComponentModel;
using System.Collections.Generic;
using DataAccess.Entities.Settings;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities.KeKhaiDangKyGia
{
    public class KeKhaiDangKyGia
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid DoanhNghiepQuanLyId { get; set; }
        [ForeignKey(nameof(DoanhNghiepQuanLyId))]
        public DoanhNghiep? DoanhNghiepQuanLy { get; set; }
        public string? MaHoSo { get; set; }
        public string? PhanLoai { get; set; }
        public string? MaNghe { get; set; }
        public Guid DonViQuanLyId { get; set; } // Macqcq
        public string? DonViDongChuyenId { get; set; } // Madvdongchuyen

        [DisplayName("Số quyết định")]
        public string? SoQd { get; set; }

        [DisplayName("Ngày quyết định")]
        public DateTime NgayQd { get; set; }
        public string? SoQdLk { get; set; }
        public DateTime NgayQdLk { get; set; }

        [DisplayName("Ngày áp dụng")]
        public DateTime NgayThucHien { get; set; }
        public DateTime NgayTraHoSo { get; set; }
        public DateTime ThoiGianThucHien { get; set; }
        public string? DonViTinh { get; set; }

        [DisplayName("Ghi chú")]
        public string? GhiChu { get; set; }

        public string? ThongTinNguoiChuyen { get; set; }
        public string? SoDtNguoiChuyen { get; set; }
        public DateTime NgayChuyen { get; set; }

        [DisplayName("Trạng thái")]
        public string? TrangThai { get; set; }
        public string? LyDo { get; set; }
        public string? SoHsDuyet { get; set; }
        public DateTime NgayDuyet { get; set; }

        public string? YtCauThanhGia { get; set; }
        public string? ThyDgGadGia { get; set; }

        public DateTime ThoiDiem { get; set; }

        public string? ChucDanhKy { get; set; }
        public string? HoTenNguoiKy { get; set; }

        //Trang thái kết nối CSDLQG
        public string? TrangThaiCSDLQG { get; set; }
        public DateTime NgayKetNoi { get; set; }
        public string? ChiTietExcel { get; set; }
        [NotMapped]
        public List<DataAccess.Entities.Manages.AttachedFile> AttachedFiles { get; set; } = [];
    }
}
