using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.DinhGiaHHDV
{
    public class GiaThiTruong : BaseEntity
    {
        public string? MaHoSo { get; set; }
        public Guid DiaBanId { get; set; }
        public Guid DonViQuanLyId { get; set; }
        public Guid DonViChuQuanId { get; set; }
        public Guid ThongTuId { get; set; }
        public string? SoQd { get; set; }
        public DateTime Thoidiem { get; set; }
        public string? SoQdLk { get; set; }
        public DateTime ThoiDiemLk { get; set; }
        public string? Thang { get; set; }
        public string? Nam { get; set; }
        public string? CongBo { get; set; }
        public string? LichSu { get; set; }
        public string? GhiChu { get; set; }
        public string? LyDo { get; set; }
        public string? TrangThai { get; set; }
        public string? PhanLoaiHoSo { get; set; }//0: Hồ sơ nhập chi tiết; 1: Hồ sơ nhận dữ liệu từ file excel
        public string? ChiTietExcel { get; set; }
        //Trang thái kết nối CSDLQG
        public string? TrangThaiCSDLQG { get; set; }
        public DateTime NgayKetNoi { get; set; }

        [NotMapped]
        public List<DataAccess.Entities.Manages.AttachedFile> AttachedFiles { get; set; } = [];
    }
}
