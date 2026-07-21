using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccess.Entities.KeKhaiDangKyGia;
using Services.Model;
using Services.DTOs.KeKhaiDangKyGia;

namespace Services.KeKhaiDangKyGia
{
    public interface IDoanhNghiepService
    {
        Task<DoanhNghiep?> GetDoanhNghiepByMaSoThueAsync(string maSoThue);
        Task<DoanhNghiep> GetOrCreateTempDoanhNghiepAsync(string maSoThue, Guid? defaultUnitId = null);
        Task<List<DoanhNghiepLvKd>> GetLvkdByDoanhNghiepIdAsync(Guid doanhNghiepId);
        Task<CommonResponse> StoreLvKdAsync(string maSoThue, string maNganh, string maNghe, Guid donViQuanLyId);
        Task<DoanhNghiepLvKd?> GetLvKdByIdAsync(Guid id);
        Task<CommonResponse> UpdateLvKdAsync(Guid id, string maNganh, string maNghe, Guid donViQuanLyId);
        Task<CommonResponse> DeleteLvKdAsync(Guid id);
        Task<CommonResponse> CompleteRegistrationAsync(DoanhNghiep request, string username, string password);
        Task<List<DoanhNghiep>> GetListDoanhNghiepAsync();
        Task<DoanhNghiep?> GetDoanhNghiepByIdAsync(Guid id);
    }

    public interface IKeKhaiDangKyGiaService
    {
        Task<CommonResponse> GetListByFilterAsync(KeKhaiDangKyGiaFilter filter);
        Task<CommonResponse> CreateAsync(Guid doanhNghiepQuanLyId, string maNghe);
        Task<CommonResponse> StoreAsync(DataAccess.Entities.KeKhaiDangKyGia.KeKhaiDangKyGia request);
        Task<CommonResponse> EditAsync(Guid id);
        Task<CommonResponse> UpdateAsync(DataAccess.Entities.KeKhaiDangKyGia.KeKhaiDangKyGia request);
        Task<CommonResponse> DeleteAsync(Guid id);
        Task<CommonResponse> GetCodeExcelAsync(string MaHoSo);
        Task<CommonResponse> SaveCodeExcelAsync(string MaHoSo, string jsonString);
        Task<CommonResponse> GetSingleByIdAsync(Guid id);
        Task<CommonResponse> GetDetailsByMaHoSoAsync(string maHoSo);
        Task<CommonResponse> ChuyenAsync(Guid hoSoId, Guid donViQuanLyId, string? thongTinNguoiChuyen, string? soDtNguoiChuyen);
        Task<CommonResponse> GetKeKhaiDangKyGiaStatsAsync();
    }

    public interface IKeKhaiDangKyGiaXetDuyetService
    {
        Task<CommonResponse> GetListXetDuyetByFilterAsync(KeKhaiDangKyGiaFilter filter);
        Task<CommonResponse> DuyetAsync(Guid id, string soHsDuyet);
        Task<CommonResponse> HuyDuyetAsync(Guid id);
        Task<CommonResponse> TraLaiAsync(Guid id, string lyDo);
        Task<CommonResponse> CongBoAsync(Guid id);
        Task<CommonResponse> HuyCongBoAsync(Guid id);
        Task<List<DataAccess.Entities.Settings.DanhMucGia.DanhMucKinhDoanh>> GetDanhMucKinhDoanhByFilterAsync(KeKhaiDangKyGiaFilter filter);
        Task<CommonResponse> GetSingleByIdAsync(Guid id);
    }

    public interface IKeKhaiDangKyGiaCsKdService
    {
        Task<CommonResponse> GetListByFilterAsync(KeKhaiDangKyGiaCsKdFilter filter);
        Task<CommonResponse> CreateAsync(string maNghe);
        Task<CommonResponse> StoreAsync(KeKhaiDangKyGiaCsKd request);
        Task<CommonResponse> EditAsync(Guid id);
        Task<CommonResponse> UpdateAsync(KeKhaiDangKyGiaCsKd request);
        Task<CommonResponse> DeleteAsync(Guid id);
    }

    public interface IKeKhaiDangKyGiaDanhMucService
    {
        // Danh mục đối tượng (KeKhaiDangKyGiaDMDT)
        Task<CommonResponse> GetListDTAsync(string doanhNghiepQuanLyIdStr, string search, int pageSize, int pageCurrent);
        Task<CommonResponse> StoreDTAsync(KeKhaiDangKyGiaDMDT request);
        Task<CommonResponse> EditDTAsync(Guid id);
        Task<CommonResponse> UpdateDTAsync(KeKhaiDangKyGiaDMDT request);
        Task<CommonResponse> DeleteDTAsync(Guid id);

        // Danh mục hàng hóa (KeKhaiDangKyGiaDMHH)
        Task<CommonResponse> GetListHHAsync(string doanhNghiepQuanLyIdStr, string search, int pageSize, int pageCurrent);
        Task<CommonResponse> StoreHHAsync(KeKhaiDangKyGiaDMHH request);
        Task<CommonResponse> EditHHAsync(Guid id);
        Task<CommonResponse> UpdateHHAsync(KeKhaiDangKyGiaDMHH request);
        Task<CommonResponse> DeleteHHAsync(Guid id);

        // Danh mục kho hàng (KeKhaiDangKyGiaDMKH)
        Task<CommonResponse> GetListKHAsync(string doanhNghiepQuanLyIdStr, string search, int pageSize, int pageCurrent);
        Task<CommonResponse> StoreKHAsync(KeKhaiDangKyGiaDMKH request);
        Task<CommonResponse> EditKHAsync(Guid id);
        Task<CommonResponse> UpdateKHAsync(KeKhaiDangKyGiaDMKH request);
        Task<CommonResponse> DeleteKHAsync(Guid id);
    }

    public interface IKeKhaiDangKyGiaTheoDoiService
    {
        Task<CommonResponse> GetListTheoDoiByFilterAsync(KeKhaiDangKyGiaFilter filter);
    }
}
