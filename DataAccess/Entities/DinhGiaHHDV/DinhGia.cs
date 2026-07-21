using DataAccess.Entities.Settings;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities.DinhGiaHHDV
{
    public class DinhGia : BaseEntity
    {
        public string? MaNghe { get; set; }
        [DisplayName("Mã hồ sơ")]
        public string? MaHoSo { get; set; }
        [DisplayName("Số quyết định")]
        public string? SoQd { get; set; }
        [DisplayName("Mô tả")]
        public string? MoTa { get; set; }
        public string? CongBo { get; set; }
        [DisplayName("Ghi chú")]
        public string? GhiChu { get; set; }
        [DisplayName("Thời điểm")]
        public DateTime ThoiDiem { get; set; }
        [DisplayName("Ngày công bố")]
        public DateTime NgayCongBo { get; set; }
        [DisplayName("Ngày duyệt")]
        public DateTime NgayDuyet { get; set; }
        [DisplayName("Đơn Vị Quản Lý")]
        public Guid DonViQuanLyId { get; set; } // MaDv
        [ForeignKey(nameof(DonViQuanLyId))]
        public DanhMucDonVi? DonViQuanLy { get; set; }
        [DisplayName("Lý do")]
        public string? LyDo { get; set; }
        [DisplayName("Thông tin")]
        public string? ThongTin { get; set; }
        [DisplayName("Trạng thái")]
        public string? TrangThai { get; set; }
        public string? ChiTietExcel { get; set; }
        [NotMapped]
        public List<DataAccess.Entities.Manages.AttachedFile> AttachedFiles { get; set; } = [];
    }
}
