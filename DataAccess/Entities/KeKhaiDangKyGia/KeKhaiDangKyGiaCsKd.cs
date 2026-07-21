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
    public class KeKhaiDangKyGiaCsKd
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } // Macskd
        public string? TenCsKd { get; set; }
        public Guid DoanhNghiepQuanLyId { get; set; }
        [ForeignKey(nameof(DoanhNghiepQuanLyId))]
        public DoanhNghiep? DoanhNghiepQuanLy { get; set; }
        public string? MaNghe { get; set; }
        [DisplayName("Địa chỉ")]
        public string? DiaChi { get; set; }
        [DisplayName("Số điện thoại")]
        public string? SoDienThoai { get; set; }
        //Trang thái kết nối CSDLQG
        public string? TrangThaiCSDLQG { get; set; }
        public DateTime NgayKetNoi { get; set; }
        public string? TrangThaiCSDLQG_DMHH { get; set; }
        public DateTime NgayKetNoi_DMHH { get; set; }
        public string? TrangThaiCSDLQG_DMDT { get; set; }
        public DateTime NgayKetNoi_DMDT { get; set; }
        public string? TrangThaiCSDLQG_DMKH { get; set; }
        public DateTime NgayKetNoi_DMKH { get; set; }
    }
}
