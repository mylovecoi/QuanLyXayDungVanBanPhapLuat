using DataAccess;
using DataAccess.Entities.ThamDinhGia;
using Microsoft.EntityFrameworkCore;
using Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.ThamDinhGia
{
    public class ThamDinhGiaDanhMucHangHoaService(ApplicationDbContext dbContext) : IThamDinhGiaDanhMucHangHoaService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<CommonResponse> GetListThamDinhGiaDanhMucHangHoaAsync(string search, int pageSize, int pageCurrent)
        {
            try
            {
                var query = _dbContext.ThamDinhGiaDanhMucHangHoas.AsQueryable();

                if (!string.IsNullOrEmpty(search))
                {
                    search = search.ToLower();
                    query = query.Where(x =>
                        (x.TenDanhMucHangHoa != null && x.TenDanhMucHangHoa.ToLower().Contains(search))
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
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Đã xảy ra lỗi khi lấy dữ liệu: " + ex.Message };
            }
        }

        public async Task<CommonResponse> StoreAsync(ThamDinhGiaDanhMucHangHoa request)
        {
            try
            {
                request.Id = Guid.NewGuid();
                request.TrangThai = string.IsNullOrEmpty(request.TrangThai) ? "Kích hoạt" : request.TrangThai;
                _dbContext.ThamDinhGiaDanhMucHangHoas.Add(request);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Thêm danh mục hàng hóa thẩm định giá thành công!" };
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
                var data = await _dbContext.ThamDinhGiaDanhMucHangHoas.FindAsync(id);
                if (data == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin danh mục!" };
                return new CommonResponse { Status = "success", Data = data };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = ex.Message };
            }
        }

        public async Task<CommonResponse> UpdateAsync(ThamDinhGiaDanhMucHangHoa request)
        {
            try
            {
                var data = await _dbContext.ThamDinhGiaDanhMucHangHoas.FindAsync(request.Id);
                if (data == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin cần cập nhật!" };

                data.TenDanhMucHangHoa = request.TenDanhMucHangHoa;
                data.TrangThai = string.IsNullOrEmpty(request.TrangThai) ? "Kích hoạt" : request.TrangThai;

                _dbContext.ThamDinhGiaDanhMucHangHoas.Update(data);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Cập nhật danh mục hàng hóa thẩm định giá thành công!" };
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
                var data = await _dbContext.ThamDinhGiaDanhMucHangHoas.FindAsync(id);
                if (data == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin danh mục!" };

                _dbContext.ThamDinhGiaDanhMucHangHoas.Remove(data);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Xóa danh mục hàng hóa thẩm định giá thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể xóa danh mục: " + ex.Message };
            }
        }
    }
}
