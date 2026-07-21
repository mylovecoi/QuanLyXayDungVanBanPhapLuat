using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.KeKhaiDangKyGia
{
    public class KeKhaiDangKyGiaCt
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid DoanhNghiepQuanLyId { get; set; }
        [ForeignKey(nameof(DoanhNghiepQuanLyId))]
        public DoanhNghiep? DoanhNghiepQuanLy { get; set; }
        public string? MaHoSo { get; set; }

        [DisplayName("Tên dịch vụ cung ứng")]
        public string? TenDvCungUng { get; set; }

        [DisplayName("Quy cách chất lượng")]
        public string? QuyCachChatLuong { get; set; }

        [DisplayName("Thời gian thực hiện")]
        public string? ThoiGianThucHien { get; set; }

        [DisplayName("Loại giá")]
        public string? LoaiGia { get; set; }

        [DisplayName("Đơn vị tính")]
        public string? DonViTinh { get; set; }

        [DisplayName("Mức giá kê khai liền kề")]
        public double MucGiaKeKhaiLk { get; set; }

        [DisplayName("Mức giá kê khai")]
        public double MucGiaKeKhai { get; set; }

        [DisplayName("Hình thức kinh doanh")]
        public string? HinhThucKinhDoanh { get; set; }

        [DisplayName("Ghi chú")]
        public string? GhiChu { get; set; }

        [DisplayName("Trạng thái")]
        public string? TrangThai { get; set; }
    }
}
