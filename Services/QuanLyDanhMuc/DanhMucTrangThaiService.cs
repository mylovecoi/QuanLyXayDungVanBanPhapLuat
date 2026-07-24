using DataAccess;
using DataAccess.Entities.QuanLyDanhMuc;
using Microsoft.EntityFrameworkCore;
using Services.Model;

namespace Services.QuanLyDanhMuc
{
    public interface IDanhMucTrangThaiService
    {
        Task<CommonResponse> GetDanhMucTrangThaisAsync(string search, int pageSize = 5, int pageCurrent = 1);
        Task<CommonResponse> StoreAsync(DanhMucTrangThai request);
        Task<CommonResponse> EditAsync(Guid id);
        Task<CommonResponse> UpdateAsync(DanhMucTrangThai request);
        Task<CommonResponse> DeleteAsync(Guid id);
        Task<bool> CheckDuplicateAsync(string maTrangThai, Guid id);
        Task<int> GetNextThuTuSapXepAsync();
    }

    public class DanhMucTrangThaiService(ApplicationDbContext dbContext) : IDanhMucTrangThaiService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<CommonResponse> GetDanhMucTrangThaisAsync(string search, int pageSize = 5, int pageCurrent = 1)
        {
            try
            {
                var query = _dbContext.DanhMucTrangThais.AsNoTracking().AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(x =>
                        x.MaTrangThai.Contains(search) ||
                        x.TenTrangThai.Contains(search) ||
                        x.MaMauHex.Contains(search) ||
                        (x.MoTa != null && x.MoTa.Contains(search)) ||
                        (x.GhiChu != null && x.GhiChu.Contains(search)));
                }

                query = query.OrderBy(x => x.ThuTuSapXep).ThenBy(x => x.TenTrangThai);

                var totalRecord = await query.CountAsync();
                var data = await query.Skip((pageCurrent - 1) * pageSize).Take(pageSize).ToListAsync();

                return new CommonResponse("success", "Thanh cong", data, totalRecord);
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> StoreAsync(DanhMucTrangThai request)
        {
            try
            {
                if (await CheckDuplicateAsync(request.MaTrangThai, Guid.Empty))
                {
                    return new CommonResponse("error", "Ma trang thai da ton tai!");
                }

                _dbContext.DanhMucTrangThais.Add(request);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse("success", "Thanh cong");
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
                var data = await _dbContext.DanhMucTrangThais.FindAsync(id);
                if (data == null)
                {
                    return new CommonResponse("error", "Khong tim thay thong tin!");
                }

                return new CommonResponse("success", "Thanh cong", data);
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<CommonResponse> UpdateAsync(DanhMucTrangThai request)
        {
            try
            {
                if (await CheckDuplicateAsync(request.MaTrangThai, request.Id))
                {
                    return new CommonResponse("error", "Ma trang thai da ton tai!");
                }

                var data = await _dbContext.DanhMucTrangThais.FindAsync(request.Id);
                if (data == null)
                {
                    return new CommonResponse("error", "Khong tim thay thong tin!");
                }

                data.MaTrangThai = request.MaTrangThai;
                data.TenTrangThai = request.TenTrangThai;
                data.MaMauHex = request.MaMauHex;
                data.ThuTuSapXep = request.ThuTuSapXep;
                data.TrangThai = request.TrangThai;
                data.MoTa = request.MoTa;
                data.GhiChu = request.GhiChu;

                _dbContext.DanhMucTrangThais.Update(data);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse("success", "Thanh cong");
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
                var data = await _dbContext.DanhMucTrangThais.FindAsync(id);
                if (data == null)
                {
                    return new CommonResponse("error", "Khong tim thay thong tin!");
                }

                _dbContext.DanhMucTrangThais.Remove(data);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse("success", "Thanh cong");
            }
            catch
            {
                return new CommonResponse();
            }
        }

        public async Task<bool> CheckDuplicateAsync(string maTrangThai, Guid id)
        {
            return await _dbContext.DanhMucTrangThais.AnyAsync(x => x.MaTrangThai == maTrangThai && x.Id != id);
        }

        public async Task<int> GetNextThuTuSapXepAsync()
        {
            var maxThuTuSapXep = await _dbContext.DanhMucTrangThais
                .AsNoTracking()
                .Select(x => (int?)x.ThuTuSapXep)
                .MaxAsync();

            return (maxThuTuSapXep ?? 0) + 1;
        }
    }
}
