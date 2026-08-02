using DataAccess.Entities.QuanLyDanhMuc;

namespace UI.ViewModels
{
    public class DanhMucTieuChiDiemUpsertViewModel
    {
        public DanhMucTieuChiDiem TieuChi { get; set; } = new();
        public List<DanhMucTieuChiDiemMuc> Mucs { get; set; } = new();
    }
}
