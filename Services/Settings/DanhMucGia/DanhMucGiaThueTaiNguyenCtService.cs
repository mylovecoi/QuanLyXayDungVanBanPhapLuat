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
    public class DanhMucGiaThueTaiNguyenCtService(ApplicationDbContext dbContext) : IDanhMucGiaThueTaiNguyenCtService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<CommonResponse> GetListDanhMucCtAsync(Guid danhMucGiaThueTaiNguyenId, string search, int pageSize, int pageCurrent)
        {
            var query = _dbContext.DanhMucGiaThueTaiNguyenCts
                .Where(x => x.DanhMucGiaThueTaiNguyenId == danhMucGiaThueTaiNguyenId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(x =>
                    (x.Ten != null && x.Ten.ToLower().Contains(search)) ||
                    (x.Cap1 != null && x.Cap1.ToLower().Contains(search)) ||
                    (x.Cap2 != null && x.Cap2.ToLower().Contains(search)) ||
                    (x.Cap3 != null && x.Cap3.ToLower().Contains(search)) ||
                    (x.Cap4 != null && x.Cap4.ToLower().Contains(search)) ||
                    (x.Cap5 != null && x.Cap5.ToLower().Contains(search)) ||
                    (x.Cap6 != null && x.Cap6.ToLower().Contains(search))
                );
            }

            query = query.OrderBy(x => x.STTSapXep);

            var totalRecord = await query.CountAsync();
            var dataView = await query.Skip((pageCurrent - 1) * pageSize).Take(pageSize).ToListAsync();

            return new CommonResponse
            {
                Status = "success",
                Data = dataView,
                TotalRecord = totalRecord
            };
        }

        public async Task<CommonResponse> StoreAsync(DanhMucGiaThueTaiNguyenCt request)
        {
            try
            {
                request.Id = Guid.NewGuid();

                if (request.STTSapXep == 0)
                {
                    var currentItems = await _dbContext.DanhMucGiaThueTaiNguyenCts
                        .Where(x => x.DanhMucGiaThueTaiNguyenId == request.DanhMucGiaThueTaiNguyenId)
                        .ToListAsync();

                    int maxStt = currentItems.Any() ? currentItems.Max(x => x.STTSapXep) : 0;
                    request.STTSapXep = maxStt + 1;
                }

                _dbContext.DanhMucGiaThueTaiNguyenCts.Add(request);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Thêm chi tiết danh mục giá thuê tài nguyên thành công!" };
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
                var data = await _dbContext.DanhMucGiaThueTaiNguyenCts.FindAsync(id);
                if (data == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin chi tiết danh mục!" };
                return new CommonResponse { Status = "success", Data = data };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = ex.Message };
            }
        }

        public async Task<CommonResponse> UpdateAsync(DanhMucGiaThueTaiNguyenCt request)
        {
            try
            {
                var data = await _dbContext.DanhMucGiaThueTaiNguyenCts.FindAsync(request.Id);
                if (data == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin cần cập nhật!" };

                data.Cap1 = request.Cap1;
                data.Cap2 = request.Cap2;
                data.Cap3 = request.Cap3;
                data.Cap4 = request.Cap4;
                data.Cap5 = request.Cap5;
                data.Cap6 = request.Cap6;
                data.Ten = request.Ten;
                data.DonViTinh = request.DonViTinh;
                data.STTSapXep = request.STTSapXep;

                _dbContext.DanhMucGiaThueTaiNguyenCts.Update(data);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Cập nhật chi tiết danh mục giá thuê tài nguyên thành công!" };
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
                var data = await _dbContext.DanhMucGiaThueTaiNguyenCts.FindAsync(id);
                if (data == null) return new CommonResponse { Status = "error", Message = "Không tìm thấy thông tin chi tiết danh mục!" };

                _dbContext.DanhMucGiaThueTaiNguyenCts.Remove(data);
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = "Xóa chi tiết danh mục giá thuê tài nguyên thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể xóa chi tiết danh mục: " + ex.Message };
            }
        }

        public async Task<CommonResponse> DeleteAllAsync(Guid danhMucGiaThueTaiNguyenId)
        {
            try
            {
                var list = await _dbContext.DanhMucGiaThueTaiNguyenCts.Where(x => x.DanhMucGiaThueTaiNguyenId == danhMucGiaThueTaiNguyenId).ToListAsync();
                if (list.Any())
                {
                    _dbContext.DanhMucGiaThueTaiNguyenCts.RemoveRange(list);
                    await _dbContext.SaveChangesAsync();
                }
                return new CommonResponse { Status = "success", Message = "Xóa toàn bộ chi tiết danh mục giá thuê tài nguyên thành công!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể xóa toàn bộ chi tiết danh mục: " + ex.Message };
            }
        }

        public async Task<CommonResponse> StoreRangeAsync(List<DanhMucGiaThueTaiNguyenCt> requests)
        {
            try
            {
                var now = DateTime.Now;
                for (int i = 0; i < requests.Count; i++)
                {
                    var request = requests[i];
                    request.Id = Guid.NewGuid();
                    request.CreatedDate = now.AddSeconds(-i);
                    _dbContext.DanhMucGiaThueTaiNguyenCts.Add(request);
                }
                await _dbContext.SaveChangesAsync();
                return new CommonResponse { Status = "success", Message = $"Nhận thành công {requests.Count} chi tiết giá thuê tài nguyên từ file Excel!" };
            }
            catch (Exception ex)
            {
                return new CommonResponse { Status = "error", Message = "Không thể lưu dữ liệu từ Excel: " + ex.Message };
            }
        }
    }
}
