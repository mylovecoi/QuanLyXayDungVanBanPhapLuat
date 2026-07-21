using DataAccess.Entities.Settings;
using DataAccess.Enums;

namespace Services.DTOs.BaoCaoKhac
{
    public class BaoCaoSuDungLaoDongResponse
    {
        public ThongTinToChuc ThongTinToChuc { get; set; } = new();
        public List<DanhMucCanBo> DanhSachCongChungVien { get; set; } = new();
        public List<DanhMucCanBo> DanhSachNhanVien { get; set; } = new();
        public ThongKeTongHop ThongKe { get; set; } = new();
    }

    public class ThongTinToChuc
    {
        public string TenToChuc { get; set; } = string.Empty;
        public string TinhThanhPho { get; set; } = string.Empty;
        public string QuyenSo { get; set; } = string.Empty;
        public DateTime NgayMoSo { get; set; }
        public DateTime NgayKhoaSo { get; set; }
        public int Nam { get; set; }
    }

    public class ThongKeTongHop
    {
        public int TongSoLaoDong { get; set; }
        public int SoCongChungVien { get; set; }
        public int SoNhanVienNghiepVu { get; set; }
        public int SoNhanVienKhac { get; set; }
        public int TongSoHopDongDaKy { get; set; }
        public int SoHopDongDaChamDut { get; set; }
        public int SoHopDongDangThucHien { get; set; }
        public decimal TongTienBaoHiemTrachNhiem { get; set; }
        public decimal TongTienBHXH { get; set; }
        public decimal TongTienBHYT { get; set; }
        public DateTime NgayBaoCao { get; set; }
        public string DiaDanh { get; set; } = string.Empty;
    }
}
