using System;
using Microsoft.AspNetCore.Http;

namespace Services.DTOs.KeKhaiDangKyGia
{
    public class KeKhaiDangKyGiaFilter : BaseFilterDTO
    {
        public Guid DoanhNghiepQuanLyId { get; set; }
        public Guid DonViQuanLyId { get; set; }
        public string? TrangThai { get; set; }
        public string? MaNghe { get; set; }

        public KeKhaiDangKyGiaFilter()
        {
        }

        public KeKhaiDangKyGiaFilter(HttpRequest request) : base(request)
        {
            DoanhNghiepQuanLyId = Guid.TryParse(request.Query["DoanhNghiepQuanLyId"], out var id) ? id : Guid.Empty;
            DonViQuanLyId = Guid.TryParse(request.Query["DonViQuanLyId"], out var donViId) ? donViId : Guid.Empty;
            TrangThai = request.Query.TryGetValue("TrangThai", out var trangThai) && !string.IsNullOrEmpty(trangThai) ? trangThai.ToString().Trim() : "CD";
            MaNghe = request.Query.TryGetValue("MaNghe", out var maNghe) && !string.IsNullOrEmpty(maNghe) ? maNghe.ToString().Trim() : "all";
        }

        public void SetDonViQuanLyId(Guid id) => DonViQuanLyId = id;
    }
}
