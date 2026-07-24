using DataAccess;
using DataAccess.Entities.QuanLyDanhMuc;
using Microsoft.EntityFrameworkCore;
using Services.Model;

namespace Services.QuanLyDanhMuc
{
    public interface IDanhMucVanBanService
    {
        Task<CommonResponse> GetDanhMucVanBansAsync(string search, int pageSize = 5, int pageCurrent = 1);
        Task<CommonResponse> StoreAsync(DanhMucVanBan request);
        Task<CommonResponse> EditAsync(Guid id);
        Task<CommonResponse> UpdateAsync(DanhMucVanBan request);
        Task<CommonResponse> DeleteAsync(Guid id);
        Task<bool> CheckDuplicateAsync(string tenLoaiVanBan, Guid id);
    }

    public class DanhMucVanBanService(ApplicationDbContext dbContext) : IDanhMucVanBanService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<CommonResponse> GetDanhMucVanBansAsync(string search, int pageSize = 5, int pageCurrent = 1)
        {
            try
            {
                var query = _dbContext.DanhMucVanBans.AsNoTracking().AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(x =>
                        x.TenLoaiVanBan.Contains(search) ||
                        x.CapChinhQuyen.Contains(search) ||
                        x.ChuTheBanHanh.Contains(search) ||
                        (x.KyHieuMau != null && x.KyHieuMau.Contains(search)) ||
                        (x.MoTa != null && x.MoTa.Contains(search)));
                }

                query = query.OrderBy(x => x.ThuTuSapXep).ThenBy(x => x.TenLoaiVanBan);

                var totalRecord = await query.CountAsync();
                var data = await query.Skip((pageCurrent - 1) * pageSize).Take(pageSize).ToListAsync();

                return new CommonResponse("success", "Thành công", data, totalRecord);
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> StoreAsync(DanhMucVanBan request)
        {
            try
            {
                if (await CheckDuplicateAsync(request.TenLoaiVanBan, Guid.Empty))
                {
                    return new CommonResponse("error", "Tên loại văn bản đã tồn tại!");
                }

                _dbContext.DanhMucVanBans.Add(request);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse("success", "Thành công");
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> EditAsync(Guid id)
        {
            try
            {
                var data = await _dbContext.DanhMucVanBans.FindAsync(id);
                if (data == null)
                {
                    return new CommonResponse("error", "Không tìm thấy thông tin!");
                }

                return new CommonResponse("success", "Thành công", data);
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> UpdateAsync(DanhMucVanBan request)
        {
            try
            {
                if (await CheckDuplicateAsync(request.TenLoaiVanBan, request.Id))
                {
                    return new CommonResponse("error", "Tên loại văn bản đã tồn tại!");
                }

                var data = await _dbContext.DanhMucVanBans.FindAsync(request.Id);
                if (data == null)
                {
                    return new CommonResponse("error", "Không tìm thấy thông tin!");
                }

                data.TenLoaiVanBan = request.TenLoaiVanBan;
                data.CapChinhQuyen = request.CapChinhQuyen;
                data.ChuTheBanHanh = request.ChuTheBanHanh;
                data.KyHieuMau = request.KyHieuMau;
                data.ThuTuSapXep = request.ThuTuSapXep;
                data.TrangThai = request.TrangThai;
                data.MoTa = request.MoTa;
                data.GhiChu = request.GhiChu;

                _dbContext.DanhMucVanBans.Update(data);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse("success", "Thành công");
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> DeleteAsync(Guid id)
        {
            try
            {
                var data = await _dbContext.DanhMucVanBans.FindAsync(id);
                if (data == null)
                {
                    return new CommonResponse("error", "Không tìm thấy thông tin!");
                }

                _dbContext.DanhMucVanBans.Remove(data);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse("success", "Thành công");
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<bool> CheckDuplicateAsync(string tenLoaiVanBan, Guid id)
        {
            return await _dbContext.DanhMucVanBans.AnyAsync(x => x.TenLoaiVanBan == tenLoaiVanBan && x.Id != id);
        }
    }
}
