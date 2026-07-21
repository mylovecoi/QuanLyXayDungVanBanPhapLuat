using DataAccess.Entities.Settings;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities.KeKhaiDangKyGia
{
    public class DoanhNghiep : BaseEntity
    {
        public Guid DonViQuanLyId { get; set; }
        [ForeignKey(nameof(DonViQuanLyId))]
        public DanhMucDonVi? DonViQuanLy { get; set; }
        [Required(ErrorMessage = "Thông tin không được bỏ trống")]
        [DisplayName("Mã số thuế")]
        public string? MaSoThue { get; set; }
        public string? TenDoanhNghiep { get; set; }
        public string? DiaChi { get; set; }
        [Phone]
        public string? SoDienThoai { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? GhiChu { get; set; }
        public string? TrangThai { get; set; }
        public string? GiayPhepKd { get; set; }
        [NotMapped]
        public IFormFile? GiayPhepKdUpload { get; set; }
        public string? Level { get; set; }
        public string? MaHoSo { get; set; }
        //Trang thái kết nối CSDLQG
        public string? TrangThaiCSDLQG { get; set; }
        public DateTime NgayKetNoi { get; set; }
    }
}
