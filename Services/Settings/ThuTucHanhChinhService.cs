using DataAccess;
using DataAccess.Entities.Settings;
using Microsoft.EntityFrameworkCore;
using Services.Manages;
using Services.Model;

namespace Services.Settings
{
    public interface IThuTucHanhChinhService
    {
        Task<CommonResponse> GetThuTucHanhChinhsAsync(string search, int pageSize, int currentPage);
        Task<CommonResponse> StoreAsync(ThuTucHanhChinh request);
        Task<CommonResponse> EditAsync(Guid id);
        Task<CommonResponse> UpdateAsync(ThuTucHanhChinh request);
        Task<CommonResponse> DeleteAsync(Guid id);
    }

    public class ThuTucHanhChinhService(
        ApplicationDbContext context,
        IAttachedFileService attachedFileService) : IThuTucHanhChinhService
    {
        public async Task<CommonResponse> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await context.ThuTucHanhChinhs.FindAsync(id);
                if (entity is null)
                    return new CommonResponse
                    {
                        Status = "error",
                        Message = "Không tìm thấy thông tin thủ tục hành chính cần xóa"
                    };

                context.ThuTucHanhChinhs.Remove(entity);
                await attachedFileService.RemoveRangeByGroupId(id);
                await context.SaveChangesAsync();
                return new CommonResponse { Status = "success" };
            }
            catch
            {
                return new CommonResponse
                {
                    Status = "error",
                    Message = "Đã xảy ra lỗi khi xóa dữ liệu. Vui lòng thử lại sau!"
                };
            }
        }

        public async Task<CommonResponse> EditAsync(Guid id)
        {
            try
            {
                var entity = await context.ThuTucHanhChinhs.FindAsync(id);
                if (entity is null)
                    return new CommonResponse
                    {
                        Status = "error",
                        Message = "Không tìm thấy thông tin thủ tục hành chính cần cập nhật"
                    };

                return new CommonResponse
                {
                    Status = "success",
                    Data = entity
                };
            }
            catch
            {
                return new CommonResponse
                {
                    Status = "error",
                    Message = "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau!"
                };
            }
        }

        public async Task<CommonResponse> GetThuTucHanhChinhsAsync(string search, int pageSize, int pageCurrent)
        {
            try
            {
                var query = context.ThuTucHanhChinhs.Where(t =>
                    string.IsNullOrEmpty(search) ||
                    EF.Functions.Like(t.MaThuTuc, $"%{search}%") ||
                    EF.Functions.Like(t.TenThuTuc, $"%{search}%"));

                var total = await query.CountAsync();
                query = query.OrderByDescending(t => t.UpdatedDate)
                    .Skip((pageCurrent - 1) * pageSize).Take(pageSize);

                var data = await query.ToListAsync();

                foreach (var item in data)
                {
                    item.DSFileDinhKem = await attachedFileService.GetAllAttachedFilesAsync(item.Id, "ThuTucHanhChinh");
                }

                return new CommonResponse
                {
                    Status = "success",
                    Data = data,
                    TotalRecord = total
                };
            }
            catch
            {
                return new CommonResponse
                {
                    Status = "error",
                    Message = "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau!"
                };
            }
        }

        public async Task<CommonResponse> StoreAsync(ThuTucHanhChinh request)
        {
            try
            {
                context.ThuTucHanhChinhs.Add(request);
                await context.SaveChangesAsync();
                return new CommonResponse { Status = "success" };
            }
            catch
            {
                return new CommonResponse
                {
                    Status = "error",
                    Message = "Đã xảy ra lỗi khi lưu dữ liệu. Vui lòng thử lại sau!"
                };
            }
        }

        public async Task<CommonResponse> UpdateAsync(ThuTucHanhChinh request)
        {
            try
            {
                var entity = await context.ThuTucHanhChinhs.FindAsync(request.Id);
                if (entity is null)
                    return new CommonResponse
                    {
                        Status = "error",
                        Message = "Không tìm thấy thông tin thủ tục hành chính cần cập nhật"
                    };

                entity.MaThuTuc = request.MaThuTuc;
                entity.TenThuTuc = request.TenThuTuc;
                entity.TenQuyetDinh = request.TenQuyetDinh;
                entity.NgayQuyetDinh = request.NgayQuyetDinh;
                entity.CoQuanThucHien = request.CoQuanThucHien;
                entity.CachThucThucHien = request.CachThucThucHien;
                entity.DoiTuongThucHien = request.DoiTuongThucHien;
                entity.TrinhTuThucHien = request.TrinhTuThucHien;
                entity.ThoiHanGiaiQuyet = request.ThoiHanGiaiQuyet;
                entity.Phi = request.Phi;
                entity.LePhi = request.LePhi;
                entity.ThanhPhanHoSo = request.ThanhPhanHoSo;
                entity.YeuCauDieuKien = request.YeuCauDieuKien;
                entity.CanCuPhapLy = request.CanCuPhapLy;
                entity.KetQuaThucHien = request.KetQuaThucHien;

                context.ThuTucHanhChinhs.Update(entity);
                await context.SaveChangesAsync();
                return new CommonResponse { Status = "success" };
            }
            catch
            {
                return new CommonResponse
                {
                    Status = "error",
                    Message = "Đã xảy ra lỗi khi cập nhật dữ liệu. Vui lòng thử lại sau!"
                };
            }
        }
    }
}