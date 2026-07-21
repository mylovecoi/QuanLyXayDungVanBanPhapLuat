using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.Manages.ThongTinHoSo.BaoCaoThongKe
{
    public class BaoCaoTongQuatResponseDto
    {
        public string TenHopDong { get; set; } = string.Empty;
        public int SoLuong { get; set; }
        public int Level { get; set; }
        public double ChiPhi { get; set; }
        public List<BaoCaoChiTietDto> ChiTiets { get; set; } = new();
    }

    public class BaoCaoChiTietDto
    {
        public DateTime NgayXuLy { get; set; }
        public string MaSoHoSo { get; set; } = string.Empty;
        public DateTime NgayTao { get; set; }
        public string? StrGiaTriHopDong { get; set; }
        public string? LoaiTaiSan { get; set; }
        public string? DiaBan { get; set; }
        public string? ThongTinTaiSan { get; set; }
        public string? ThongTinGiayToChungThuc { get; set; }
        public string? GhiChu { get; set; }
        public double ChiPhi { get; set; }
    }
}
