using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.DinhGiaHHDV
{
    public class GiaThiTruongTongHop : BaseEntity
    {
        public string? MaHoSo { get; set; }
        public string? MaHoSoTongHop { get; set; }
        public Guid DonViQuanLyId { get; set; }
        public Guid ThongTuId { get; set; }
        public string? SoBc { get; set; }
        public DateTime NgayBc { get; set; }
        public DateTime NgayChotBc { get; set; }
        public string? Thang { get; set; }
        public string? Nam { get; set; }
        public string? CongBo { get; set; }
        public string? LichSu { get; set; }
        public string? GhiChu { get; set; }
        public string? TrangThai { get; set; }
        public string? PhanLoaiHoSo { get; set; }//0: Hồ sơ nhập chi tiết; 1: Hồ sơ nhận dữ liệu từ file excel
        public string? ChiTietExcel { get; set; }
        //Trang thái kết nối CSDLQG
        public string? TrangThaiCSDLQG { get; set; }
        public DateTime NgayKetNoi { get; set; }
    }
}
