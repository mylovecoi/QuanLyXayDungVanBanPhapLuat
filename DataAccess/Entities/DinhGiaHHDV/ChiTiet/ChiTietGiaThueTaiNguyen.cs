using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.DinhGiaHHDV.ChiTiet
{
    public class ChiTietGiaThueTaiNguyen : ChiTietBaseEntity
    {
        [DisplayName("Mã nhóm tài nguyên cấp I")]
        public string? Cap1 { get; set; }
        [DisplayName("Mã nhóm tài nguyên cấp II")]
        public string? Cap2 { get; set; }
        [DisplayName("Mã nhóm tài nguyên cấp III")]
        public string? Cap3 { get; set; }
        [DisplayName("Mã nhóm tài nguyên cấp IV")]
        public string? Cap4 { get; set; }
        [DisplayName("Mã nhóm tài nguyên cấp V")]
        public string? Cap5 { get; set; }
        [DisplayName("Mã nhóm tài nguyên cấp VI")]
        public string? Cap6 { get; set; }
        [DisplayName("Tên nhóm, loại tài nguyên")]
        public string? Ten { get; set; }
        [DisplayName("Giá tính thuế tài nguyên(đồng)")]
        public double Gia { get; set; }
    }
}
