using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs
{
    public class ReportResponseDto<T>
    {
        public string TenBaoCao { get; set; } = string.Empty;
        public string TenDonVi { get; set; } = string.Empty; // lấy từ system info
        public string TenDonViChuQuan { get; set; } = string.Empty; // lấy từ system info
        public string TenDiaDanh { get; set; } = string.Empty;
        public string NguoiKy { get; set; } = string.Empty;
        public string? ChucDanhNguoiKy { get; set; }
        public string? KyHieuDonVi { get; set; }
        public DateTime NgayBaoCaoTu { get; set; } = DateTime.Now;
        public DateTime NgayBaoCaoDen { get; set; } = DateTime.Now;
        public DateTime NgayBaoCao { get; set; } = DateTime.Now;
        public List<T> Data { get; set; } = new();
    }
}
