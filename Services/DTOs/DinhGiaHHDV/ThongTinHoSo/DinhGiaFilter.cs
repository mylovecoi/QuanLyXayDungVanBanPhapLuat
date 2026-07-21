using Microsoft.AspNetCore.Http;
using Services.Systems;

namespace Services.DTOs.DinhGiaHHDV.ThongTinHoSo
{
    public class DinhGiaFilter : BaseFilterDTO
    {
        public Guid LoaiHopDong { get; private set; }
        public Guid DonViId { get; private set; }
        public string? TrangThai { get; private set; }
        public DateTime? ThoiDiemTu { get; private set; }
        public DateTime? ThoiDiemDen { get; private set; }

        public DinhGiaFilter(HttpRequest request, IAuthService authService) : base(request)
        {
            LoaiHopDong = Guid.TryParse(request.Query["LoaiHopDong"], out var loaiHopDong) ? loaiHopDong : Guid.Empty;
            DonViId = Guid.TryParse(request.Query["DonViId"], out var donViId) ? donViId : authService.GetUserInfo()?.DanhMucDonViId ?? Guid.Empty;
            TrangThai = request.Query.TryGetValue("TrangThaiHoSo", out var trangThai) && !string.IsNullOrEmpty(trangThai) ? trangThai.ToString().Trim() : "CC";

            var now = DateTime.Now;
            var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            ThoiDiemTu = DateTime.TryParse(request.Query["ThoiDiemTu"], out var thoiDiemTu) ? thoiDiemTu : firstDayOfMonth;
            ThoiDiemDen = DateTime.TryParse(request.Query["ThoiDiemDen"], out var thoiDiemDen) ? thoiDiemDen : lastDayOfMonth;

        }

        public void SetStatus(string trangThai) => TrangThai = trangThai;
    }
}
