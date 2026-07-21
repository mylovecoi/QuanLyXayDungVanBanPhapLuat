using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.DinhGiaHHDV.ChiTiet
{
    public class ChiTietGiaChung : ChiTietBaseEntity
    {
        public string? MaNghe { get; set; }
        public string? MaChiTiet { get; set; }
        [DisplayName("Tên chi tiết")]
        public string? TenChiTiet { get; set; }
        [DisplayName("Đơn giá trước")]
        public double DonGia1 { get; set; }
        [DisplayName("Đơn giá sau")]
        public double DonGia2 { get; set; }
        [DisplayName("Ghi chú")]
        public string? GhiChu { get; set; }
    }
}
