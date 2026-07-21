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
    public class ThamDinhGiaDanhMucHangHoaCtService(ApplicationDbContext dbContext) : IThamDinhGiaDanhMucHangHoaCtService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<CommonResponse> GetListDanhMucCtAsync(Guid hangHoaId, string search, int pageSize, int pageCurrent)
        {
            try
            {
                var query = _dbContext.ThamDinhGiaDanhMucHangHoaCts
                    .Where(x => x.HangHoaId == hangHoaId)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(search))
                {
                    search = search.ToLower();
                    query = query.Where(x =>
                        (x.TenHangHoa != null && x.TenHangHoa.ToLower().Contains(search)) ||
                        (x.MaHangHoa != null && x.MaHangHoa.ToLower().Contains(search))
                    );
                }

                query = query.OrderBy(x => x.MaHangHoa).ThenByDescending(x => x.CreatedDate);

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

        public async Task<CommonResponse> StoreAsync(ThamDinhGiaDanhMucHangHoaCt request)
        {
            try
            {
                request.Id = Guid.NewGuid();
                request.TrangThai = string.IsNullOrEmpty(request.TrangThai) ? "Kích hoạt" : request.TrangThai;
                _dbContext.ThamDinhGiaDanhMucHangHoaCts.Add(request);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Thêm chi tiết danh mục hàng hóa thẩm định giá thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể thêm chi tiết danh mục: " + ex.Message };
            }
        }

        public async Task<CommonResponse> EditAsync(Guid id)
        {
            try
            {
                var data = await _dbContext.ThamDinhGiaDanhMucHangHoaCts.FindAsync(id);
                if (data == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin chi tiết danh mục!" };
                return new CommonResponse { Status = "success", Data = data };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = ex.Message };
            }
        }

        public async Task<CommonResponse> UpdateAsync(ThamDinhGiaDanhMucHangHoaCt request)
        {
            try
            {
                var data = await _dbContext.ThamDinhGiaDanhMucHangHoaCts.FindAsync(request.Id);
                if (data == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin cần cập nhật!" };

                data.MaHangHoa = request.MaHangHoa;
                data.TenHangHoa = request.TenHangHoa;
                data.QuyCachChatLuong = request.QuyCachChatLuong;
                data.ThongSoKt = request.ThongSoKt;
                data.XuatXu = request.XuatXu;
                data.DonViTinh = request.DonViTinh;
                data.TrangThai = string.IsNullOrEmpty(request.TrangThai) ? "Kích hoạt" : request.TrangThai;

                _dbContext.ThamDinhGiaDanhMucHangHoaCts.Update(data);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Cập nhật chi tiết danh mục hàng hóa thẩm định giá thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể cập nhật chi tiết danh mục: " + ex.Message };
            }
        }

        public async Task<CommonResponse> DeleteAsync(Guid id)
        {
            try
            {
                var data = await _dbContext.ThamDinhGiaDanhMucHangHoaCts.FindAsync(id);
                if (data == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin chi tiết danh mục!" };

                _dbContext.ThamDinhGiaDanhMucHangHoaCts.Remove(data);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Xóa chi tiết danh mục hàng hóa thẩm định giá thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể xóa chi tiết danh mục: " + ex.Message };
            }
        }

        public async Task<CommonResponse> DeleteAllAsync(Guid hangHoaId)
        {
            try
            {
                var list = await _dbContext.ThamDinhGiaDanhMucHangHoaCts.Where(x => x.HangHoaId == hangHoaId).ToListAsync();
                if (list.Any())
                {
                    _dbContext.ThamDinhGiaDanhMucHangHoaCts.RemoveRange(list);
                    await _dbContext.SaveChangesAsync();
                }
                return new CommonResponse { Status = "success", Message = "Xóa toàn bộ chi tiết danh mục hàng hóa thẩm định giá thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể xóa toàn bộ chi tiết danh mục: " + ex.Message };
            }
        }

        public async Task<CommonResponse> StoreRangeAsync(List<ThamDinhGiaDanhMucHangHoaCt> requests)
        {
            try
            {
                var now = DateTime.Now;
                for (int i = 0; i < requests.Count; i++)
                {
                    var request = requests[i];
                    request.Id = Guid.NewGuid();
                    request.TrangThai = string.IsNullOrEmpty(request.TrangThai) ? "Kích hoạt" : request.TrangThai;
                    request.CreatedDate = now.AddSeconds(-i);
                    _dbContext.ThamDinhGiaDanhMucHangHoaCts.Add(request);
                }
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = $"Nhận thành công {requests.Count} chi tiết hàng hóa từ file Excel!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể lưu dữ liệu từ Excel: " + ex.Message };
            }
        }
    }
}
