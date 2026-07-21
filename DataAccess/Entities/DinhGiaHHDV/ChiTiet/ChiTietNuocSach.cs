using DataAccess.Entities.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.DinhGiaHHDV.ChiTiet
{
    public class ChiTietNuocSach : ChiTietBaseEntity
    {
        [DisplayName("Mã đối tượng")]
        public string? MaDoiTuong { get; set; }
        [DisplayName("Đối tượng sử dụng")]
        public string? DoiTuongSuDung { get; set; }
        [DisplayName("Tỷ trọng tiêu thụ")]
        public string? TyTrongTieuThu { get; set; }
        [DisplayName("Sản lượng")]
        public string? SanLuong { get; set; }
        [DisplayName("Thuế suất")]
        public double ThueSuat { get; set; }
        [DisplayName("Đơn giá chưa thuế")]
        public double DonGia1 { get; set; }
        [DisplayName("Đơn giá có thuế")]
        public double DonGia2 { get; set; }
    }
}
