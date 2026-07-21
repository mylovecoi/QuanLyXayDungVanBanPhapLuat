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
    public class ThamDinhGiaHoiDongCtService(ApplicationDbContext dbContext) : IThamDinhGiaHoiDongCtService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<CommonResponse> GetListDanhMucCtAsync(Guid hoiDongId, string search, int pageSize, int pageCurrent)
        {
            try
            {
                var query = _dbContext.ThamDinhGiaHoiDongCts
                    .Where(x => x.HoiDongId == hoiDongId)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(search))
                {
                    search = search.ToLower();
                    query = query.Where(x =>
                        (x.HoTen != null && x.HoTen.ToLower().Contains(search)) ||
                        (x.ChucVu != null && x.ChucVu.ToLower().Contains(search)) ||
                        (x.VaiTro != null && x.VaiTro.ToLower().Contains(search))
                    );
                }

                query = query.OrderBy(x => x.STTSapXep).ThenByDescending(x => x.CreatedDate);

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

        public async Task<CommonResponse> StoreAsync(ThamDinhGiaHoiDongCt request)
        {
            try
            {
                request.Id = Guid.NewGuid();
                _dbContext.ThamDinhGiaHoiDongCts.Add(request);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Thêm thành viên hội đồng thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể thêm thành viên: " + ex.Message };
            }
        }

        public async Task<CommonResponse> EditAsync(Guid id)
        {
            try
            {
                var data = await _dbContext.ThamDinhGiaHoiDongCts.FindAsync(id);
                if (data == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin thành viên!" };
                return new CommonResponse { Status = "success", Data = data };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = ex.Message };
            }
        }

        public async Task<CommonResponse> UpdateAsync(ThamDinhGiaHoiDongCt request)
        {
            try
            {
                var data = await _dbContext.ThamDinhGiaHoiDongCts.FindAsync(request.Id);
                if (data == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin cần cập nhật!" };

                data.STTSapXep = request.STTSapXep;
                data.HoTen = request.HoTen;
                data.ChucVu = request.ChucVu;
                data.VaiTro = request.VaiTro;

                _dbContext.ThamDinhGiaHoiDongCts.Update(data);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Cập nhật thành viên hội đồng thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể cập nhật thành viên: " + ex.Message };
            }
        }

        public async Task<CommonResponse> DeleteAsync(Guid id)
        {
            try
            {
                var data = await _dbContext.ThamDinhGiaHoiDongCts.FindAsync(id);
                if (data == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin thành viên!" };

                _dbContext.ThamDinhGiaHoiDongCts.Remove(data);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Xóa thành viên hội đồng thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể xóa thành viên: " + ex.Message };
            }
        }

        public async Task<CommonResponse> DeleteAllAsync(Guid hoiDongId)
        {
            try
            {
                var list = await _dbContext.ThamDinhGiaHoiDongCts.Where(x => x.HoiDongId == hoiDongId).ToListAsync();
                if (list.Any())
                {
                    _dbContext.ThamDinhGiaHoiDongCts.RemoveRange(list);
                    await _dbContext.SaveChangesAsync();
                }
                return new CommonResponse { Status = "success", Message = "Xóa toàn bộ thành viên hội đồng thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể xóa toàn bộ thành viên: " + ex.Message };
            }
        }

        public async Task<CommonResponse> StoreRangeAsync(List<ThamDinhGiaHoiDongCt> requests)
        {
            try
            {
                var now = DateTime.Now;
                for (int i = 0; i < requests.Count; i++)
                {
                    var request = requests[i];
                    request.Id = Guid.NewGuid();
                    request.CreatedDate = now.AddSeconds(-i);
                    _dbContext.ThamDinhGiaHoiDongCts.Add(request);
                }
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = $"Nhận thành công {requests.Count} thành viên hội đồng từ file Excel!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể lưu dữ liệu từ Excel: " + ex.Message };
            }
        }
    }
}
