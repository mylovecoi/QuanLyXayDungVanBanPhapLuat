using DataAccess.Entities.ThamDinhGia;
using Services.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.ThamDinhGia
{
    public interface IThamDinhGiaDanhMucDonViService
    {
        Task<CommonResponse> GetDanhMucDonViAsync(string search, int pageSize, int pageCurrent);
        Task<CommonResponse> StoreAsync(ThamDinhGiaDanhMucDonVi request);
        Task<CommonResponse> StoreRangeAsync(List<ThamDinhGiaDanhMucDonVi> requests);
        Task<CommonResponse> EditAsync(Guid id);
        Task<CommonResponse> UpdateAsync(ThamDinhGiaDanhMucDonVi request);
        Task<CommonResponse> DeleteAsync(Guid id);
    }
    
    public interface IThamDinhGiaDanhMucHangHoaService
    {
        Task<CommonResponse> GetListThamDinhGiaDanhMucHangHoaAsync(string search, int pageSize, int pageCurrent);
        Task<CommonResponse> StoreAsync(ThamDinhGiaDanhMucHangHoa request);
        Task<CommonResponse> EditAsync(Guid id);
        Task<CommonResponse> UpdateAsync(ThamDinhGiaDanhMucHangHoa request);
        Task<CommonResponse> DeleteAsync(Guid id);
    }

    public interface IThamDinhGiaDanhMucHangHoaCtService
    {
        Task<CommonResponse> GetListDanhMucCtAsync(Guid hangHoaId, string search, int pageSize, int pageCurrent);
        Task<CommonResponse> StoreAsync(ThamDinhGiaDanhMucHangHoaCt request);
        Task<CommonResponse> StoreRangeAsync(List<ThamDinhGiaDanhMucHangHoaCt> requests);
        Task<CommonResponse> EditAsync(Guid id);
        Task<CommonResponse> UpdateAsync(ThamDinhGiaDanhMucHangHoaCt request);
        Task<CommonResponse> DeleteAsync(Guid id);
        Task<CommonResponse> DeleteAllAsync(Guid hangHoaId);
    }

    public interface IThamDinhGiaHoiDongService
    {
        Task<CommonResponse> GetListThamDinhGiaHoiDongAsync(string search, int pageSize, int pageCurrent);
        Task<CommonResponse> StoreAsync(ThamDinhGiaHoiDong request);
        Task<CommonResponse> EditAsync(Guid id);
        Task<CommonResponse> UpdateAsync(ThamDinhGiaHoiDong request);
        Task<CommonResponse> DeleteAsync(Guid id);
    }

    public interface IThamDinhGiaHoiDongCtService
    {
        Task<CommonResponse> GetListDanhMucCtAsync(Guid hoiDongId, string search, int pageSize, int pageCurrent);
        Task<CommonResponse> StoreAsync(ThamDinhGiaHoiDongCt request);
        Task<CommonResponse> StoreRangeAsync(List<ThamDinhGiaHoiDongCt> requests);
        Task<CommonResponse> EditAsync(Guid id);
        Task<CommonResponse> UpdateAsync(ThamDinhGiaHoiDongCt request);
        Task<CommonResponse> DeleteAsync(Guid id);
        Task<CommonResponse> DeleteAllAsync(Guid hoiDongId);
    }

    public interface IThamDinhGiaService
    {
        Task<CommonResponse> GetListByFilterAsync(int year, Guid donViId, string search, int pageSize, int pageCurrent);
        Task<CommonResponse> CreateAsync(Guid hangHoaId, Guid donViId, string phanLoai);
        Task<CommonResponse> StoreAsync(DataAccess.Entities.ThamDinhGia.ThamDinhGia request);
        Task<CommonResponse> EditAsync(Guid id);
        Task<CommonResponse> UpdateAsync(DataAccess.Entities.ThamDinhGia.ThamDinhGia request);
        Task<CommonResponse> DeleteAsync(Guid id);
        Task<CommonResponse> GetCodeExcelAsync(string MaHoSo);
        Task<CommonResponse> SaveCodeExcelAsync(string MaHoSo, string jsonString);
        Task<CommonResponse> GetDetailsByMaHoSoAsync(string maHoSo);
        Task<CommonResponse> ChuyenAsync(Guid hoSoId, string trangThai);
        Task<CommonResponse> GetThamDinhGiaStatsAsync();
    }

    public interface IThamDinhGiaXetDuyetService
    {
        Task<CommonResponse> GetListXetDuyetByFilterAsync(int year, Guid donViId, bool isSSA, string search, int pageSize, int pageCurrent);
        Task<CommonResponse> DuyetAsync(Guid id);
        Task<CommonResponse> HuyDuyetAsync(Guid id);
        Task<CommonResponse> TraLaiAsync(Guid id, string lyDo);
        Task<CommonResponse> CongBoAsync(Guid id);
        Task<CommonResponse> HuyCongBoAsync(Guid id);
    }
}
