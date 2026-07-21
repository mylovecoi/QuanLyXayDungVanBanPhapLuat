using DataAccess;
using DataAccess.Entities.Settings;
using DataAccess.Entities.Settings.DanhMucGia;
using Microsoft.EntityFrameworkCore;
using Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Settings.DanhMucGia
{
    public class DanhMucNuocSachService(ApplicationDbContext dbContext) : IDanhMucNuocSachService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<CommonResponse> GetListDanhMucNuocSachAsync(string search, int pageSize, int pageCurrent)
        {
            var query = _dbContext.DanhMucNuocSachs.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(x => 
                    (x.MaDanhMuc != null && x.MaDanhMuc.ToLower().Contains(search)) || 
                    (x.TenDanhMuc != null && x.TenDanhMuc.ToLower().Contains(search))
                );
            }

            query = query.OrderBy(x => x.MaDanhMuc);

            var totalRecord = await query.CountAsync();
            var dataView = await query.Skip((pageCurrent - 1) * pageSize).Take(pageSize).ToListAsync();

            return new CommonResponse 
            { 
                Status = "success", 
                Data = dataView, 
                TotalRecord = totalRecord 
            };
        }

        public async Task<CommonResponse> StoreAsync(DanhMucNuocSach request)
        {
            try
            {
                request.Id = Guid.NewGuid();
                request.TrangThai = string.IsNullOrEmpty(request.TrangThai) ? "TD" : request.TrangThai;
                _dbContext.DanhMucNuocSachs.Add(request);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Thêm danh mục nước sạch thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể thêm danh mục: " + ex.Message };
            }
        }

        public async Task<CommonResponse> EditAsync(Guid id)
        {
            try
            {
                var data = await _dbContext.DanhMucNuocSachs.FindAsync(id);
                if (data == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin danh mục!" };
                return new CommonResponse { Status = "success", Data = data };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = ex.Message };
            }
        }

        public async Task<CommonResponse> UpdateAsync(DanhMucNuocSach request)
        {
            try
            {
                var data = await _dbContext.DanhMucNuocSachs.FindAsync(request.Id);
                if (data == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin cần cập nhật!" };

                data.MaDanhMuc = request.MaDanhMuc;
                data.TenDanhMuc = request.TenDanhMuc;
                data.TrangThai = request.TrangThai;

                _dbContext.DanhMucNuocSachs.Update(data);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Cập nhật danh mục nước sạch thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể cập nhật danh mục: " + ex.Message };
            }
        }

        public async Task<CommonResponse> DeleteAsync(Guid id)
        {
            try
            {
                var data = await _dbContext.DanhMucNuocSachs.FindAsync(id);
                if (data == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin danh mục!" };

                // Xóa cả chi tiết liên quan
                var details = await _dbContext.DanhMucNuocSachCts.Where(x => x.DanhMucNuocSachId == id).ToListAsync();
                if (details.Any())
                {
                    _dbContext.DanhMucNuocSachCts.RemoveRange(details);
                }

                _dbContext.DanhMucNuocSachs.Remove(data);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Xóa danh mục nước sạch thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể xóa danh mục: " + ex.Message };
            }
        }
    }
}
