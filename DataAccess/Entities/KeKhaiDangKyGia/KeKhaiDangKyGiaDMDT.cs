using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.KeKhaiDangKyGia
{
    public class KeKhaiDangKyGiaDMDT
    {
        public Guid Id { get; set; }
        public Guid DoanhNghiepQuanLyID { get; set; }
        public Guid DonViQuanLyId { get; set; } = Guid.Empty;
        public string? MaDT { get; set; }
        public string? TenDT { get; set; }
        public string? GhiChu { get; set; }
        //Trang thái kết nối CSDLQG
        public string? TrangThaiCSDLQG { get; set; }
        public DateTime NgayKetNoi { get; set; }
    }
}
