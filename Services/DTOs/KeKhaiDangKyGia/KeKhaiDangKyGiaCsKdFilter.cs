using Microsoft.AspNetCore.Http;

namespace Services.DTOs.KeKhaiDangKyGia
{
    public class KeKhaiDangKyGiaCsKdFilter : BaseFilterDTO
    {
        public string MaNghe { get; private set; } = "";

        public KeKhaiDangKyGiaCsKdFilter()
        {
        }

        public KeKhaiDangKyGiaCsKdFilter(HttpRequest request) : base(request)
        {
            MaNghe = request.Query.TryGetValue("MaNghe", out var maNghe) ? maNghe.ToString() : "";
        }
    }
}
