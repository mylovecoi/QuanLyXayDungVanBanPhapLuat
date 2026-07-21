using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.ThamDinhGia
{
    public class ThamDinhGiaHoiDong : BaseEntity
    {
        public string? ToTung { get; set; }
        public string? CanCuPhapLy { get; set; }
        public string? TheoDeNghi { get; set; }
        public int CapHoiDong { get; set; }
        public int LoaiHoiDong { get; set; }
        public string? SoQd { get; set; }
        public DateTime NgayQd { get; set; }
        public string? CoQuanBanHanh { get; set; }
        public string? TenHoiDong { get; set; }
        public string? ChuTichHoiDong { get; set; }
        public string? ChucVu { get; set; }
        public string? NhiemVuHoiDong { get; set; }
        public string? NoiDungQd { get; set; }
        public string? MaTinhApDung { get; set; }
        public string? MaHuyenApDung { get; set; }
        public string? Ipf1 { get; set; }
        [NotMapped]
        public IFormFile? Ipf1Upload { get; set; }
    }
}
