using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.ThamDinhGia
{
    public class ThamDinhGiaDanhMucDonVi : BaseEntity
    {
        public string? MaGCN { get; set; }
        public string? TenDv { get; set; }
        public string? DiaChi { get; set; }
        public string? NguoiDaiDien { get; set; }
        public string? ChucVu { get; set; }
        public string? SoThe { get; set; }
        public DateTime NgayCap { get; set; }
        public string? SoQd { get; set; }
        public DateTime NgayQd { get; set; }
        public string? TrangThai { get; set; }
        public string? STTSapXep { get; set; }
    }
}
