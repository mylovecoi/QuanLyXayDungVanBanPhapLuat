using DataAccess.Entities.DinhGiaHHDV;
using Services.DTOs.DinhGiaHHDV.ThongTinHoSo;
using Services.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.DinhGiaHHDV.DinhGiaKhac
{
    public interface IDinhGiaService
    {
        Task<CommonResponse> GetListByFilterAsync(DinhGiaFilter filter, string MaNghe);
        Task<CommonResponse> GetSingleByIdAsync(Guid hoSoId);
        Task<CommonResponse> CreateAsync(Guid donViId, string MaNghe, Guid? danhMucId);
        Task<CommonResponse> StoreAsync(DinhGia request);
        Task<CommonResponse> EditAsync(Guid hoSoId);
        Task<CommonResponse> UpdateAsync(DinhGia request);
        Task<CommonResponse> DeleteAsync(Guid hoSoId);
        Task<CommonResponse> ChuyenAsync(Guid hoSoId, string trangThai);
        Task<CommonResponse> GetCodeExcelAsync(string Mahs);
        Task<CommonResponse> SaveCodeExcelAsync(string Mahs, string jsonString);
        Task<CommonResponse> GetDetailsByMaHoSoAsync(string maHoSo);
        //Task<List<DynamicCategoryOption>> GetCategoryOptionsByMaNgheAsync(string maNghe);
        Task<CommonResponse> GetSoLuongDinhGiaTheoThangAsync();
        (string danhMucTable, string chiTietTable) GetTableNames(string maNghe);
    }

    public interface IDinhGiaXetDuyetService
    {
        Task<CommonResponse> GetListXetDuyetByFilterAsync(DinhGiaFilter filter, string MaNghe);
        Task<CommonResponse> DuyetAsync(Guid id);
        Task<CommonResponse> HuyDuyetAsync(Guid id);
        Task<CommonResponse> TraLaiAsync(Guid id, string lyDo);
        Task<CommonResponse> CongBoAsync(Guid id);
        Task<CommonResponse> HuyCongBoAsync(Guid id);
        Task<CommonResponse> GetSingleByIdAsync(Guid hoSoId);
    }
}
