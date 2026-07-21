using DataAccess;
using DataAccess.Entities.DinhGiaHHDV;
using Microsoft.EntityFrameworkCore;
using Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.DinhGiaHHDV.GiaThiTruong
{
    public class GiaThiTruongDanhMucService(ApplicationDbContext dbContext) : IGiaThiTruongDanhMucService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<CommonResponse> GetListGiaThiTruongDanhMucAsync(string search, int pageSize, int pageCurrent)
        {
            var query = _dbContext.GiaThiTruongDanhMucs.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(x =>
                    (x.TenTT != null && x.TenTT.ToLower().Contains(search))
                );
            }

            query = query.OrderByDescending(x => x.CreatedDate);

            var totalRecord = await query.CountAsync();
            var dataView = await query.Skip((pageCurrent - 1) * pageSize).Take(pageSize).ToListAsync();

            return new CommonResponse
            {
                Status = "success",
                Data = dataView,
                TotalRecord = totalRecord
            };
        }

        public async Task<CommonResponse> StoreAsync(GiaThiTruongDanhMuc request)
        {
            try
            {
                request.Id = Guid.NewGuid();
                request.TheoDoi = string.IsNullOrEmpty(request.TheoDoi) ? "TD" : request.TheoDoi;
                _dbContext.GiaThiTruongDanhMucs.Add(request);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Thêm danh mục giá thị trường thành công!" };
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
                var data = await _dbContext.GiaThiTruongDanhMucs.FindAsync(id);
                if (data == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin danh mục!" };
                return new CommonResponse { Status = "success", Data = data };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = ex.Message };
            }
        }

        public async Task<CommonResponse> UpdateAsync(GiaThiTruongDanhMuc request)
        {
            try
            {
                var data = await _dbContext.GiaThiTruongDanhMucs.FindAsync(request.Id);
                if (data == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin cần cập nhật!" };

                data.TenTT = request.TenTT;
                data.ThoiDiemBanHanhTT = request.ThoiDiemBanHanhTT;
                data.TheoDoi = string.IsNullOrEmpty(request.TheoDoi) ? "TD" : request.TheoDoi;

                _dbContext.GiaThiTruongDanhMucs.Update(data);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Cập nhật danh mục giá thị trường thành công!" };
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
                var data = await _dbContext.GiaThiTruongDanhMucs.FindAsync(id);
                if (data == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin danh mục!" };

                _dbContext.GiaThiTruongDanhMucs.Remove(data);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Xóa danh mục giá thị trường thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể xóa danh mục: " + ex.Message };
            }
        }
    }
}
