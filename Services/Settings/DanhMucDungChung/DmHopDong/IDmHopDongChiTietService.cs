using DataAccess.Entities.Settings;
using Services.Model;

namespace Services.Settings.DanhMucDungChung.DmHopDong
{
    public interface IDmHopDongChiTietService
    {
        Task<CommonResponse> GetListByDanhMucIdAsync(Guid danhMucId);
        Task<ICollection<DanhMucHopDongChiTiet>> GetRawListByDanhMucIdAsync(Guid danhMucId);
        Task<DanhMucHopDongChiTiet?> GetByIdAsync(Guid id);
        Task<CommonResponse> StoreAsync(DanhMucHopDongChiTiet request);
        Task<CommonResponse> UpdateAsync(DanhMucHopDongChiTiet request);
        Task<CommonResponse> DeleteAsync(Guid id);
    }
}




