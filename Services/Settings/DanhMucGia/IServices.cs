using DataAccess.Entities.Settings;
using DataAccess.Entities.Settings.DanhMucGia;
using Services.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Services.DTOs.Settings.DanhMucDungChung;

namespace Services.Settings.DanhMucGia
{
    public interface IDanhMucGiaChungService
    {
        Task<CommonResponse> GetListDanhMucGiaChungAsync(string search, string maNghe, int pageSize, int pageCurrent);
        Task<CommonResponse> StoreAsync(DanhMucGiaChung request);
        Task<CommonResponse> EditAsync(Guid id);
        Task<CommonResponse> UpdateAsync(DanhMucGiaChung request);
        Task<CommonResponse> DeleteAsync(Guid id);
    }

    public interface IDanhMucGiaChungCtService
    {
        Task<CommonResponse> GetListDanhMucCtAsync(Guid danhMucGiaChungId, string search, int pageSize, int pageCurrent);
        Task<CommonResponse> StoreAsync(DanhMucGiaChungCt request);
        Task<CommonResponse> EditAsync(Guid id);
        Task<CommonResponse> UpdateAsync(DanhMucGiaChungCt request);
        Task<CommonResponse> DeleteAsync(Guid id);
        Task<CommonResponse> DeleteAllAsync(Guid danhMucGiaChungId);
        Task<CommonResponse> StoreRangeAsync(List<DanhMucGiaChungCt> requests);
    }

    public interface IDmKinhDoanhService
    {
        List<DanhMucKinhDoanh> GetListDmKinhDoanh();
        List<string> GetRolesByRole(string Role);
        List<string> GetListRolesByMaNghe(List<string> listMaNghe);

        Task<CommonResponse> GetListByFilterAsync(DanhMucKinhDoanhFilter filter);
        Task<CommonResponse> CreateAsync(Guid Id, string LoaiGia);
        Task<CommonResponse> StoreAsync(DanhMucKinhDoanh request, string[] DonViQuanLyList, string[] DonViDongChuyenList);
        Task<CommonResponse> EditAsync(Guid id);
        Task<CommonResponse> UpdateAsync(DanhMucKinhDoanh request, string[] DonViQuanLyList, string[] DonViDongChuyenList);
        Task<CommonResponse> DeleteAsync(Guid id_delete);
    }

    public interface IDanhMucNuocSachService
    {
        Task<CommonResponse> GetListDanhMucNuocSachAsync(string search, int pageSize, int pageCurrent);
        Task<CommonResponse> StoreAsync(DanhMucNuocSach request);
        Task<CommonResponse> EditAsync(Guid id);
        Task<CommonResponse> UpdateAsync(DanhMucNuocSach request);
        Task<CommonResponse> DeleteAsync(Guid id);
    }

    public interface IDanhMucNuocSachCtService
    {
        Task<CommonResponse> GetListDanhMucCtAsync(Guid danhMucNuocSachId, string search, int pageSize, int pageCurrent);
        Task<CommonResponse> StoreAsync(DanhMucNuocSachCt request);
        Task<CommonResponse> EditAsync(Guid id);
        Task<CommonResponse> UpdateAsync(DanhMucNuocSachCt request);
        Task<CommonResponse> DeleteAsync(Guid id);
        Task<CommonResponse> DeleteAllAsync(Guid danhMucNuocSachId);
        Task<CommonResponse> StoreRangeAsync(List<DanhMucNuocSachCt> requests);
    }

    public interface IDanhMucGiaThueTaiNguyenService
    {
        Task<CommonResponse> GetListDanhMucGiaThueTaiNguyenAsync(string search, int pageSize, int pageCurrent);
        Task<CommonResponse> StoreAsync(DanhMucGiaThueTaiNguyen request);
        Task<CommonResponse> EditAsync(Guid id);
        Task<CommonResponse> UpdateAsync(DanhMucGiaThueTaiNguyen request);
        Task<CommonResponse> DeleteAsync(Guid id);
    }

    public interface IDanhMucGiaThueTaiNguyenCtService
    {
        Task<CommonResponse> GetListDanhMucCtAsync(Guid danhMucGiaThueTaiNguyenId, string search, int pageSize, int pageCurrent);
        Task<CommonResponse> StoreAsync(DanhMucGiaThueTaiNguyenCt request);
        Task<CommonResponse> EditAsync(Guid id);
        Task<CommonResponse> UpdateAsync(DanhMucGiaThueTaiNguyenCt request);
        Task<CommonResponse> DeleteAsync(Guid id);
        Task<CommonResponse> DeleteAllAsync(Guid danhMucGiaThueTaiNguyenId);
        Task<CommonResponse> StoreRangeAsync(List<DanhMucGiaThueTaiNguyenCt> requests);
    }
}
