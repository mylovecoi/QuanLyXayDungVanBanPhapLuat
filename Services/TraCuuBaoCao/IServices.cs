using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccess.Entities.Settings;
using DataAccess.Entities.Settings.DanhMucGia;
using DataAccess.Entities.DinhGiaHHDV;
using DataAccess.Entities.KeKhaiDangKyGia;
using DataAccess.Entities.ThamDinhGia;
using Services.Model;

namespace Services.TraCuuBaoCao
{
    public interface ITraCuuService
    {
        Task<List<DanhMucKinhDoanh>> GetDanhMucKinhDoanhNganhAsync(string loaiGia);
        Task<List<DanhMucKinhDoanh>> GetDanhMucKinhDoanhNgheAsync(string loaiGia);
        Task<List<GiaThiTruongDanhMuc>> GetGiaThiTruongDanhMucAsync();
        Task<List<ThamDinhGiaDanhMucHangHoa>> GetThamDinhGiaDanhMucHangHoaAsync();
        Task<Tuple<List<object>, Dictionary<string, DinhGia>>> SearchDinhGiaCtAsync(string maNghe, DateTime? tuNgay, DateTime? denNgay, string soQd, string moTa, string maHoSo = null);
        Task<Tuple<List<KeKhaiDangKyGiaCt>, Dictionary<string, DataAccess.Entities.KeKhaiDangKyGia.KeKhaiDangKyGia>>> SearchKeKhaiDangKyGiaCtAsync(string maNghe, DateTime? tuNgay, DateTime? denNgay, string soQd, string moTa, string maHoSo = null);
        Task<Tuple<List<GiaThiTruongCt>, Dictionary<string, GiaThiTruong>>> SearchGiaThiTruongCtAsync(Guid thongTuId, DateTime? tuNgay, DateTime? denNgay, string soQd, string moTa, string maHoSo = null);
        Task<Tuple<List<ThamDinhGiaCt>, Dictionary<Guid, DataAccess.Entities.ThamDinhGia.ThamDinhGia>>> SearchThamDinhGiaCtAsync(Guid hangHoaId, DateTime? tuNgay, DateTime? denNgay, string soTbKl, string dvYeuCau, string maHoSo = null);
    }

    public interface IBaoCaoService
    {
        Task<List<DinhGia>> SearchDinhGiaReportAsync(string maNghe, DateTime? tuNgay, DateTime? denNgay);
        Task<List<DataAccess.Entities.KeKhaiDangKyGia.KeKhaiDangKyGia>> SearchKeKhaiDangKyGiaReportAsync(string maNghe, DateTime? tuNgay, DateTime? denNgay);
        Task<List<GiaThiTruong>> SearchGiaThiTruongReportAsync(Guid thongTuId, DateTime? tuNgay, DateTime? denNgay);
        Task<List<DataAccess.Entities.ThamDinhGia.ThamDinhGia>> SearchThamDinhGiaReportAsync(DateTime? tuNgay, DateTime? denNgay);
    }
}
