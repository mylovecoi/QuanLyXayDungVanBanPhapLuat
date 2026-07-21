using DataAccess.Entities.DinhGiaHHDV;
using Services.Model;
using System;
using System.Threading.Tasks;

namespace Services.DinhGiaHHDV.GiaThiTruong
{
    public interface IGiaThiTruongDanhMucService
    {
        Task<CommonResponse> GetListGiaThiTruongDanhMucAsync(string search, int pageSize, int pageCurrent);
        Task<CommonResponse> StoreAsync(GiaThiTruongDanhMuc request);
        Task<CommonResponse> EditAsync(Guid id);
        Task<CommonResponse> UpdateAsync(GiaThiTruongDanhMuc request);
        Task<CommonResponse> DeleteAsync(Guid id);
    }

    public interface IGiaThiTruongDanhMucCtService
    {
        Task<CommonResponse> GetListDanhMucCtAsync(Guid thongTuId, string search, int pageSize, int pageCurrent);
        Task<CommonResponse> StoreAsync(GiaThiTruongDanhMucCt request);
        Task<CommonResponse> StoreRangeAsync(List<GiaThiTruongDanhMucCt> requests);
        Task<CommonResponse> EditAsync(Guid id);
        Task<CommonResponse> UpdateAsync(GiaThiTruongDanhMucCt request);
        Task<CommonResponse> DeleteAsync(Guid id);
        Task<CommonResponse> DeleteAllAsync(Guid thongTuId);
    }

    public interface IGiaThiTruongService
    {
        Task<CommonResponse> GetListByFilterAsync(int year, string thang, Guid donViId, string search, int pageSize, int pageCurrent);
        Task<CommonResponse> CreateAsync(Guid thongTuId, Guid donViId, string thang, string nam);
        Task<CommonResponse> StoreAsync(DataAccess.Entities.DinhGiaHHDV.GiaThiTruong request);
        Task<CommonResponse> EditAsync(Guid id);
        Task<CommonResponse> UpdateAsync(DataAccess.Entities.DinhGiaHHDV.GiaThiTruong request);
        Task<CommonResponse> DeleteAsync(Guid id);
        Task<CommonResponse> GetCodeExcelAsync(string MaHoSo);
        Task<CommonResponse> SaveCodeExcelAsync(string MaHoSo, string jsonString);
        Task<CommonResponse> GetDetailsByMaHoSoAsync(string maHoSo);
        Task<CommonResponse> ChuyenAsync(Guid hoSoId, string trangThai);
        Task<CommonResponse> GetGiaThiTruongStatsAsync();
    }

    public interface IGiaThiTruongXetDuyetService
    {
        Task<CommonResponse> GetListXetDuyetByFilterAsync(int year, string thang, Guid donViId, bool isSSA, string search, int pageSize, int pageCurrent);
        Task<CommonResponse> DuyetAsync(Guid id);
        Task<CommonResponse> HuyDuyetAsync(Guid id);
        Task<CommonResponse> TraLaiAsync(Guid id, string lyDo);
        Task<CommonResponse> CongBoAsync(Guid id);
        Task<CommonResponse> HuyCongBoAsync(Guid id);
    }

    public interface IGiaThiTruongTongHopService
    {
        Task<CommonResponse> GetListByFilterAsync(int year, string thang, Guid donViId, string search, int pageSize, int pageCurrent);
        Task<CommonResponse> CreateAsync(Guid thongTuId, Guid donViId, string thang, string nam, string[] selectedHoSo);
        Task<CommonResponse> StoreAsync(GiaThiTruongTongHop request, List<GiaThiTruongTongHopCt> details);
        Task<CommonResponse> EditAsync(Guid id);
        Task<CommonResponse> UpdateAsync(GiaThiTruongTongHop request, List<GiaThiTruongTongHopCt> details);
        Task<CommonResponse> DeleteAsync(Guid id);
        Task<CommonResponse> GetCodeExcelAsync(string MaHoSo);
        Task<CommonResponse> SaveCodeExcelAsync(string MaHoSo, string jsonString);
    }
}
