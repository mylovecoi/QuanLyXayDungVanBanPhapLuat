using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Services.Systems;

namespace Services.DTOs.Manages.ThongTinHoSo
{
    public class HoSoFilter : BaseFilterDTO
    {
        public bool? LoaiNghiepVu { get; private set; }
        public Guid LoaiHopDong { get; private set; }
        public Guid DonViId { get; private set; }
        public string? Status { get; private set; }
        public bool? LoaiCongChung { get; private set; }// true bản giấy - false bản điện tử
        public DateTime? NgayYeuCauTu { get; private set; }
        public DateTime? NgayYeuCauDen { get; private set; }
        public DateTime? NgayCongChungTu { get; private set; }
        public DateTime? NgayCongChungDen { get; private set; }

        public HoSoFilter(HttpRequest request, bool? loaiNghiepVu, IAuthService authService) : base(request)
        {
            LoaiNghiepVu = loaiNghiepVu; // Công chứng / chứng thực
            LoaiHopDong = Guid.TryParse(request.Query["LoaiHopDong"], out var loaiHopDong) ? loaiHopDong : Guid.Empty;
            DonViId = Guid.TryParse(request.Query["DonViId"], out var donViId) ? donViId : authService.GetUserInfo()?.DanhMucDonViId ?? Guid.Empty;
            Status = request.Query.TryGetValue("TrangThaiHoSo", out var status) && !string.IsNullOrEmpty(status) ? status.ToString().Trim() : "CTN";
            LoaiCongChung = bool.TryParse(request.Query["LoaiCongChung"], out var loaiCongChung) ? loaiCongChung : null;

            var now = DateTime.Now;
            var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            NgayYeuCauTu = DateTime.TryParse(request.Query["NgayYeuCauTu"], out var ngayYeuCauTu) ? ngayYeuCauTu : firstDayOfMonth;
            NgayYeuCauDen = DateTime.TryParse(request.Query["NgayYeuCauDen"], out var ngayYeuCauDen) ? ngayYeuCauDen : lastDayOfMonth;
            NgayCongChungTu = DateTime.TryParse(request.Query["NgayCongChungTu"], out var ngayCongChungTu) ? ngayCongChungTu : firstDayOfMonth;
            NgayCongChungDen = DateTime.TryParse(request.Query["NgayCongChungDen"], out var ngayCongChungDen) ? ngayCongChungDen : lastDayOfMonth;

        }

        public void SetStatus(string status) => Status = status;
    }
}
