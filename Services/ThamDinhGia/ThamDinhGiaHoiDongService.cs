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
    public class ThamDinhGiaHoiDongService(ApplicationDbContext dbContext) : IThamDinhGiaHoiDongService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<CommonResponse> GetListThamDinhGiaHoiDongAsync(string search, int pageSize, int pageCurrent)
        {
            try
            {
                var query = _dbContext.ThamDinhGiaHoiDongs.AsQueryable();

                if (!string.IsNullOrEmpty(search))
                {
                    search = search.ToLower();
                    query = query.Where(x =>
                        (x.TenHoiDong != null && x.TenHoiDong.ToLower().Contains(search)) ||
                        (x.SoQd != null && x.SoQd.ToLower().Contains(search)) ||
                        (x.ChuTichHoiDong != null && x.ChuTichHoiDong.ToLower().Contains(search))
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

        public async Task<CommonResponse> StoreAsync(ThamDinhGiaHoiDong request)
        {
            try
            {
                request.Id = Guid.NewGuid();
                _dbContext.ThamDinhGiaHoiDongs.Add(request);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Thêm thông tin hội đồng thẩm định giá thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể thêm hội đồng: " + ex.Message };
            }
        }

        public async Task<CommonResponse> EditAsync(Guid id)
        {
            try
            {
                var data = await _dbContext.ThamDinhGiaHoiDongs.FindAsync(id);
                if (data == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin hội đồng!" };
                return new CommonResponse { Status = "success", Data = data };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = ex.Message };
            }
        }

        public async Task<CommonResponse> UpdateAsync(ThamDinhGiaHoiDong request)
        {
            try
            {
                var data = await _dbContext.ThamDinhGiaHoiDongs.FindAsync(request.Id);
                if (data == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin cần cập nhật!" };

                data.ToTung = request.ToTung;
                data.CanCuPhapLy = request.CanCuPhapLy;
                data.TheoDeNghi = request.TheoDeNghi;
                data.CapHoiDong = request.CapHoiDong;
                data.LoaiHoiDong = request.LoaiHoiDong;
                data.SoQd = request.SoQd;
                data.NgayQd = request.NgayQd;
                data.CoQuanBanHanh = request.CoQuanBanHanh;
                data.TenHoiDong = request.TenHoiDong;
                data.ChuTichHoiDong = request.ChuTichHoiDong;
                data.ChucVu = request.ChucVu;
                data.NhiemVuHoiDong = request.NhiemVuHoiDong;
                data.NoiDungQd = request.NoiDungQd;
                data.MaTinhApDung = request.MaTinhApDung;
                data.MaHuyenApDung = request.MaHuyenApDung;
                if (!string.IsNullOrEmpty(request.Ipf1))
                {
                    data.Ipf1 = request.Ipf1;
                }

                _dbContext.ThamDinhGiaHoiDongs.Update(data);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Cập nhật thông tin hội đồng thẩm định giá thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể cập nhật hội đồng: " + ex.Message };
            }
        }

        public async Task<CommonResponse> DeleteAsync(Guid id)
        {
            try
            {
                var data = await _dbContext.ThamDinhGiaHoiDongs.FindAsync(id);
                if (data == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin hội đồng!" };

                // Xóa cả chi tiết hội đồng
                var details = await _dbContext.ThamDinhGiaHoiDongCts.Where(x => x.HoiDongId == id).ToListAsync();
                if (details.Any())
                {
                    _dbContext.ThamDinhGiaHoiDongCts.RemoveRange(details);
                }

                _dbContext.ThamDinhGiaHoiDongs.Remove(data);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Xóa thông tin hội đồng thẩm định giá thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể xóa hội đồng: " + ex.Message };
            }
        }
    }
}
