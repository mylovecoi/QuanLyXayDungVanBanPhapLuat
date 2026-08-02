using DataAccess.Entities.Manages;
using DataAccess.Entities.Settings;

namespace Services.DTOs
{
    public class HomeResponseDto
    {
        public List<ThuTucHanhChinh> ThuTucHanhChinhs { get; set; } = new();
        public List<AttachedFile> VanBanPhapLuats { get; set; } = new();
        public DashboardSummaryDto Summary { get; set; } = new();
        public DashboardHoSoChartDto HoSoChart { get; set; } = new();
        public DashboardThiHanhChartDto ThiHanhChart { get; set; } = new();
        public List<DashboardStepItemDto> HoSoByStep { get; set; } = new();
        public List<DashboardDonViItemDto> HoSoByDonVi { get; set; } = new();
        public List<DashboardCanhBaoItemDto> ThiHanhCanhBao { get; set; } = new();
    }

    public class DashboardSummaryDto
    {
        public int TongHoSoVanBan { get; set; }
        public int HoSoDangXuLy { get; set; }
        public int HoSoDaHoanThanh { get; set; }
        public int HoSoDaBanHanh { get; set; }
        public int TongKeHoachThiHanh { get; set; }
        public int NhiemVuThiHanhQuaHan { get; set; }
        public int NhiemVuThiHanhChamTienDo { get; set; }
        public int NhiemVuThiHanhChuaNhapLieu { get; set; }
        public int NamThongKe { get; set; }
    }

    public class DashboardHoSoChartDto
    {
        public int Year { get; set; }
        public List<string> Categories { get; set; } = new();
        public List<int> HoSoTaoMoiTheoThang { get; set; } = new();
        public List<int> HoSoHoanThanhTheoThang { get; set; } = new();
        public List<int> HoSoBanHanhTheoThang { get; set; } = new();
        public List<string> XepLoaiLabels { get; set; } = new();
        public List<int> XepLoaiValues { get; set; } = new();
    }

    public class DashboardThiHanhChartDto
    {
        public List<string> TrangThaiLabels { get; set; } = new();
        public List<int> TrangThaiValues { get; set; } = new();
        public List<string> CanhBaoLabels { get; set; } = new();
        public List<int> CanhBaoValues { get; set; } = new();
    }

    public class DashboardStepItemDto
    {
        public string MaBuoc { get; set; } = string.Empty;
        public string TenBuoc { get; set; } = string.Empty;
        public int SoLuong { get; set; }
    }

    public class DashboardDonViItemDto
    {
        public Guid DonViId { get; set; }
        public string TenDonVi { get; set; } = string.Empty;
        public int SoLuongHoSo { get; set; }
    }

    public class DashboardCanhBaoItemDto
    {
        public string MaCanhBao { get; set; } = string.Empty;
        public string TieuChi { get; set; } = string.Empty;
        public int SoLuong { get; set; }
    }
}
