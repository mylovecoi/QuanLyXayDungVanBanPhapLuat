using Microsoft.AspNetCore.Http;

namespace Services.DTOs.Settings.DanhMucDungChung
{
    public class DanhMucKinhDoanhFilter : BaseFilterDTO
    {
        public string LoaiGia { get; private set; } = "";

        public DanhMucKinhDoanhFilter()
        {
        }

        public DanhMucKinhDoanhFilter(HttpRequest request) : base(request)
        {
            LoaiGia = request.Query.TryGetValue("LoaiGia", out var loaiGia) ? loaiGia.ToString() : "";
        }
    }
}
